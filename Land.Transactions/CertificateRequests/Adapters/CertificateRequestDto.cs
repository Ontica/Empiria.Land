/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate Requests                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Data Transfer Object                    *
*  Type     : CertificateRequestDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO with a land certificate requested within a transaction context.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.RecordableSubjects.Adapters;

using Empiria.Land.Certificates;


namespace Empiria.Land.Transactions.CertificateRequests {

  /// <summary>DTO with a land certificate requested within a transaction context.</summary>
  public class CertificateRequestDto {

    internal CertificateRequestDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string Type {
      get; internal set;
    }


    public string CertificateID {
      get; internal set;
    }


    public RecordableSubjectDto RecordableSubject {
      get; internal set;
    }


    public MediaData MediaLink {
      get; internal set;
    } = MediaData.Empty;


    public string Status {
      get; internal set;
    }


    public RecordingContextDto IssuingRecordingContext {
      get; internal set;
    }


    public CertificateActions Actions {
      get; internal set;
    }

  }  // class CertificateRequestDto

} // namespace Empiria.Land.Transactions.CertificateRequests
