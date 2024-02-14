/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Service provider                      *
*  Type     : ElectronicDeliveryService                    License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Service that provides transation's electronic delivery.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.WebApi {

  /// <summary>Service that provides transation's electronic delivery.</summary>
  internal class ElectronicDeliveryService {

    internal void DeliverTransaction(string transactionUID, string messageUID) {
      var transaction = LRSTransaction.TryParse(transactionUID);

      transaction.Workflow.DeliverElectronicallyToRequester(messageUID);
    }

  }  // class ElectronicDeliveryService

}  // namespace Empiria.Land.WebApi
