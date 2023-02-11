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

    static internal Contact InterestedContact {
      get {
        return Person.Parse(-6);
      }
    }

    #endregion Properties

    #region Methods

    static internal LRSTransactionStatus GetNextStatusAfterReceive(LRSTransaction transaction) {
      if (ExecutionServer.LicenseName == "Zacatecas") {
        return LRSTransactionStatus.Control;
      }

      if (LRSWorkflowRules.IsRecorderOfficerCase(transaction.TransactionType,
                                                 transaction.RecorderOffice)) {
        return LRSTransactionStatus.Revision;

      } else if (LRSWorkflowRules.IsJuridicCase(transaction.TransactionType,
                                                transaction.DocumentType)) {
        return LRSTransactionStatus.Juridic;

      } else if (LRSWorkflowRules.IsRecordingDocumentCase(transaction.TransactionType,
                                                          transaction.DocumentType)) {
        return LRSTransactionStatus.Recording;

      } else if (LRSWorkflowRules.IsCertificateIssueCase(transaction.TransactionType,
                                                         transaction.DocumentType)) {
        return LRSTransactionStatus.Elaboration;

      } else {
        return LRSTransactionStatus.Control;
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


    static public string GetStatusName(LRSTransactionStatus status) {
      switch (status) {
        case LRSTransactionStatus.Payment:
          return "Precalificación";
        case LRSTransactionStatus.Received:
          return "Trámite recibido";
        case LRSTransactionStatus.Reentry:
          return "Trámite reingresado";
        case LRSTransactionStatus.Process:
          return "En mesas de trabajo";
        case LRSTransactionStatus.Control:
          return "En mesa de control";
        case LRSTransactionStatus.Qualification:
          return "En calificación";
        case LRSTransactionStatus.Recording:
          return "En registro en libros";
        case LRSTransactionStatus.Elaboration:
          return "En elaboración";
        case LRSTransactionStatus.Revision:
          return "En revisión";
        case LRSTransactionStatus.Juridic:
          return "En área jurídica";
        case LRSTransactionStatus.OnSign:
          return "En firma";
        case LRSTransactionStatus.Digitalization:
          return "En digitalización y resguardo";
        case LRSTransactionStatus.ToDeliver:
          return "En ventanilla de entregas";
        case LRSTransactionStatus.Delivered:
          return "Entregado al interesado";
        case LRSTransactionStatus.ToReturn:
          return "En ventanilla de devoluciones";
        case LRSTransactionStatus.Returned:
          return "Devuelto al interesado";
        case LRSTransactionStatus.Deleted:
          return "Trámite eliminado";
        case LRSTransactionStatus.Archived:
          return "Archivado/Concluido";
        default:
          return "No determinado";
      }
    }

    static public bool IsNotSignable(LRSTransactionType type, LRSDocumentType docType) {
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


    static public bool IsEmptyItemsTransaction(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 704 ||
          transaction.TransactionType.Id == 705) {
        return true;
      }

      return false;
    }


    static public bool IsReadyForReentry(LRSTransaction transaction) {
      var user = ExecutionServer.CurrentPrincipal;

      return ((transaction.Workflow.CurrentStatus == LRSTransactionStatus.Returned ||
              (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Delivered ||
               transaction.Workflow.CurrentStatus == LRSTransactionStatus.Archived)) &&
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
      if (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Digitalization ||
          transaction.Workflow.CurrentStatus == LRSTransactionStatus.ToDeliver ||
          transaction.Workflow.CurrentStatus == LRSTransactionStatus.Delivered) {
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

      if (!(transaction.Workflow.CurrentStatus == LRSTransactionStatus.Recording ||
            transaction.Workflow.CurrentStatus == LRSTransactionStatus.Elaboration ||
            transaction.Workflow.CurrentStatus == LRSTransactionStatus.Juridic)) {
        return false;
      }

      if (transaction.Workflow.GetCurrentTask().Responsible.Id == ExecutionServer.CurrentUserId) {
        return true;
      } else {
        return false;
      }
    }


    static public bool IsStatusOfficeWork(LRSTransactionStatus currentStatus) {
      if (currentStatus == LRSTransactionStatus.Payment || currentStatus == LRSTransactionStatus.ToDeliver ||
          currentStatus == LRSTransactionStatus.ToReturn || currentStatus == LRSTransactionStatus.Delivered ||
          currentStatus == LRSTransactionStatus.Returned || currentStatus == LRSTransactionStatus.Archived) {
        return false;
      }
      return true;
    }


    static public string ValidateStatusChange(LRSTransaction transaction, LRSTransactionStatus nextStatus) {
      if (nextStatus == LRSTransactionStatus.Received && transaction.Payments.Count == 0) {
        return "Este trámite todavía no tiene registrada una boleta de pago.";
      }

      if (!IsRecordingDocumentCase(transaction.TransactionType, transaction.DocumentType)) {
        return String.Empty;
      }

      if (transaction.TransactionType.Id == 704) {
        return String.Empty;
      }

      if (nextStatus == LRSTransactionStatus.Revision || nextStatus == LRSTransactionStatus.OnSign ||
          nextStatus == LRSTransactionStatus.Archived || nextStatus == LRSTransactionStatus.ToDeliver) {
        if (transaction.Document.IsEmptyInstance) {
          return "Necesito primero se ingrese la información del documento a inscribir.";
        }
      }

      return String.Empty;
    }


    static public string ValidateTakeTransaction(LRSTransaction transaction) {
      return String.Empty;
    }

    #endregion Methods

  }  // class LRSWorkflowRules

}  // namespace Empiria.Land.Registration.Transactions
