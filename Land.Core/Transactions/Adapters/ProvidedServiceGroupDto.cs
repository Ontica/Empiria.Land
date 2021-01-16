/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : ProvidedServiceGroupDto                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that groups services provided by government offices.                                *
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

} // namespace Empiria.Land.Transactions.Adapters
