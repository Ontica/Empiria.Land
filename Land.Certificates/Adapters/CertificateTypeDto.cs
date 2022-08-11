/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : CertificateTypeDto                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that returns payment analytics data.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Certificates {

  /// <summary>DTO with a certificate type with its commands used for certificate issuing.</summary>
  public class CertificateTypeDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public FixedList<CertificateIssuingCommandDto> IssuingCommands {
      get; internal set;
    }

  }  // class CertificateTypeDto


  public class CertificateIssuingCommandDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public CertificateIssuingCommandRuleDto Rules {
      get; internal set;
    } = new CertificateIssuingCommandRuleDto();


  }  // class CertificateIssuingCommandDto



  public class CertificateIssuingCommandRuleDto {

    public RecordableSubjectType SubjectType {
      get; internal set;
    } = RecordableSubjectType.None;


    public bool SelectSubject {
      get; internal set;
    }

    public bool SelectBookEntry {
      get; internal set;
    }

  }  // class CertificateIssuingCommandRuleDto

} // namespace Empiria.Land.Certificates.UseCases
