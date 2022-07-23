/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Use cases Layer                         *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Use case interactor class               *
*  Type     : TransactionTypeUseCases                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for transaction types.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Services;

using Empiria.Contacts;

using Empiria.Land.Transactions.Adapters;

using Empiria.Land.Registration;
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

    public FixedList<NamedEntityDto> Agencies() {
      FixedList<Contact> list = LRSTransaction.GetAgenciesList();

      return list.MapToNamedEntityList();
    }


    public FixedList<NamedEntityDto> FilingOffices() {
      var list = new FixedList<RecorderOffice>(new[] { RecorderOffice.Parse(101) });

      return list.MapToNamedEntityList();
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
