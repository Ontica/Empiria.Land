/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : ProvidedServiceDto                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that represents data about a government organization provided service.              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO that groups services provided by government offices.</summary>
  public class ProvidedServiceGroupDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public ProvidedServiceDto[] Services {
      get; internal set;
    }

  }  // class ProvidedServiceGroupDto


  /// <summary>Output DTO that represents data about a government organization provided service.</summary>
  public class ProvidedServiceDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public NamedEntityDto Unit {
      get; internal set;
    }

    public FeeConceptDto[] FeeConcepts {
      get; internal set;
    }

  }  // class ProvidedServiceDto



  /// <summary>Output DTO with data related to a transaction concept fee.</summary>
  public class FeeConceptDto {

    public string UID {
      get; internal set;
    }

    public string LegalBasis {
      get; internal set;
    }

    public string FinancialCode {
      get; internal set;
    }

    public bool RequiresTaxableBase {
      get; internal set;
    }

  }  // public class FeeConceptDto

}  // namespace Empiria.Land.Transactions.Adapters
