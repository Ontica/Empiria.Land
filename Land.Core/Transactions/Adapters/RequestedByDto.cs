/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RequestedByDto                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for transaction requester data.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Output DTO for transaction requester data.</summary>
  public class RequestedByDto {

    public string Name {
      get; internal set;
    }

    public string Email {
      get; internal set;
    }

  }  // class RequestedByDto

}  // namespace Empiria.Land.Transactions.Adapters
