/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Interface adapters                      *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Enumeration                             *
*  Type     : TransactionStage                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerates the different workflow stages for a transaction.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions {

  /// <summary>Enumerates the different workflow stages for a transaction.</summary>
  public enum TransactionStage {

    MyInbox,

    Pending,

    InProgress,

    ControlDesk,

    Completed,

    Returned,

    OnHold,

    All

  }  // enum TransactionStage

}  // namespace Empiria.Land.Transactions
