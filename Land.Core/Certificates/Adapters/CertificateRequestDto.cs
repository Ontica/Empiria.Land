/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : CertificateRequestDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO with a land certificate requested within a transaction context.                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Storage;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Certificates.Adapters {

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

} // namespace Empiria.Land.Certificates.Adapters
