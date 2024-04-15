/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : LRSExternalTransaction                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data for an external transaction request, that could be integrated into a                *
*             land transaction.                                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;

using Empiria.Land.Registration;
using Empiria.Land.Transactions.Data;
using Empiria.Land.Transactions.Workflow;

namespace Empiria.Land.Transactions {

  /// <summary>Holds data for an external transaction request,
  /// that could be integrated into a land transaction.</summary>
  public class LRSExternalTransaction {

    static private readonly decimal BASE_SALARY_VALUE = ConfigurationData.Get<decimal>("BaseSalaryValue");

    #region Constructors and parsers

    protected LRSExternalTransaction() {
      // Public instance creation not allowed. Instances must be created using a derived class.
    }

    static public LRSExternalTransaction Empty { get; } = new LRSExternalTransaction() {
      IsEmptyInstance = true
    };

    static internal LRSExternalTransaction Parse(JsonObject jsonObject) {
      var transaction = new LRSExternalTransaction();

      transaction.ExternalTransactionNo = jsonObject.Get<string>("ExternalTransactionNo", String.Empty);
      transaction.ExternalTransactionTime = jsonObject.Get<DateTime>("ExternalTransactionTime", ExecutionServer.DateMaxValue);
      transaction.PaymentAmount = jsonObject.Get<decimal>("PaymentAmount", 0m);
      transaction.PaymentReceiptNo = jsonObject.Get<string>("PaymentReceiptNo", String.Empty);
      transaction.RequestedBy = jsonObject.Get<string>("RequestedBy", String.Empty);

      return transaction;
    }

    #endregion Constructors and parsers

    #region Public properties

    /// <summary>The external transaction number or unique identifier.</summary>
    public string ExternalTransactionNo {
      get;
      set;
    } = String.Empty;

    /// <summary>Date and time of the requested transaction in the external system.</summary>
    public DateTime ExternalTransactionTime {
      get;
      set;
    } = ExecutionServer.DateMaxValue;

    public bool IsEmptyInstance {
      get;
      private set;
    }

    /// <summary>The amount payed for the transaction controlled by the external system.</summary>
    public decimal PaymentAmount {
      get;
      set;
    } = 0m;

    /// <summary>The payment receipt number controlled by the external system.
    /// This number should be unique among each external system.</summary>
    public string PaymentReceiptNo {
      get;
      set;
    } = String.Empty;

    /// <summary>The full name of the person or company that request the transaction.</summary>
    public string RequestedBy {
      get;
      set;
    } = String.Empty;

    /// <summary>Represents the transaction type. This property should be overrided by a derived type.</summary>
    protected internal virtual LRSTransactionType TransactionType {
      get;
      set;
    } = LRSTransactionType.Empty;

    /// <summary>Represents the document type. This property should be overrided by a derived type.</summary>
    protected internal virtual LRSDocumentType DocumentType {
      get;
      set;
    } = LRSDocumentType.Empty;

    protected internal virtual Contact Agency {
      get;
      set;
    } = LRSWorkflowRules.InterestedContact;

    #endregion Public properties

    #region Methods

    private void ApplyItemsRuleToTransaction(LRSTransaction transaction) {
      if (this.TransactionType.Id == 699 && this.DocumentType.Id == 708) {
        transaction.Services.Add(RecordingActType.Parse(2284), LRSLawArticle.Parse(874), BASE_SALARY_VALUE * 2);
        transaction.Services.Add(RecordingActType.Parse(2114), LRSLawArticle.Parse(859), BASE_SALARY_VALUE * 2);
      } else if (this.TransactionType.Id == 702 && this.DocumentType.Id == 713) {
        transaction.Services.Add(RecordingActType.Parse(2114), LRSLawArticle.Parse(859), BASE_SALARY_VALUE * 2);
      } else if (this.TransactionType.Id == 702 && this.DocumentType.Id == 710) {
        transaction.Services.Add(RecordingActType.Parse(2111), LRSLawArticle.Parse(859), BASE_SALARY_VALUE * 2);
      } else if (this.TransactionType.Id == 702 && this.DocumentType.Id == 711) {
        transaction.Services.Add(RecordingActType.Parse(2112), LRSLawArticle.Parse(859), BASE_SALARY_VALUE * 2);
      }
    }


    public virtual void AssertIsValid() {
      this.ExternalTransactionNo = EmpiriaString.TrimAll(this.ExternalTransactionNo).ToUpperInvariant();
      this.PaymentReceiptNo = EmpiriaString.TrimAll(this.PaymentReceiptNo);
      this.RequestedBy = EmpiriaString.TrimAll(this.RequestedBy).ToUpperInvariant();

      Assertion.Require(this.ExternalTransactionNo,
        "Requiero el número de trámite CITyS");
      Assertion.Require(this.RequestedBy,
        "Favor de proporcionar el nombre de quien solicita el trámite.");
      Assertion.Require(this.RequestedBy.Length >= 10,
        "El campo de quien solicita el trámite es demasiado pequeño (menos de 10 caracteres).");

      Assertion.Require(this.PaymentReceiptNo,
        "Requiero se proporcione el número de recibo.");
      Assertion.Require(this.PaymentReceiptNo.Length > 3,
        "El número de recibo debería contener más de 3 caractres.");
      Assertion.Require(this.PaymentAmount > 100m,
        "El importe del trámite debería ser mayor a $100.00.");
      Assertion.Require(this.ExternalTransactionTime < DateTime.Now,
        $"La fecha y hora del trámite externo no puede ser mayor a la fecha y hora actual: {this.ExternalTransactionTime}");

      Assertion.Require(!TransactionsDataService.ExistsExternalTransactionNo(this.ExternalTransactionNo),
                       $"Ya tengo registrado otro trámite externo con el mismo número: '{this.ExternalTransactionNo}'");

    }

    protected LRSTransaction CreateLRSTransaction() {
      var transaction = new LRSTransaction(this.TransactionType);

      transaction.DocumentType = this.DocumentType;
      transaction.SetExternalTransaction(this);
      transaction.RequestedBy = this.RequestedBy;
      transaction.Agency = this.Agency;
      transaction.DocumentDescriptor = "CITyS-" + this.ExternalTransactionNo;
      transaction.RecorderOffice = RecorderOffice.Parse(99);

      transaction.Save();

      this.ApplyItemsRuleToTransaction(transaction);

      transaction.PaymentData.SetPayment(this.PaymentReceiptNo, this.PaymentAmount);

      transaction.Workflow.Receive("Recibido automáticamente desde el sistema externo de trámites.");

      return transaction;
    }

    public virtual JsonObject ToJson() {
      var json = new JsonObject();

      json.Add("ExternalTransactionNo", this.ExternalTransactionNo);
      json.Add("ExternalTransactionTime", this.ExternalTransactionTime);
      json.Add("PaymentAmount", this.PaymentAmount);
      json.Add("PaymentReceiptNo", this.PaymentReceiptNo);
      json.Add("RequestedBy", this.RequestedBy);

      return json;
    }

    public override string ToString() {
      return this.ToJson().ToString();
    }

    #endregion Methods

  }  // class LRSExternalTransaction

}  // namespace Empiria.Land.Transactions
