﻿/* Empiria Land **********************************************************************************************
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

using Empiria.Land.Transactions;

using Empiria.Land.ESign.Adapters;
using Empiria.Land.ESign.Data;
using Empiria.Land.Registration;

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

    public FixedList<TransactionDescriptor> GetMyESignRequestedTransactions(ESignRequestsQuery query) {
      Assertion.Require(query, nameof(query));

      query.RecorderOfficeId = Permissions.GetUserDefaultRecorderOffice().Id;

      query.EnsureIsValid();

      string filter = query.MapToFilterString();
      string sort = query.MapToSortString();

      var list = ESignDataService.GetESignRequestedTransactions(filter, sort, query.PageSize);

      return TransactionMapper.MapToDescriptor(list);
    }

    #endregion Use cases

  } // class ESignRequestsUseCases

} // namespace Empiria.Land.ESign.UseCases
