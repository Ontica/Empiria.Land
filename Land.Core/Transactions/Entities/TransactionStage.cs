/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : TransactionStage                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the different workflow stages for a transaction.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions {

  /// <summary>Enumerates the different workflow stages for a transaction.</summary>
  public enum TransactionStage {

    Pending,

    InProgress,

    Completed,

    Returned,

    OnHold,

    All

  }  // enum TransactionStage

}  // namespace Empiria.Land.Transactions
