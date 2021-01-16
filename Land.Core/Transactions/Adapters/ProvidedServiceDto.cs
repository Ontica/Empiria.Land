/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : ProvidedServiceDto                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that represents data about a government organization provided service.              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

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

}  // namespace Empiria.Land.Transactions.Adapters
