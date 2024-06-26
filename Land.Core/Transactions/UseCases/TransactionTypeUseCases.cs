﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionTypeUseCases                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction types.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Services;

using Empiria.Contacts;

using Empiria.Land.Registration;

using Empiria.Land.Transactions.Adapters;

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Use cases for transaction types.</summary>
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

    public FixedList<NamedEntityDto> Agencies() {
      FixedList<Contact> list = LRSTransaction.GetAgenciesList();

      return list.MapToNamedEntityList();
    }


    public FixedList<NamedEntityDto> FilingOffices() {
      var offices = RecorderOffice.GetList();

      offices = base.RestrictUserDataAccessTo(offices);

      return offices.MapToNamedEntityList();
    }


    public TransactionTypeDto GetTransactionType(string transactionTypeUID) {
      Assertion.Require(transactionTypeUID, "transactionTypeUID");

      var transactionType = LRSTransactionType.Parse(transactionTypeUID);

      return TransactionTypeMapper.Map(transactionType);
    }


    public FixedList<ProvidedServiceGroupDto> ProvidedServices() {
      FixedList<RecordingActTypeCategory> list =
                  RecordingActTypeCategory.GetList("TransactionActTypesCategories.List");

      return ProvidedServiceMapper.Map(list);
    }


    public FixedList<TransactionTypeDto> TransactionTypes() {
      FixedList<LRSTransactionType> list = LRSTransactionType.GetList();

      return TransactionTypeMapper.Map(list);
    }


    #endregion Use cases

  }  // class TransactionTypeUseCases

}  // namespace Empiria.Land.Transactions.UseCases
