/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Electronic Sign                            Component : Use cases Layer                         *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Use case interactor class               *
*  Type     : ESignRequestsUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that returns electronic sign requests.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.Land.ESign.Adapters;

using Empiria.Land.Transactions;

namespace Empiria.Land.ESign.UseCases {

  /// <summary>Use cases that returns electronic sign requests</summary>
  public class ESignRequestsUseCases : UseCase {

    #region Constructors and parsers

    protected ESignRequestsUseCases() {
      // no-op
    }

    static public ESignRequestsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<ESignRequestsUseCases>();
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

  } // class ESignRequestsUseCases

} // namespace Empiria.Land.ESign.UseCases
