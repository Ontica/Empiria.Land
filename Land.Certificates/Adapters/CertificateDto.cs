/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Data Transfer Object                    *
*  Type     : CertificateDto                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that links transactions with Land certificates.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Certificates {

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

  }  // class CertificateDto

} // namespace Empiria.Land.Certificates
