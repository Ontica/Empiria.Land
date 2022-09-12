/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate Requests                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Command payload                         *
*  Type     : CertificateRequestCommand                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for request land certificates within a transaction context.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Certificates;
using Empiria.Land.Registration;

namespace Empiria.Land.Transactions.CertificateRequests {

  /// <summary>Command payload used for request land certificates within a transaction context.</summary>
  public class CertificateRequestCommand {

    public CertificateRequestCommandType Type {
      get; set;
    } = CertificateRequestCommandType.Undefined;


    public CertificateRequestCommandPayload Payload {
      get; set;
    } = new CertificateRequestCommandPayload();


    internal void EnsureIsValid() {
      Assertion.Require(Type != CertificateRequestCommandType.Undefined,
                        "Unrecognized CreateTransactionCertificateCommandType.Type");

      Assertion.Require(Payload, "CreateTransactionCertificateCommandType.Payload");

      Assertion.Require(Payload.CertificateTypeUID,
                        "CreateTransactionCertificateCommandType.Payload.CertificateTypeUID");
    }


  }  // class CertificateRequestCommand



  /// <summary>Command payload of a CertificateRequestCommand.</summary>
  public class CertificateRequestCommandPayload {

    public string CertificateTypeUID {
      get; set;
    } = string.Empty;


    public string RecordableSubjectUID {
      get; set;
    } = string.Empty;


    public string RecordingBookUID {
      get; set;
    } = string.Empty;


    public string BookEntryUID {
      get; set;
    } = string.Empty;


    public string BookEntryNo {
      get; set;
    } = string.Empty;


    public DateTime PresentationTime {
      get; set;
    } = ExecutionServer.DateMinValue;


    public DateTime AuthorizationDate {
      get; set;
    } = ExecutionServer.DateMinValue;

  }  // class CertificateRequestCommandPayload



  /// <summary>Extension methods for CertificateRequestCommand.</summary>
  static internal class CertificateRequestCommandExtensions {

    static internal CertificateType GetCertificateType(this CertificateRequestCommand command) {
      return (CertificateType) CertificateType.Parse(command.Payload.CertificateTypeUID);
    }


    static internal Resource GetRecordableSubject(this CertificateRequestCommand command) {
      var registrationHelper = new RecordableSubjectRegistrationHelper(command);

      return registrationHelper.GetRecordableSubject();
    }

  }

}  // namespace Empiria.Land.Transactions.CertificateRequests
