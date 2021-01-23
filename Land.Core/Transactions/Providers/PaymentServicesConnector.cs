/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Integration Layer                       *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : External Service Connector              *
*  Type     : PaymentServicesConnector                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Connector used to gain access to transaction payment order services.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Reflection;

using Empiria.Land.Registration.Transactions;

using Empiria.Land.Integration.PaymentServices;

namespace Empiria.Land.Transactions.Providers {

  /// <summary>Connector used to gain access to transaction payment order services.</summary>
  internal class PaymentServicesConnector {

    private readonly IPaymentService _externalService;

    internal PaymentServicesConnector() {
      _externalService = GetPaymentOrderService();
    }

    #region Services

    internal Task<decimal> CalculateFixedFee(string serviceUID, decimal quantity) {
      return _externalService.CalculateFixedFee(serviceUID, quantity);
    }


    internal Task<decimal> CalculateVariableFee(string serviceUID, decimal taxableBase) {
      return _externalService.CalculateVariableFee(serviceUID, taxableBase);
    }


    internal Task<IPaymentOrder> GeneratePaymentOrder(LRSTransaction transaction) {
      PaymentOrderRequestDto paymentOrderRequest = MapToPaymentOrderRequest(transaction);

      return _externalService.GeneratePaymentOrderFor(paymentOrderRequest);
    }

    #endregion Services

    #region Mapper methods

    private PaymentOrderRequestDto MapToPaymentOrderRequest(LRSTransaction transaction) {
      var po = new PaymentOrderRequestDto();

      po.BaseTransactionUID = transaction.UID;
      po.RequestedBy = transaction.RequestedBy;
      po.Address = String.Empty;
      po.RFC = transaction.ExtensionData.RFC;
      if (po.RFC.Length == 0) {
        po.RFC = "XAXX010101000";
      }

      po.Concepts = MapToPaymentOrderRequestConceptsArray(transaction.Items);

      return po;
    }


    private PaymentOrderRequestConceptDto[] MapToPaymentOrderRequestConceptsArray(LRSTransactionItemList items) {
      var conceptsArray = new PaymentOrderRequestConceptDto[items.Count];

      for (int i = 0; i < items.Count; i++) {
        conceptsArray[i] = MapToPaymentOrderRequestConceptDto(items[i]);
      }

      return conceptsArray;
    }


    private PaymentOrderRequestConceptDto MapToPaymentOrderRequestConceptDto(LRSTransactionItem item) {
      var concept = new PaymentOrderRequestConceptDto();

      concept.ConceptUID = item.TreasuryCode.FinancialConceptCode;
      concept.Quantity = item.Quantity.Amount;
      concept.TaxableBase = item.OperationValue.Amount;
      concept.Total = item.Fee.Total;

      return concept;
    }

    #endregion Mapper methods

    #region Helper methods

    static private IPaymentService GetPaymentOrderService() {
      Type type = ObjectFactory.GetType("Empiria.Land.Integration",
                                        "Empiria.Land.Integration.PaymentServices.FakePaymentService");

      return (IPaymentService) ObjectFactory.CreateObject(type);
    }

    #endregion Helper methods

  }  // class PaymentServicesConnector

}  // namespace Empiria.Land.Transactions.Providers
