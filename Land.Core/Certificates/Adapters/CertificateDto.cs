/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Data Transfer Object                    *
*  Type     : CertificateDto                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO with land certificate data.                                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Certificates {

  /// <summary>DTO with land certificate data.</summary>
  public class CertificateDto {

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


  }  // class CertificateDto



  /// <summary>Holds the actions that can be executed for a land certificate.</summary>
  public class CertificateActions {

    internal CertificateActions() {
      // no-op
    }

    public bool CanClose {
      get; internal set;
    }


    public bool CanDelete {
      get; internal set;
    }


    public bool CanOpen {
      get; internal set;
    }

  }  // class CertificateActions

} // namespace Empiria.Land.Certificates
