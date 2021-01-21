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
        return _transaction.Workflow.CanBeDeleted;
      }
    }

    public bool CanEdit {
      get {
        return _transaction.Workflow.CanBeDeleted;
      }
    }

    public bool CanSubmit {
      get {
        return this.CanEditServices && _transaction.Payments.Count > 0;
      }
    }

    public bool CanEditServices {
      get {
        return _transaction.Workflow.CanBeDeleted;
      }
    }

    public bool CanGeneratePaymentOrder {
      get {
        return this.CanEditServices && _transaction.Items.Count > 0;
      }
    }

    public bool CanEditPaymentReceipt {
      get {
        return _transaction.PaymentOrderData.RouteNumber.Length != 0;
      }
    }

    public bool CanUploadDocuments {
      get {
        return false;
      }
    }

    public bool CanEditInstrument {
      get {
        return false;
      }
    }


    public bool CanEditRecordingActs {
      get {
        return false;
      }
    }

    public bool CanEditCertificates {
      get {
        return false;
      }
    }

    #endregion User's action flags


    #region Flags used to control the user interface

    public bool ShowServiceEditor {
      get {
        return true;
      }
    }

    public bool ShowPaymentReceiptEditor {
      get {
        return false;
      }
    }

    public bool ShowUploadDocumentsTab {
      get {
        return false;
      }
    }

    public bool ShowInstrumentRecordingTab {
      get {
        return false;
      }
    }

    public bool ShowCertificatesEmissionTab {
      get {
        return false;
      }
    }

    #endregion Flags used to control the user interface

  }  // class TransactionControlData

}  // namespace Empiria.Land.Transactions
