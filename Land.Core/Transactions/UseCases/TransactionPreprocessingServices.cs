/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Preprocessing                 Component : Services Layer                          *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Services provider                       *
*  Type     : TransactionPreprocessingServices           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction searching and retrieving.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Preprocessing.Services {

  /// <summary>Use cases for transaction searching and retrieving.</summary>
  public partial class TransactionPreprocessingServices : Service {

    #region Constructors and parsers

    protected TransactionPreprocessingServices() {
      // no-op
    }

    static public TransactionPreprocessingServices ServiceInteractor() {
      return Service.CreateInstance<TransactionPreprocessingServices>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public TransactionPreprocessingDto GetPreprocessingData(string transactionUID) {
      Assertion.Require(transactionUID, "transactionUID");

      var transaction = LRSTransaction.Parse(transactionUID);

      var preprocessingControlData = new TransactionPreprocessingData(transaction);

      return TransactionPreprocessingMapper.Map(preprocessingControlData);
    }


    #endregion Use cases

  }  // class TransactionPreprocessingServices

}  // namespace Empiria.Land.Transactions.Preprocessing.UseCases
