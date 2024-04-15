/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Payments                       Component : Integration Layer                       *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : External Service Connector              *
*  Type     : PaymentServicesConnector                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Connector used to gain access to transaction payment order services.                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Threading.Tasks;

using Empiria.Reflection;

using Empiria.Land.Integration.PaymentServices;

using Empiria.Land.Media;
using Empiria.Land.Media.Adapters;

using Empiria.Land.Transactions.Adapters;

namespace Empiria.Land.Transactions.Payments.Providers {

  /// <summary>Connector used to gain access to transaction payment order services.</summary>
  internal class PaymentServicesConnector {

    private readonly bool ConnectedToPaymentOrderServices;

    internal PaymentServicesConnector() {
      ConnectedToPaymentOrderServices = TransactionControlData.ConnectedToPaymentOrderServices;
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
      Assertion.Require(transaction, "transaction");

      if (!ConnectedToPaymentOrderServices) {
        return Task.FromResult(GetDisconnectedPaymentOrder(transaction));
      }

      var payableServices = transaction.Services.PayableServices;

      if (HasServicesThanCanNotBeAutoCalculated(payableServices)) {
        return Task.FromResult(GetDisconnectedPaymentOrder(transaction));
      }

      PaymentOrderRequestDto paymentOrderRequest = MapToPaymentOrderRequest(transaction, payableServices);

      IPaymentService externalService = GetPaymentOrderService();

      return externalService.GeneratePaymentOrderFor(paymentOrderRequest);
    }


    internal Task<string> GetPaymentStatus(PaymentOrder paymentOrder) {
      if (!ConnectedToPaymentOrderServices) {
        return Task.FromResult("Desconocido");
      }

      Assertion.Require(paymentOrder, "paymentOrder");

      IPaymentService externalService = GetPaymentOrderService();

      return externalService.GetPaymentStatus(paymentOrder);
    }


    #endregion Services

    #region Mapper methods

    private Task<decimal> CalculateFixedFee(string financialConceptCode, decimal quantity) {
      if (!ConnectedToPaymentOrderServices) {
        return Task.FromResult(0m);
      }

      IPaymentService externalService = GetPaymentOrderService();

      return externalService.CalculateFixedFee(financialConceptCode, quantity);
    }


    private Task<decimal> CalculateVariableFee(string financialConceptCode, decimal taxableBase) {
      if (!ConnectedToPaymentOrderServices) {
        return Task.FromResult(0m);
      }

      IPaymentService externalService = GetPaymentOrderService();

      return externalService.CalculateVariableFee(financialConceptCode, taxableBase);
    }


    private static IPaymentOrder GetDisconnectedPaymentOrder(LRSTransaction transaction) {
      var po = new Integration.PaymentServices.PaymentOrderDto {
        UID = Guid.NewGuid().ToString(),
        Issuer = "Empiria.Land",
        Version = "5.0",
        IssueTime = DateTime.Now,
        DueDate = DateTime.Now.AddDays(15),
        Status = "Pendiente de pago"
      };

      var builder = new LandMediaBuilder();

      var mediaDto = builder.GetMediaDto(LandMediaContent.TransactionPaymentOrder, transaction.UID);

      po.AddAttribute("url", mediaDto.Url);
      po.AddAttribute("mediaType", mediaDto.MediaType);

      return po;
    }


    private PaymentOrderRequestDto MapToPaymentOrderRequest(LRSTransaction transaction,
                                                            FixedList<LRSTransactionService> items) {
      return new PaymentOrderRequestDto {

        BaseTransactionUID = transaction.UID,
        RequestedBy = transaction.ExtensionData.BillTo.Length != 0 ? transaction.ExtensionData.BillTo : transaction.RequestedBy,
        Address = String.Empty,
        RFC = transaction.ExtensionData.RFC,
        Concepts = MapToPaymentOrderRequestConceptsArray(items)
      };
    }


    private PaymentOrderRequestConceptDto[] MapToPaymentOrderRequestConceptsArray(FixedList<LRSTransactionService> items) {
      var conceptsArray = new PaymentOrderRequestConceptDto[items.Count];

      for (int i = 0; i < items.Count; i++) {
        conceptsArray[i] = MapToPaymentOrderRequestConceptDto(items[i]);
      }

      return conceptsArray;
    }


    private PaymentOrderRequestConceptDto MapToPaymentOrderRequestConceptDto(LRSTransactionService item) {
      var concept = new PaymentOrderRequestConceptDto();

      concept.ConceptUID = item.TreasuryCode.FinancialConceptCode;
      concept.Quantity = item.Quantity.Amount;
      concept.TaxableBase = item.OperationValue.Amount;
      concept.Total = item.Fee.Total;

      return concept;
    }

    #endregion Mapper methods

    #region Helper methods


    static private bool HasServicesThanCanNotBeAutoCalculated(FixedList<LRSTransactionService> payableItems) {
      return payableItems.CountAll(x => !x.TreasuryCode.Autocalculated) > 0;
    }


    static private IPaymentService GetPaymentOrderService() {
      Type type = ObjectFactory.GetType("SIT.Finanzas.Connector",
                                        "Empiria.Zacatecas.Integration.SITFinanzasConnector.PaymentService");

      string baseAddress = ConfigurationData.GetString("PaymentOrderService.BaseAddress");

      return (IPaymentService) ObjectFactory.CreateObject(type,
                                                          new Type[] { typeof(String) },
                                                          new object[] { baseAddress });
    }

    #endregion Helper methods

  }  // class PaymentServicesConnector

}  // namespace Empiria.Land.Transactions.Payments.Providers
