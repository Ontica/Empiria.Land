/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                    Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Control data class                      *
*  Type     : TransactionControlData                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides edition and other control data for a Transaction for the current user.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Transactions.Workflow;

namespace Empiria.Land.Transactions {

  /// <summary>Provides edition and other control data for a Transaction for the current user.</summary>
  public class TransactionControlData {

    public static readonly bool ConnectedToPaymentOrderServices =
                                  ConfigurationData.Get("ConnectedToPaymentOrderServices", false);

    private readonly LRSTransaction _transaction;

    internal TransactionControlData(LRSTransaction transaction) {
      Assertion.Require(transaction, nameof(transaction));

      _transaction = transaction;
    }

    #region User's action flags

    public bool CanCancelPaymentOrder {
      get {
        if (IsSubmitted) {
          return false;
        }
        return (_transaction.PaymentData.HasPaymentOrder && !_transaction.PaymentData.HasPayment);
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

        if (_transaction.PaymentData.HasPaymentOrder || _transaction.PaymentData.HasPayment) {
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

        if (!IsTransactionInStatus(TransactionStatus.Elaboration,
                                   TransactionStatus.Recording,
                                   TransactionStatus.Revision)) {
          return false;
        }

        if (!IsUserInRole("LandRegistrar",
                          "LandCertificator")) {
          return false;
        }

        return true;
      }
    }


    public bool CanEditInstrument {
      get {
        if (!IsReadyForRecording) {
          return false;
        }

        if (_transaction.HasInstrument && _transaction.LandRecord.IsClosed) {
          return false;
        }

        return true;
      }
    }


    public bool IsReadyForRecording {
      get {
        if (!ShowInstrumentRecordingTab) {
          return false;
        }

        if (!IsAssignedToUser) {
          return false;
        }

        if (!IsTransactionInStatus(TransactionStatus.Recording, TransactionStatus.Revision)) {
          return false;
        }

        if (!IsUserInRole("LandRegistrar")) {
          return false;
        }

        return true;
      }
    }


    public bool CanEditPayment {
      get {
        if (IsSubmitted) {
          return false;
        }

        if (!_transaction.PaymentData.HasPaymentOrder) {
          return false;
        }

        return true;
      }
    }


    public bool CanCancelPayment {
      get {
        return _transaction.PaymentData.HasPayment && CanEditPayment;
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

        if (_transaction.PaymentData.IsFeeWaiverApplicable) {
          return false;
        }

        if (!_transaction.HasServices) {
          return false;
        }

        if (!_transaction.Services.HasPayableServices) {
          return false;
        }

        return (!_transaction.PaymentData.HasPaymentOrder);
      }
    }


    public bool CanSubmit {
      get {
        if (IsSubmitted) {
          return false;
        }

        if (!IsUserInRole("ReceptionClerk")) {
          return false;
        }

        if (!_transaction.PaymentData.IsFeeWaiverApplicable &&
            !_transaction.PaymentData.HasPayment) {
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


    public bool CanPrintControlVoucher {
      get {
        if (IsSubmitted) {
          return false;
        }
        if (_transaction.PaymentData.IsFeeWaiverApplicable) {
          return true;
        }
        if (_transaction.PaymentData.HasPaymentOrder &&
            _transaction.PaymentData.PaymentOrder.Issuer != "Empiria.Land") {
          return true;
        }
        return false;
      }
    }


    public bool CanRegisterAntecedent {
      get {
        if (!ShowPreprocessingTab) {
          return false;
        }

        if (!IsTransactionInStatus(TransactionStatus.Digitalization)) {
          return false;
        }

        return IsUserInRole("Digitizer");
      }
    }


    public bool CanUploadDocuments {
      get {
        if (!ShowPreprocessingTab) {
          return false;
        }

        if (!IsTransactionInStatus(TransactionStatus.Digitalization)) {
          return false;
        }

        return IsUserInRole("Digitizer");
      }
    }


    public bool ComesFromAgencyExternalFilingSystem {
      get {
        return !String.IsNullOrWhiteSpace(_transaction.ExternalTransactionNo) && !_transaction.Agency.IsEmptyInstance;
      }
    }


    public bool IsAssignedToUser {
      get {
        if (_transaction.IsEmptyInstance) {
          return false;
        }

        return (_transaction.Workflow.GetCurrentTask().Responsible.Id == ExecutionServer.CurrentUserId);
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

        if (_transaction.Services.Contains(x => x.ServiceType.Name.Contains("Certificate"))) {
          return true;
        }

        return LRSWorkflowRules.IsCertificateIssueCase(_transaction);
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

        return LRSWorkflowRules.IsRecordingDocumentCase(_transaction);
      }
    }


    public bool ShowPaymentReceiptEditor {
      get {
        if (_transaction.PaymentData.HasPayment) {
          return true;
        }

        return _transaction.PaymentData.HasPaymentOrder;
      }
    }


    public bool ShowServiceEditor {
      get {
        if (_transaction.HasServices) {
          return true;
        }

        return !LRSWorkflowRules.IsEmptyServicesTransaction(_transaction);
      }
    }


    public bool ShowPreprocessingTab {
      get {
        if (!IsSubmitted) {
          return false;
        }

        // ToDo: Check if has uploaded documents

        if (!IsUserInRole("Digitizer")) {
          return false;
        }

        return LRSWorkflowRules.IsDigitalizable(_transaction);
      }
    }

    #endregion Flags used to control the user interface

    #region Helper methods

    private bool IsSubmitted {
      get {
        return !IsTransactionInStatus(TransactionStatus.Payment);
      }
    }


    private bool IsTransactionInStatus(params TransactionStatus[] statusList) {
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
