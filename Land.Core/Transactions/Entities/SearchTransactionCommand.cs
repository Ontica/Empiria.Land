/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Command payload                         *
*  Type     : SearchTransactionCommand                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used for transaction searching.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions {

  /// <summary>Command payload used for transaction searching.</summary>
  public class SearchTransactionCommand {

    public TransactionStage Stage {
      get;
      set;
    } = TransactionStage.All;


    public TransactionStatus Status {
      get;
      set;
    } = TransactionStatus.All;


    public string Keywords {
      get;
      set;
    } = string.Empty;


    public string OrderBy {
      get;
      set;
    } = String.Empty;


    public int PageSize {
      get;
      set;
    } = 50;


    public int Page {
      get;
      set;
    } = 1;

  }  // class SearchTransactionCommand

}  // namespace Empiria.Land.Transactions
