/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Integration Layer                       *
*  Assembly : Empiria.Land.dll                           Pattern   : Dependency Inversion Interface          *
*  Type     : ITransactionRepository                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Interface that describes a transaction repository.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Providers {

  /// <summary>Interface that describes a transaction repository.</summary>
  public interface ITransactionRepository {

    bool ExistsExternalTransactionNo(string externalTransactionNo);

    int GetLastControlNumber(RecorderOffice recorderOffice);

    FixedList<LRSTransactionItem> GetTransactionItemsList(LRSTransaction transaction);

    FixedList<LRSPayment> GetTransactionPayments(LRSTransaction transaction);

    FixedList<LRSTransaction> GetTransactionsList(string filter, string orderBy, int pageSize);

  }

}
