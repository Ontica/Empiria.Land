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
using Empiria.Land.Transactions.Adapters;
using Empiria.Land.Media.Adapters;
using Empiria.Land.Media;

namespace Empiria.Land.Transactions.Providers {

  /// <summary>Connector used to gain access to transaction payment order services.</summary>
  internal class PaymentServicesConnector {

    private readonly bool ConnectedToPaymentOrderServices;

    private readonly IPaymentService _externalService;

    internal PaymentServicesConnector() {
      ConnectedToPaymentOrderServices = TransactionControlData.ConnectedToPaymentOrderServices;

      _externalService = GetPaymentOrderService();
    }

    #region Services

    internal Task<decimal> CalculateFee(RequestedServiceFields requestedServiceFields) {
      requestedServiceFields.AssertValid();

      var concept = LRSLawArticle.Parse(requestedServiceFields.FeeConceptUID);

      if (String.IsNullOrWhiteSpace(concept.FinancialConceptCode)) {
        return Task.FromResult(0m);
      }

      if (requestedServiceFields.TaxableBase != decimal.Zero) {
        return this.CalculateVariableFee(concept.FinancialConceptCode,
                                         requestedServiceFields.TaxableBase);
      } else {
        return this.CalculateFixedFee(concept.FinancialConceptCode,
                                      requestedServiceFields.Quantity);
      }
    }


    internal Task<IPaymentOrder> GeneratePaymentOrder(LRSTransaction transaction) {
      Assertion.AssertObject(transaction, "transaction");

      if (!ConnectedToPaymentOrderServices) {
        return Task.FromResult(GetDisconnectedPaymentOrder(transaction));
      }

      var payableItems = transaction.Items.PayableItems;

      PaymentOrderRequestDto paymentOrderRequest = MapToPaymentOrderRequest(transaction, payableItems);

      return _externalService.GeneratePaymentOrderFor(paymentOrderRequest);
    }


    internal Task<string> GetPaymentStatus(PaymentOrder paymentOrder) {
      if (!ConnectedToPaymentOrderServices) {
        return Task.FromResult("Desconocido");
      }

      Assertion.AssertObject(paymentOrder, "paymentOrder");

      return _externalService.GetPaymentStatus(paymentOrder);
    }


    #endregion Services

    #region Mapper methods

    private Task<decimal> CalculateFixedFee(string financialConceptCode, decimal quantity) {
      if (!ConnectedToPaymentOrderServices) {
        return Task.FromResult(0m);
      }

      return _externalService.CalculateFixedFee(financialConceptCode, quantity);
    }


    private Task<decimal> CalculateVariableFee(string financialConceptCode, decimal taxableBase) {
      if (!ConnectedToPaymentOrderServices) {
        return Task.FromResult(0m);
      }

      return _externalService.CalculateVariableFee(financialConceptCode, taxableBase);
    }


    private static IPaymentOrder GetDisconnectedPaymentOrder(LRSTransaction transaction) {
      var po = new Integration.PaymentServices.PaymentOrderDto {
        UID = Guid.NewGuid().ToString(),
        Issuer = "Empiria.Land",
        Version = "5.0",
        IssueTime = DateTime.Now,
        DueDate = DateTime.Now.AddDays(15)
      };

      var builder = new LandMediaBuilder(LandMediaContent.TransactionPaymentOrder, transaction);

      var mediaDto = builder.GetMediaDto();

      po.AddAttribute("url", mediaDto.Url);
      po.AddAttribute("mediaType", mediaDto.MediaType);

      return po;
    }


    private PaymentOrderRequestDto MapToPaymentOrderRequest(LRSTransaction transaction,
                                                            FixedList<LRSTransactionItem> items) {
      var po = new PaymentOrderRequestDto();

      po.BaseTransactionUID = transaction.UID;
      po.RequestedBy = transaction.RequestedBy;
      po.Address = String.Empty;
      po.RFC = transaction.ExtensionData.RFC;
      if (po.RFC.Length == 0) {
        po.RFC = "XAXX010101000";
      }

      po.Concepts = MapToPaymentOrderRequestConceptsArray(items);

      return po;
    }


    private PaymentOrderRequestConceptDto[] MapToPaymentOrderRequestConceptsArray(FixedList<LRSTransactionItem> items) {
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
      Type type = ObjectFactory.GetType("SIT.Finanzas.Connector",
                                        "Empiria.Zacatecas.Integration.SITFinanzasConnector.PaymentService");

      return (IPaymentService) ObjectFactory.CreateObject(type);
    }

    #endregion Helper methods

  }  // class PaymentServicesConnector

}  // namespace Empiria.Land.Transactions.Providers
