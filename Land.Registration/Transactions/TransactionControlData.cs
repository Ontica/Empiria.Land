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

    public static readonly bool ConnectedToPaymentOrderServices =
                                  ConfigurationData.Get("ConnectedToPaymentOrderServices", false);

    private readonly LRSTransaction _transaction;

    internal TransactionControlData(LRSTransaction transaction) {
      _transaction = transaction;
    }

    #region User's action flags

    public bool CanCancelPaymentOrder {
      get {
        if (IsSubmitted) {
          return false;
        }
        return (_transaction.HasPaymentOrder && !_transaction.HasPayment);
      }
    }

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

        if (!IsUserInRole("Land.Registrar",
                          "Land.Certificator")) {
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

        if (!IsUserInRole("Land.Registrar")) {
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


    public bool CanEditPayment {
      get {
        if (IsSubmitted) {
          return false;
        }

        if (!_transaction.HasPaymentOrder) {
          return false;
        }

        return true;
      }
    }


    public bool CanCancelPayment {
      get {
        return _transaction.HasPayment && CanEditPayment;
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

        if (!_transaction.HasServices) {
          return false;
        }

        if (!_transaction.Items.ContainsPayableItems()) {
          return false;
        }

        return (!_transaction.HasPaymentOrder);
      }
    }


    public bool CanSubmit {
      get {
        if (IsSubmitted) {
          return false;
        }

        if (!IsUserInRole("Land.ReceptionClerk")) {
          return false;
        }

        if (!_transaction.IsFeeWaiverApplicable &&
            !_transaction.HasPayment) {
          return false;
        }

        return true;
      }
    }

    public bool CanPrintSubmissionReceipt {
      get {
        if (!IsSubmitted) {
          return false;
        }
        return true;
      }
    }


    public bool CanRegisterAntecedent {
      get {
        if (!ShowPreprocessingTab) {
          return false;
        }

        if (!IsTransactionInStatus(LRSTransactionStatus.Digitalization)) {
          return false;
        }

        return IsUserInRole("Land.Digitizer");
      }
    }


    public bool CanUploadDocuments {
      get {
        if (!ShowPreprocessingTab) {
          return false;
        }

        if (!IsTransactionInStatus(LRSTransactionStatus.Digitalization)) {
          return false;
        }

        return IsUserInRole("Land.Digitizer");
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

        return _transaction.HasPaymentOrder;
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

    public bool ShowPreprocessingTab {
      get {
        if (!IsSubmitted) {
          return false;
        }

        // ToDo: Check if has uploaded documents

        return false;

        //return LRSWorkflowRules.IsDigitalizable(_transaction.TransactionType,
        //                                        _transaction.DocumentType);
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
