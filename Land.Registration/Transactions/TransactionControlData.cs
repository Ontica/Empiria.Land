/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Control data class                      *
*  Type     : TransactionControlData                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides edition and other control data for a Transaction for the current user.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions {

  /// <summary>Provides edition and other control data for a Transaction for the current user.</summary>
  public class TransactionControlData {

    private readonly LRSTransaction _transaction;

    internal TransactionControlData(LRSTransaction transaction) {
      _transaction = transaction;
    }

    #region User's action flags

    public bool CanDelete {
      get {
        return CanEdit;
      }
    }

    public bool CanEdit {
      get {
        if (_transaction.IsNew) {
          return false;
        }

        if (IsSubmitted) {
          return false;
        }

        if (_transaction.HasPaymentOrder || _transaction.HasPayment) {
          return false;
        }

        return true;
      }
    }

    public bool CanEditCertificates {
      get {
        if (!this.ShowCertificatesEmissionTab) {
          return false;
        }

        if (!this.IsAssignedToUser) {
          return false;
        }

        if (!IsTransactionInStatus(LRSTransactionStatus.Elaboration,
                                   LRSTransactionStatus.Recording)) {
          return false;
        }

        if (!IsUserInRole("LRSTransaction.Register",
                          "LRSTransaction.Certificates")) {
          return false;
        }

        return true;
      }
    }

    public bool CanEditInstrument {
      get {
        if (!ShowInstrumentRecordingTab) {
          return false;
        }

        if (!IsAssignedToUser) {
          return false;
        }

        if (!IsTransactionInStatus(LRSTransactionStatus.Recording)) {
          return false;
        }

        if (!IsUserInRole("LRSTransaction.Register")) {
          return false;
        }

        return true;
      }
    }

    public bool CanEditRecordingActs {
      get {
        if (!CanEditInstrument) {
          return false;
        }

        if (!_transaction.HasInstrument) {
          return false;
        }

        // ToDo: if instrument is open and has recording go

        return true;
      }
    }

    public bool CanEditPaymentReceipt {
      get {
        if (IsSubmitted) {
          return false;
        }

        if (!_transaction.HasPaymentOrder) {
          return false;
        }

        if (!IsUserInRole("LRSTransaction.ReceiveTransaction")) {
          return false;
        }

        return true;
      }
    }

    public bool CanEditServices {
      get {
        if (!ShowServiceEditor) {
          return false;
        }

        if (!CanEdit) {
          return false;
        }

        return true;
      }
    }

    public bool CanGeneratePaymentOrder {
      get {
        if (!CanEditServices) {
          return false;
        }

        if (_transaction.IsFeeWaiverApplicable) {
          return false;
        }

        return (_transaction.HasServices && !_transaction.HasPaymentOrder);
      }
    }

    public bool CanSubmit {
      get {
        if (IsSubmitted) {
          return false;
        }

        if (!IsUserInRole("LRSTransaction.ReceiveTransaction")) {
          return false;
        }

        if (!LRSWorkflowRules.IsEmptyItemsTransaction(_transaction) &&
            !_transaction.HasPaymentOrder) {
          return false;
        }

        return true;
      }
    }

    public bool CanUploadDocuments {
      get {
        if (!ShowUploadDocumentsTab) {
          return false;
        }

        if (!IsTransactionInStatus(LRSTransactionStatus.Digitalization)) {
          return false;
        }

        return IsUserInRole("LRSTransaction.Digitalizer");
      }
    }

    #endregion User's action flags


    #region Flags used to control the user interface

    public bool ShowCertificatesEmissionTab {
      get {
        if (!IsSubmitted) {
          return false;
        }

        if (_transaction.HasCertificates) {
          return true;
        }

        return LRSWorkflowRules.IsCertificateIssueCase(_transaction.TransactionType,
                                                       _transaction.DocumentType);
      }
    }

    public bool ShowInstrumentRecordingTab {
      get {
        if (!IsSubmitted) {
          return false;
        }

        if (_transaction.HasInstrument) {
          return true;
        }

        return LRSWorkflowRules.IsRecordingDocumentCase(_transaction.TransactionType,
                                                        _transaction.DocumentType);
      }
    }

    public bool ShowPaymentReceiptEditor {
      get {
        if (_transaction.HasPayment) {
          return true;
        }

        return false;
      }
    }

    public bool ShowServiceEditor {
      get {
        if (_transaction.HasServices) {
          return true;
        }

        return !LRSWorkflowRules.IsEmptyItemsTransaction(_transaction);
      }
    }

    public bool ShowUploadDocumentsTab {
      get {
        if (!IsSubmitted) {
          return false;
        }

        // ToDo: Check if has uploaded documents

        return LRSWorkflowRules.IsDigitalizable(_transaction.TransactionType,
                                                _transaction.DocumentType);
      }
    }

    #endregion Flags used to control the user interface

    #region Helper methods

    private bool IsAssignedToUser {
      get {
        return (_transaction.Workflow.GetCurrentTask().Responsible.Id == ExecutionServer.CurrentUserId);
      }
    }

    private bool IsSubmitted {
      get {
        return !IsTransactionInStatus(LRSTransactionStatus.Payment);
      }
    }

    private bool IsTransactionInStatus(params LRSTransactionStatus[] statusList) {
      foreach (var status in statusList) {
        if (_transaction.Workflow.CurrentStatus == status) {
          return true;
        }
      }
      return false;
    }

    private bool IsUserInRole(params string[] rolesList) {
      foreach (var role in rolesList) {
        if (ExecutionServer.CurrentPrincipal.IsInRole(role)) {
          return true;
        }
      }
      return false;
    }

    #endregion Helper methods

  }  // class TransactionControlData

}  // namespace Empiria.Land.Transactions
