/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSWorkflowRules                               Pattern  : Micro-workflow                      *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Micro-workflow rules set for the Land Registration System.                                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

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

    private static bool IsJuridicCase(LRSTransactionType transactionType, LRSDocumentType documentType) {
      if (transactionType.Id == 706 && documentType.Id == 734) {
        return true;
      }
      return false;
    }

    static public List<LRSTransactionStatus> GetNextStatusList(LRSTransactionType type,
                                      LRSDocumentType docType, LRSTransactionStatus currentStatus) {
      List<LRSTransactionStatus> list = new List<LRSTransactionStatus>();

      switch (currentStatus) {

        case LRSTransactionStatus.Payment:
          list.Add(LRSTransactionStatus.Received);
          list.Add(LRSTransactionStatus.Deleted);
          break;

        case LRSTransactionStatus.Received:
        case LRSTransactionStatus.Reentry:
          if (ExecutionServer.LicenseName == "Tlaxcala") {
            if (type.Id == 699 || (type.Id == 706 && (docType.Id == 744))) {
              list.Add(LRSTransactionStatus.Recording);
            }
          }
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.Process:
        case LRSTransactionStatus.Control:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            // Certificado || Cancelación || Copia simple
            if (type.Id == 701 || type.Id == 704 || docType.Id == 723 || docType.Id == 734) {
              list.Add(LRSTransactionStatus.Elaboration);
            } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
              list.Add(LRSTransactionStatus.Qualification);
              list.Add(LRSTransactionStatus.Recording);
              list.Add(LRSTransactionStatus.Elaboration);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            AddRecordingOrElaborationStatus(list, type, docType);
            if (LRSWorkflowRules.IsArchivable(type, docType)) {
              list.Add(LRSTransactionStatus.Archived);
            }
            list.Add(LRSTransactionStatus.Juridic);
          }
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.OnSign);
          if (ExecutionServer.LicenseName == "Tlaxcala" && LRSWorkflowRules.IsDigitalizable(type, docType)) {
            list.Add(LRSTransactionStatus.Digitalization);
          }
          list.Add(LRSTransactionStatus.ToReturn);
          if (ExecutionServer.LicenseName == "Zacatecas" || LRSWorkflowRules.IsCertificateIssueCase(type, docType)) {
            list.Add(LRSTransactionStatus.ToDeliver);
          }
          break;

        case LRSTransactionStatus.Juridic:           // Only used in Tlaxcala
          if (ExecutionServer.LicenseName == "Tlaxcala") {
            AddRecordingOrElaborationStatus(list, type, docType);
            list.Add(LRSTransactionStatus.Revision);
            list.Add(LRSTransactionStatus.OnSign);
            list.Add(LRSTransactionStatus.ToReturn);
            list.Add(LRSTransactionStatus.Archived);
            list.Add(LRSTransactionStatus.Control);
          }
          break;

        case LRSTransactionStatus.Qualification:       // Only used in Zacatecas
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(LRSTransactionStatus.Recording);
            list.Add(LRSTransactionStatus.Revision);
            list.Add(LRSTransactionStatus.Qualification);
            list.Add(LRSTransactionStatus.ToReturn);
            list.Add(LRSTransactionStatus.Control);
          }
          break;

        case LRSTransactionStatus.Recording:
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.Recording);
          list.Add(LRSTransactionStatus.Control);
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(LRSTransactionStatus.ToReturn);
            if (LRSWorkflowRules.IsArchivable(type, docType)) {
              list.Add(LRSTransactionStatus.Archived);
            }
            if (docType.Id == 728) {
              list.Add(LRSTransactionStatus.OnSign);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(LRSTransactionStatus.Juridic);
            if (LRSWorkflowRules.IsArchivable(type, docType)) {
              list.Add(LRSTransactionStatus.Archived);
            }
            if (type.Id == 704) {    // Trámite comercio
              list.Add(LRSTransactionStatus.ToDeliver);
              list.Add(LRSTransactionStatus.ToReturn);
            } else if (ExecutionServer.CurrentPrincipal.IsInRole("LRSTransaction.LawyerRegister")) {
              list.Add(LRSTransactionStatus.ToReturn);
            }
          }
          break;

        case LRSTransactionStatus.Elaboration:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            if (docType.Id == 734) {
              list.Add(LRSTransactionStatus.ToDeliver);
            } else if (type.Id == 704) {
              list.Add(LRSTransactionStatus.OnSign);
            } else {
              list.Add(LRSTransactionStatus.Revision);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(LRSTransactionStatus.Revision);
          }
          list.Add(LRSTransactionStatus.Elaboration);
          list.Add(LRSTransactionStatus.Control);
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(LRSTransactionStatus.ToReturn);
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(LRSTransactionStatus.Juridic);
            if (ExecutionServer.CurrentPrincipal.IsInRole("LRSTransaction.LawyerRegister")) {
              list.Add(LRSTransactionStatus.ToReturn);
            }
          }
          break;

        case LRSTransactionStatus.Revision:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(LRSTransactionStatus.OnSign);
            if (type.Id == 701 || docType.Id == 723) {
              list.Add(LRSTransactionStatus.Elaboration);
            } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
              list.Add(LRSTransactionStatus.Recording);
            } else if (type.Id == 704) {
              list.Add(LRSTransactionStatus.Elaboration);
            }
            if (LRSWorkflowRules.IsArchivable(type, docType)) {
              list.Add(LRSTransactionStatus.Archived);
            }
            list.Add(LRSTransactionStatus.Revision);
            list.Add(LRSTransactionStatus.Control);
            list.Add(LRSTransactionStatus.ToReturn);
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(LRSTransactionStatus.OnSign);
            if (LRSWorkflowRules.IsArchivable(type, docType)) {
              list.Add(LRSTransactionStatus.Archived);
            }
            AddRecordingOrElaborationStatus(list, type, docType);
            list.Add(LRSTransactionStatus.Control);
          }
          break;

        case LRSTransactionStatus.OnSign:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(LRSTransactionStatus.ToDeliver);
            list.Add(LRSTransactionStatus.Revision);
            if (type.Id == 701 || docType.Id == 723) {
              list.Add(LRSTransactionStatus.Elaboration);
            } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
              list.Add(LRSTransactionStatus.Recording);
            } else if (type.Id == 704) {
              list.Add(LRSTransactionStatus.Elaboration);
            }
            list.Add(LRSTransactionStatus.Control);
            list.Add(LRSTransactionStatus.ToReturn);
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            if (LRSWorkflowRules.IsDigitalizable(type, docType)) {
              list.Add(LRSTransactionStatus.Digitalization);
            } else {
              list.Add(LRSTransactionStatus.ToDeliver);
            }
            list.Add(LRSTransactionStatus.ToReturn);
            list.Add(LRSTransactionStatus.Control);
            list.Add(LRSTransactionStatus.Juridic);
          }
          break;

        case LRSTransactionStatus.Digitalization:
          list.Add(LRSTransactionStatus.ToDeliver);
          list.Add(LRSTransactionStatus.ToReturn);
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.ToDeliver:
          list.Add(LRSTransactionStatus.Delivered);
          if (LRSWorkflowRules.IsDigitalizable(type, docType)) {
            list.Add(LRSTransactionStatus.Digitalization);
          }
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.Archived:
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.ToReturn:
          list.Add(LRSTransactionStatus.Returned);
          list.Add(LRSTransactionStatus.Control);
          break;
      }
      return list;
    }

    private static void AddRecordingOrElaborationStatus(List<LRSTransactionStatus> list,
                                                        LRSTransactionType type,
                                                        LRSDocumentType docType) {
      if (LRSWorkflowRules.IsRecordingDocumentCase(type, docType)) {
        list.Add(LRSTransactionStatus.Recording);
      } else if (LRSWorkflowRules.IsCertificateIssueCase(type, docType)) {
        list.Add(LRSTransactionStatus.Elaboration);
      } else {
        list.Add(LRSTransactionStatus.Elaboration);
        list.Add(LRSTransactionStatus.Recording);
      }
    }

    static public string GetStatusName(LRSTransactionStatus status) {
      switch (status) {
        case LRSTransactionStatus.Payment:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            return "Precalificación";
          } else {
            return "Calificación";
          }
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
          return "Archivado";
        default:
          return "No determinado";
      }
    }

    static internal bool IsArchivable(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Zacatecas") {
        if (docType.Id == 722 || docType.Id == 761) {
          return true;
        }
      }
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 699 || type.Id == 704 || (type.Id == 706 &&
           EmpiriaMath.IsMemberOf(docType.Id, new int[] { 733, 734, 736, 737, 738, 739, 740,
                                                          741, 742, 744, 755, 756 }))) {
          return true;
        }
      }
      return false;
    }

    static internal bool IsCertificateIssueCase(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 702) {    // Certificados
          return true;
        } else if (type.Id == 706 &&
                   EmpiriaMath.IsMemberOf(docType.Id, new int[] { 709, 735, 743, 745, 746, 747 })) {
          return true;
        }
      }
      return false;
    }

    static public bool IsEmptyItemsTransaction(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 706) {
        if (EmpiriaMath.IsMemberOf(transaction.DocumentType.Id, new int[] { 733, 738, 734, 742 })) {
          return true;
        }
      }
      return false;
    }

    static public bool IsReadyForReentry(LRSTransaction transaction) {
      var user = ExecutionServer.CurrentPrincipal;

      return ((transaction.Workflow.CurrentStatus == LRSTransactionStatus.Returned ||
              (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Delivered ||
               transaction.Workflow.CurrentStatus == LRSTransactionStatus.Archived)) &&
               user.IsInRole("LRSTransaction.ReentryByFails"));
    }

    static public bool IsReadyForGenerateImagingControlID(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance || transaction.Document.IsEmptyInstance) {
        return false;
      }
      if (!ExecutionServer.CurrentPrincipal.IsInRole("LRSTransaction.DocumentSafeguard")) {
        return false;
      }
      if (transaction.Document.ImagingControlID.Length != 0) {
        return false;
      }
      if (transaction.Document.RecordingActs.Count == 0) {
        return false;
      }
      if (!transaction.Document.IsClosed) {
        return false;
      }
      if (!LRSWorkflowRules.IsDigitalizable(transaction.TransactionType, transaction.DocumentType)) {
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
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 699 || type.Id == 700 || type.Id == 704 || type.Id == 707) {
          return true;
        } else if (type.Id == 706 && !NotRecordableDocumentType(docType)) {
          return true;
        }
      }
      return false;
    }

    private static bool NotRecordableDocumentType(LRSDocumentType docType) {
      return EmpiriaMath.IsMemberOf(docType.Id, new int[] { 709, 733, 734, 735, 742, 743, 745, 746, 747 });
    }

    static public bool IsDigitalizable(LRSTransactionType type, LRSDocumentType docType) {
      if (!IsRecordingDocumentCase(type, docType)) {
        return false;
      }
      if (IsCertificateIssueCase(type, docType)) {
        return false;
      }
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 699 || type.Id == 702) {
          return false;
        } else if (NotRecordableDocumentType(docType)) {
          return false;
        } else if (EmpiriaMath.IsMemberOf(docType.Id, new int[] { 715, 724, 728, 739, 744, 757 } )) {
          return false;
        }
      }
      return true;
    }

    static internal bool UserCanEditDocument(RecordingDocument document) {
      if (!(ExecutionServer.CurrentPrincipal.IsInRole("LRSTransaction.Register") ||
            ExecutionServer.CurrentPrincipal.IsInRole("LRSTransaction.Certificates") ||
            ExecutionServer.CurrentPrincipal.IsInRole("LRSTransaction.Juridic"))) {
        return false;
      }
      if (document.IsHistoricDocument) {
        return true;
      }
      var transaction = document.GetTransaction();

      Assertion.Assert(!transaction.IsEmptyInstance,
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

    static public string ValidateStatusChange(LRSTransaction transaction, LRSTransactionStatus newStatus) {
      if (newStatus == LRSTransactionStatus.Received) {
        if (transaction.Payments.Count == 0) {
          return "Este trámite todavía no tiene registrada una boleta de pago.";
        }
      }
      if (IsRecordingDocumentCase(transaction.TransactionType, transaction.DocumentType)) {
        if (transaction.TransactionType.Id == 704 || transaction.DocumentType.Id == 721) {
          return String.Empty;
        }
      }
      if (IsRecordingDocumentCase(transaction.TransactionType, transaction.DocumentType)) {
        if (newStatus == LRSTransactionStatus.Revision || newStatus == LRSTransactionStatus.OnSign ||
            newStatus == LRSTransactionStatus.Digitalization || newStatus == LRSTransactionStatus.ToDeliver) {
          if (transaction.Document.IsEmptyInstance) {
            return "Este trámite no tiene inscrito un documento. No es posible realizar esta operación.";
          }
        }
      }
      return String.Empty;
    }

    #endregion Methods

  }  // class LRSWorkflowRules

}  // namespace Empiria.Land.Registration.Transactions
