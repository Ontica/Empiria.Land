/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : CertificateRequestTypeDto                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO with a certificate request type and its command definitions used for                       *
*             request land certificates within a transaction context.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Certificates.Adapters {

  /// <summary>DTO with a certificate request type and its command definitions used for
  /// request land certificates within a transaction context.</summary>
  public class CertificateRequestTypeDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public FixedList<CertificateRequestCommandTypeDto> IssuingCommands {
      get; internal set;
    } = new FixedList<CertificateRequestCommandTypeDto>();

  }  // class CertificateRequestTypeDto



  /// <summary>Represents a certificate request command type.</summary>
  public class CertificateRequestCommandTypeDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public CertificateRequestCommandTypeRulesDto Rules {
      get; internal set;
    } = new CertificateRequestCommandTypeRulesDto();


  }  // class CertificateRequestCommandTypeDto


  /// <summary>Holds the rules for a certificate request command type.</summary>
  public class CertificateRequestCommandTypeRulesDto {

    public RecordableSubjectType SubjectType {
      get; internal set;
    } = RecordableSubjectType.None;


    public bool SelectSubject {
      get; internal set;
    }

    public bool SelectBookEntry {
      get; internal set;
    }

    public bool GivePersonName {
      get; internal set;
    }

    public bool GiveRealEstateDescription {
      get; internal set;
    }


  }  // class CertificateRequestCommandTypeRulesDto

} // namespace Empiria.Land.Certificates.Adapters
