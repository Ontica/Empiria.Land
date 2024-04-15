/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Preprocessing                  Component : Services Layer                          *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Services provider                       *
*  Type     : TransactionPreprocessingUseCases           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction preprocessing.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Transactions.Preprocessing.Adapters;

namespace Empiria.Land.Transactions.Preprocessing.UseCases {

  /// <summary>Use cases for transaction preprocessing.</summary>
  public partial class TransactionPreprocessingUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionPreprocessingUseCases() {
      // no-op
    }

    static public TransactionPreprocessingUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionPreprocessingUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public TransactionPreprocessingDto GetPreprocessingData(string transactionUID) {
      Assertion.Require(transactionUID, nameof(transactionUID));

      var transaction = LRSTransaction.Parse(transactionUID);

      var preprocessingControlData = new TransactionPreprocessingControlData(transaction);

      return TransactionPreprocessingMapper.Map(preprocessingControlData);
    }


    #endregion Use cases

  }  // class TransactionPreprocessingUseCases

}  // namespace Empiria.Land.Transactions.Preprocessing.UseCases
