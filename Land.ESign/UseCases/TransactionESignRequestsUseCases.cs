/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Use cases Layer                         *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Use case interactor class               *
*  Type     : TransactionESignRequestsUseCases           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that returns transactions electronic sign requests.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.ESign.Adapters;

using Empiria.Land.Transactions;

namespace Empiria.Land.ESign.UseCases {

  /// <summary>Use cases that returns transactions electronic sign requests.</summary>
  public class TransactionESignRequestsUseCases : UseCase {

    #region Constructors and parsers

    protected TransactionESignRequestsUseCases() {
      // no-op
    }

    static public TransactionESignRequestsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<TransactionESignRequestsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<TransactionDescriptor> GetMyTransactionsRefusedRequests(ESignRequestsQuery query) {
      throw new NotImplementedException();
    }


    public FixedList<TransactionDescriptor> GetMyTransactionsRevokedRequests(ESignRequestsQuery query) {
      throw new NotImplementedException();
    }


    public FixedList<TransactionDescriptor> GetMyTransactionsSignedRequests(ESignRequestsQuery query) {
      throw new NotImplementedException();
    }


    public FixedList<TransactionDescriptor> GetMyTransactionsToRevokeRequests(ESignRequestsQuery query) {
      throw new NotImplementedException();
    }


    public FixedList<TransactionDescriptor> GetMyTransactionsToSignRequests(ESignRequestsQuery query) {
      throw new NotImplementedException();
    }

    #endregion Use cases

  } // class TransactionESignRequestsUseCases

} // namespace Empiria.Land.ESign.UseCases
