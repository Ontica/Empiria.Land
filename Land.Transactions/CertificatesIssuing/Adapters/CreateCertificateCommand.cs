/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Command payload                         *
*  Type     : CreateCertificateCommand                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for create land certificates.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Certificates {

  /// <summary>Command used for for create land certificates.</summary>
  public class CreateCertificateCommand {

    public CreateCertificateCommandType Type {
      get; set;
    } = CreateCertificateCommandType.Undefined;


    public CreateCertificateCommandPayload Payload {
      get; set;
    } = new CreateCertificateCommandPayload();


    internal void EnsureIsValid() {
      Assertion.Require(Type != CreateCertificateCommandType.Undefined,
                        "Unrecognized CreateCertificateCommand.Type");

      Assertion.Require(Payload, "CreateCertificateCommand.Payload");

      Assertion.Require(Payload.CertificateTypeUID,
                        "CreateCertificateCommand.Payload.CertificateTypeUID");
    }

  }  // class CreateCertificateCommand


  /// <summary>Command payload used for create land certificates.</summary>
  public class CreateCertificateCommandPayload {

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


  }  // class CreateCertificateCommandPayload

}  // namespace Empiria.Land.Certificates
