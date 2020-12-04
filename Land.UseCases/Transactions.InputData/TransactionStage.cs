/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Enumeration                             *
*  Type     : TransactionStage                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the different workflow stages for a transaction.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Enumerates the different workflow stages for a transaction.</summary>
  public enum TransactionStage {

    Pending,

    InProgress,

    Completed,

    Returned,

    OnHold,

    All

  }

}  // namespace Empiria.Land.Transactions.UseCases
