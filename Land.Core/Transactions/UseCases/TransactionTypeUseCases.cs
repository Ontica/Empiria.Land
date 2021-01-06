﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionTypeUseCases                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction types.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Services;

using Empiria.Land.Transactions.Adapters;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Use cases for transaction searching and retrieving.</summary>
  public class TransactionTypeUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionTypeUseCases() {
      // no-op
    }

    static public TransactionTypeUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionTypeUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<TransactionTypeDto> GetTransactionTypes() {
      FixedList<LRSTransactionType> list = LRSTransactionType.GetList();

      return TransactionTypeDtoMapper.Map(list);
    }

    #endregion Use cases

  }  // class TransactionTypeUseCases

}  // namespace Empiria.Land.Transactions.UseCases