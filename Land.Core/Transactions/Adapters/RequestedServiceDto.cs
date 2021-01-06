/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RequestedServiceDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that represents data about a transaction requested service.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO that represents data about a transaction requested service.</summary>
  public class RequestedServiceDto {

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string TypeName {
      get; internal set;
    }

    public string TreasuryCode {
      get; internal set;
    }

  }  // class RequestedServiceDto

} // namespace Empiria.Land.Transactions.Adapters
