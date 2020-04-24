/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Integration Services                       Component : Empiria Land Transaction Services       *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Service provider                        *
*  Type     : LandFilingTransactionProvider              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides land transaction services through external electronic filing requests.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.OnePoint.EFiling;
using Empiria.OnePoint.EPayments;

using Empiria.Land.Certification;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;


namespace Empiria.Land.Integration {

  /// <summary>Provides land transaction services through external electronic filing requests.</summary>
  public class LandFilingTransactionProvider: IFilingTransactionProvider {

    #region Fields

    static private readonly decimal BASE_SALARY_VALUE = ConfigurationData.Get<decimal>("BaseSalaryValue");

    private static readonly string PRINT_SERVICES_SERVER_BASE_ADDRESS =
                                        ConfigurationData.Get<string>("PrintServicesServerBaseAddress");

    #endregion Fields

    #region Constructors and parsers

    public LandFilingTransactionProvider() {
      // Public instance creation not allowed. Instances must be created using a derived class.
    }

    #endregion Constructors and parsers


    #region Methods


    public IPayable CreateTransaction(EFilingRequest filingRequest) {
      Assertion.AssertObject(filingRequest, "filingRequest");

      Procedure procedure = filingRequest.Procedure;

      var transactionType = LRSTransactionType.Parse(procedure.TransactionTypeId);

      var transaction = new LRSTransaction(transactionType);

      transaction.DocumentType = LRSDocumentType.Parse(procedure.DocumentTypeId);
      transaction.RequestedBy = filingRequest.RequestedBy.name;
      transaction.Agency = filingRequest.Agency;
      transaction.RecorderOffice = RecorderOffice.Parse(procedure.AuthorityOfficeId);
      transaction.ExternalTransactionNo = filingRequest.UID;

      if (filingRequest.RequestedBy.rfc.Length != 0) {
        transaction.ExtensionData.RFC = filingRequest.RequestedBy.rfc;
      }
      if (filingRequest.RequestedBy.email.Length != 0) {
        transaction.ExtensionData.SendTo = new Empiria.Messaging.SendTo(filingRequest.RequestedBy.email);
      }

      transaction.Save();

      return transaction;
    }


    public void EventProcessed(string transactionUID, string eventName) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      Assertion.AssertObject(transaction, "transaction");

      if (transaction.Workflow.IsReadyForDeliveryOrReturn) {
        transaction.Workflow.DeliveredElectronicallyToAgency();
      }
    }


    public FixedList<EFilingDocumentDTO> GetOutputDocuments(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      if (!transaction.Workflow.IsFinished) {
        return new FixedList<EFilingDocumentDTO>();
      }

      var list = new List<EFilingDocumentDTO>();

      var document = transaction.Document;

      if (!document.IsEmptyInstance && document.Security.Signed()) {
        list.Add(MapToEFilingDocumentDTO(document));
      }

      var certificates = transaction.GetIssuedCertificates();

      foreach (var certificate in certificates) {
        if (certificate.Signed()) {
          list.Add(MapToEFilingDocumentDTO(certificate));
        }
      }

      return list.ToFixedList();
    }


    public IFilingTransaction GetTransaction(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      return ConvertToDTOInterface(transaction);
    }


    public IPayable GetTransactionAsPayable(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      Assertion.AssertObject(transaction, "transaction");

      return transaction;
    }


    public IFilingTransaction SetPayment(string transactionUID, string receiptNo) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      transaction.AddPayment(receiptNo, transaction.Items.TotalFee.Total);

      return ConvertToDTOInterface(transaction);
    }



    public IFilingTransaction SetPaymentOrder(IPayable transaction,
                                              OnePoint.EPayments.PaymentOrderDTO paymentOrderData) {
      Assertion.AssertObject(transaction, "transaction");
      Assertion.AssertObject(paymentOrderData, "paymentOrderData");

      transaction.SetPaymentOrderData(paymentOrderData);

      return ConvertToDTOInterface((LRSTransaction) transaction);
    }


    public IFilingTransaction SubmitTransaction(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      transaction.Workflow.Receive("Ingresado automáticamente desde el sistema de notarías y grandes usuarios.");

      return ConvertToDTOInterface(transaction);
    }


    public OnePoint.EPayments.PaymentOrderDTO TryGetPaymentOrderData(string transactionUID) {
      Assertion.AssertObject(transactionUID, "transactionUID");

      var transaction = LRSTransaction.TryParse(transactionUID, true);

      return transaction.TryGetPaymentOrderData();
    }


    public IFilingTransaction UpdateTransaction(EFilingRequest filingRequest) {
      Assertion.AssertObject(filingRequest, "filingRequest");
      Assertion.AssertObject(filingRequest.TransactionUID, "filingRequest.TransactionUID");

      var transaction = LRSTransaction.TryParse(filingRequest.TransactionUID, true);

      transaction.RequestedBy = filingRequest.RequestedBy.name;

      if (filingRequest.RequestedBy.rfc.Length != 0) {
        transaction.ExtensionData.RFC = filingRequest.RequestedBy.rfc;
      } else {
        transaction.ExtensionData.RFC = String.Empty;
      }

      if (filingRequest.RequestedBy.email.Length != 0) {
        transaction.ExtensionData.SendTo = new Empiria.Messaging.SendTo(filingRequest.RequestedBy.email);
      } else {
        transaction.ExtensionData.SendTo = Empiria.Messaging.SendTo.Empty;
      }

      transaction.Save();

      return ConvertToDTOInterface(transaction);
    }


    #endregion Methods


    #region Utility methods


    static private IFilingTransaction ConvertToDTOInterface(LRSTransaction transaction) {
      return new FilingTransactionDTO(transaction);
    }


    static private EFilingDocumentDTO MapToEFilingDocumentDTO(Certificate certificate) {
      return new EFilingDocumentDTO() {
        uid = certificate.UID,
        type = certificate.CertificateType.Name,
        typeName = $"Certificado de {certificate.CertificateType.DisplayName}",
        name = $"Certificado {certificate.UID} del trámite {certificate.Transaction.UID}",
        contentType = "text/html",
        uri = $"{PRINT_SERVICES_SERVER_BASE_ADDRESS}/certificate.aspx?uid={certificate.UID}&" +
              $"externalTransaction={certificate.Transaction.ExternalTransactionNo}"
      };
    }


    static private EFilingDocumentDTO MapToEFilingDocumentDTO(RecordingDocument document) {
      return new EFilingDocumentDTO() {
        uid = document.UID,
        type = document.Subtype.Name,
        typeName = "Sello registral",
        name = $"Sello registral {document.UID} del trámite {document.GetTransaction().UID}.",
        contentType = "text/html",
        uri = $"{PRINT_SERVICES_SERVER_BASE_ADDRESS}/recording.seal.aspx?uid={document.UID}&" +
              $"externalTransaction={document.GetTransaction().ExternalTransactionNo}"
      };
    }


    #endregion Utility methods


    #region Inner class FilingTransactionDTO

    private class FilingTransactionDTO : IFilingTransaction {

      internal FilingTransactionDTO(LRSTransaction transaction) {
        this.Id = transaction.Id;
        this.UID = transaction.UID;
        this.PresentationTime = transaction.PresentationTime;
        this.StatusName = transaction.Workflow.CurrentStatusName;
      }

      public int Id {
        get;
        private set;
      }

      public string UID {
        get;
        private set;
      }

      public string StatusName {
        get;
        private set;
      }

      public DateTime PresentationTime {
        get;
        private set;
      }

    }

    #endregion Inner class FilingTransactionDTO


  }  // class LandFilingTransactionProvider

}  // namespace Empiria.Land.Integration
