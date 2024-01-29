/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Filing                                       Component : Filing Workflow                       *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Micro-workflow                        *
*  Type     : LRSWorkflowRules                             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Micro-workflow rules set for the Land Registration System.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Micro-workflow rules set for the Land Registration System.</summary>
  static public class LRSWorkflowRules {

    #region Properties

    static public Contact InterestedContact {
      get {
        return Person.Parse(-6);
      }
    }

    #endregion Properties

    #region Methods

    static internal TransactionStatus GetNextStatusAfterReceive(LRSTransaction transaction) {
      if (ExecutionServer.LicenseName == "Zacatecas") {
        return TransactionStatus.Control;
      }

      if (LRSWorkflowRules.IsRecorderOfficerCase(transaction.TransactionType,
                                                 transaction.RecorderOffice)) {
        return TransactionStatus.Revision;

      } else if (LRSWorkflowRules.IsJuridicCase(transaction.TransactionType,
                                                transaction.DocumentType)) {
        return TransactionStatus.Juridic;

      } else if (LRSWorkflowRules.IsRecordingDocumentCase(transaction.TransactionType,
                                                          transaction.DocumentType)) {
        return TransactionStatus.Recording;

      } else if (LRSWorkflowRules.IsCertificateIssueCase(transaction.TransactionType,
                                                         transaction.DocumentType)) {
        return TransactionStatus.Elaboration;

      } else {
        return TransactionStatus.Control;
      }
    }


    private static bool IsRecorderOfficerCase(LRSTransactionType transactionType, RecorderOffice office) {
      if (transactionType.Id == 706 && office.Id == 147) {
        return true;
      }
      return false;
    }


    private static bool IsJuridicCase(LRSTransactionType type, LRSDocumentType docType) {
      if (type.Id == 706 && docType.Id == 734) {
        return true;
      }
      return false;
    }


    static public bool IsArchivable(LRSTransactionType type, LRSDocumentType docType) {
      if (docType.Id == 761) {
        return true;
      }
      return false;
    }


    static public bool IsCertificateIssueCase(LRSTransactionType type, LRSDocumentType docType) {
      return type.Id == 701 || type.Id == 704;
    }


    static public bool IsEmptyServicesTransaction(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 704 ||
          transaction.TransactionType.Id == 705) {
        return true;
      }

      return false;
    }


    static public bool IsReadyForReentry(LRSTransaction transaction) {
      var user = ExecutionServer.CurrentPrincipal;

      return ((transaction.Workflow.CurrentStatus == TransactionStatus.Returned ||
              (transaction.Workflow.CurrentStatus == TransactionStatus.Delivered ||
               transaction.Workflow.CurrentStatus == TransactionStatus.Archived)) &&
               user.IsInRole("Supervisor"));
    }


    static public bool IsReadyForGenerateImagingControlID(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance || transaction.Document.IsEmptyInstance) {
        return false;
      }
      if (!ExecutionServer.CurrentPrincipal.IsInRole("Digitizer")) {
        return false;
      }
      if (transaction.Document.Imaging.ImagingControlID.Length != 0) {
        return false;
      }
      if (transaction.Document.RecordingActs.Count == 0) {
        return false;
      }
      if (!transaction.Document.IsClosed) {
        return false;
      }
      if (!IsDigitalizable(transaction.TransactionType, transaction.DocumentType)) {
        return false;
      }
      if (transaction.Workflow.CurrentStatus == TransactionStatus.Digitalization ||
          transaction.Workflow.CurrentStatus == TransactionStatus.ToDeliver ||
          transaction.Workflow.CurrentStatus == TransactionStatus.Delivered) {
        return true;
      }

      return false;
    }


    static public bool IsRecordingDocumentCase(LRSTransactionType type, LRSDocumentType docType) {
      if (type.Id == 700 || type.Id == 704 || type.Id == 705) {
        return true;
      }

      return false;
    }


    public static bool IsDigitalizable(LRSTransactionType type, LRSDocumentType docType) {
      return IsRecordingDocumentCase(type, docType);
    }


    static internal bool UserCanEditDocument(RecordingDocument document) {
      if (!(ExecutionServer.CurrentPrincipal.IsInRole("LandRegistrar"))) {
        return false;
      }
      if (document.IsHistoricDocument) {
        return true;
      }
      var transaction = document.GetTransaction();

      Assertion.Require(!transaction.IsEmptyInstance,
                        "Transaction can't be the empty instance, because the document is not historic.");

      if (!(transaction.Workflow.CurrentStatus == TransactionStatus.Recording ||
            transaction.Workflow.CurrentStatus == TransactionStatus.Elaboration ||
            transaction.Workflow.CurrentStatus == TransactionStatus.Juridic)) {
        return false;
      }

      if (transaction.Workflow.GetCurrentTask().Responsible.Id == ExecutionServer.CurrentUserId) {
        return true;
      } else {
        return false;
      }
    }


    static public bool IsStatusOfficeWork(TransactionStatus currentStatus) {
      if (currentStatus == TransactionStatus.Payment || currentStatus == TransactionStatus.ToDeliver ||
          currentStatus == TransactionStatus.ToReturn || currentStatus == TransactionStatus.Delivered ||
          currentStatus == TransactionStatus.Returned || currentStatus == TransactionStatus.Archived) {
        return false;
      }
      return true;
    }


    static public string ValidateStatusChange(LRSTransaction transaction, TransactionStatus nextStatus) {
      if (nextStatus == TransactionStatus.Received && transaction.PaymentData.Payments.Count == 0) {
        return "Este trámite todavía no tiene registrada una boleta de pago.";
      }

      if (!IsRecordingDocumentCase(transaction.TransactionType, transaction.DocumentType)) {
        return String.Empty;
      }

      if (transaction.TransactionType.Id == 704) {
        return String.Empty;
      }

      if (nextStatus == TransactionStatus.Revision || nextStatus == TransactionStatus.OnSign ||
          nextStatus == TransactionStatus.Archived || nextStatus == TransactionStatus.ToDeliver) {
        if (transaction.Document.IsEmptyInstance) {
          return "Necesito primero se ingrese la información del documento a inscribir.";
        }
      }

      return String.Empty;
    }


    #endregion Methods

  }  // class LRSWorkflowRules

}  // namespace Empiria.Land.Registration.Transactions
