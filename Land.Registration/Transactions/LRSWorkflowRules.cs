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
      if (LRSWorkflowRules.IsRecordable(transaction.TransactionType, transaction.DocumentType)) {
        return LRSTransactionStatus.Recording;
      } else if (LRSWorkflowRules.IsCertificateIssue(transaction.TransactionType, transaction.DocumentType)) {
        return LRSTransactionStatus.Elaboration;
      } else {
        return LRSTransactionStatus.Control;
      }      
    }

    static public List<LRSTransactionStatus> GetNextStatusList(LRSTransactionType type, LRSDocumentType docType,
                                                                LRSTransactionStatus currentStatus) {
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
            if (LRSWorkflowRules.IsRecordable(type, docType)) {
              list.Add(LRSTransactionStatus.Recording);
            } else if (LRSWorkflowRules.IsCertificateIssue(type, docType)) {
              list.Add(LRSTransactionStatus.Elaboration);
            } else {
              list.Add(LRSTransactionStatus.Elaboration);
              list.Add(LRSTransactionStatus.Recording);
            }
            if (LRSWorkflowRules.IsArchivable(type, docType)) {
              list.Add(LRSTransactionStatus.Finished);
            }
            list.Add(LRSTransactionStatus.Juridic);
          }
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.OnSign);
          if (ExecutionServer.LicenseName == "Tlaxcala" && LRSWorkflowRules.IsSafeguardable(type, docType)) {
            list.Add(LRSTransactionStatus.Safeguard);
          }
          list.Add(LRSTransactionStatus.ToReturn);
          if (ExecutionServer.LicenseName == "Zacatecas" || LRSWorkflowRules.IsCertificateIssue(type, docType)) {
            list.Add(LRSTransactionStatus.ToDeliver);
          }
          break;

        case LRSTransactionStatus.Juridic:           // Only used in Tlaxcala
          if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(LRSTransactionStatus.Control);
            list.Add(LRSTransactionStatus.Revision);
            list.Add(LRSTransactionStatus.OnSign);
            list.Add(LRSTransactionStatus.ToReturn);
            list.Add(LRSTransactionStatus.Finished);
          }
          break;

        case LRSTransactionStatus.Qualification:       // Only used in Zacatecas
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(LRSTransactionStatus.Recording);
            list.Add(LRSTransactionStatus.Revision);
            list.Add(LRSTransactionStatus.Qualification);
            list.Add(LRSTransactionStatus.Control);
            list.Add(LRSTransactionStatus.ToReturn);
          }
          break;

        case LRSTransactionStatus.Recording:
          list.Add(LRSTransactionStatus.Revision);
          list.Add(LRSTransactionStatus.Recording);
          list.Add(LRSTransactionStatus.Control);
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(LRSTransactionStatus.ToReturn);
            if (LRSWorkflowRules.IsArchivable(type, docType)) {
              list.Add(LRSTransactionStatus.Finished);
            }
            if (docType.Id == 728) {
              list.Add(LRSTransactionStatus.OnSign);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(LRSTransactionStatus.Juridic);
            if (LRSWorkflowRules.IsArchivable(type, docType)) {
              list.Add(LRSTransactionStatus.Finished);
            }
            if (type.Id == 704) {    // Trámite comercio
              list.Add(LRSTransactionStatus.ToDeliver);
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
              list.Add(LRSTransactionStatus.Finished);
            }
            list.Add(LRSTransactionStatus.Revision);
            list.Add(LRSTransactionStatus.Control);
            list.Add(LRSTransactionStatus.ToReturn);
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(LRSTransactionStatus.OnSign);
            list.Add(LRSTransactionStatus.Control);
            if (LRSWorkflowRules.IsArchivable(type, docType)) {
              list.Add(LRSTransactionStatus.Finished);
            }
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
            if (LRSWorkflowRules.IsSafeguardable(type, docType)) {
              list.Add(LRSTransactionStatus.Safeguard);
            } else {
              list.Add(LRSTransactionStatus.ToDeliver);
            }
            list.Add(LRSTransactionStatus.ToReturn);
            list.Add(LRSTransactionStatus.Control);
            list.Add(LRSTransactionStatus.Juridic);
          }
          break;

        case LRSTransactionStatus.Safeguard:
          list.Add(LRSTransactionStatus.ToDeliver);
          list.Add(LRSTransactionStatus.ToReturn);
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.ToDeliver:
          list.Add(LRSTransactionStatus.Delivered);
          if (LRSWorkflowRules.IsSafeguardable(type, docType)) {
            list.Add(LRSTransactionStatus.Safeguard);
          }
          list.Add(LRSTransactionStatus.Control);
          break;

        case LRSTransactionStatus.ToReturn:
          list.Add(LRSTransactionStatus.Returned);
          list.Add(LRSTransactionStatus.Control);
          break;
      }
      return list;
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
        case LRSTransactionStatus.Safeguard:
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
        case LRSTransactionStatus.Finished:
          return "Archivar trámite / Terminado";
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

    static internal bool IsCertificateIssue(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 702) {    // Certificados
          return true;
        } else if (EmpiriaMath.IsMemberOf(docType.Id, new int[] { 709, 735, 743, 745, 746, 747 })) {
          return true;
        }
      }
      return false;
    }

    static public bool IsEmptyItemsTransaction(LRSTransaction transaction) {
      if (transaction.TransactionType.Id == 706) {
        if (EmpiriaMath.IsMemberOf(transaction.DocumentType.Id, new int[] { 733, 738, 734, 742, 756 })) {
          return true;
        }
      }
      return false;
    }

    static public bool IsReadyForReentry(LRSTransaction transaction) {
      var user = ExecutionServer.CurrentPrincipal;

      return ((transaction.Workflow.CurrentStatus == LRSTransactionStatus.Returned) ||
              (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Delivered &&
               user.IsInRole("LRSTransaction.ReentryByFails")));
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
      if (!LRSWorkflowRules.IsSafeguardable(transaction.TransactionType, transaction.DocumentType)) {
        return false;
      }
      if (transaction.Workflow.CurrentStatus == LRSTransactionStatus.Safeguard ||
          transaction.Workflow.CurrentStatus == LRSTransactionStatus.ToDeliver ||
          transaction.Workflow.CurrentStatus == LRSTransactionStatus.Delivered) {
        return true;
      }
      return false;
    }

    static public bool IsRecordable(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 699 || type.Id == 700 || type.Id == 704 || type.Id == 707) {
          return true;
        } else if (EmpiriaMath.IsMemberOf(docType.Id, new int[] {719, 721, 728, 733, 736, 737, 738, 739,
                                                                 740, 741, 742, 744, 755, 756 })) {
          return true;
        }
      }
      return false;
    }

    static public bool IsSafeguardable(LRSTransactionType type, LRSDocumentType docType) {
      if (!IsRecordable(type, docType)) {
        return false;
      }
      if (IsCertificateIssue(type, docType)) {
        return false;
      }
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 699 || type.Id == 702) {
          return false;
        } else if (EmpiriaMath.IsMemberOf(docType.Id, new int[] { 715, 724, 728, 734, 735, 739,
                                                                  743, 744, 745, 747, 757 })) {
          return false;
        }
      }
      return true;
    }

    static public bool IsStatusOfficeWork(LRSTransactionStatus currentStatus) {
      if (currentStatus == LRSTransactionStatus.Payment || currentStatus == LRSTransactionStatus.ToDeliver ||
          currentStatus == LRSTransactionStatus.ToReturn || currentStatus == LRSTransactionStatus.Delivered ||
          currentStatus == LRSTransactionStatus.Returned || currentStatus == LRSTransactionStatus.Finished) {
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
      if (IsRecordable(transaction.TransactionType, transaction.DocumentType)) {
        if (transaction.TransactionType.Id == 704 || transaction.DocumentType.Id == 721) {
          return String.Empty;
        }
      }
      if (IsRecordable(transaction.TransactionType, transaction.DocumentType)) {
        if (newStatus == LRSTransactionStatus.Revision || newStatus == LRSTransactionStatus.OnSign ||
            newStatus == LRSTransactionStatus.Safeguard || newStatus == LRSTransactionStatus.ToDeliver) {
          if (transaction.Document.IsEmptyInstance) {
            return "Necesito primero se ingrese la información del documento a inscribir.";
          }
        }
      }
      return String.Empty;
    }

    #endregion Methods

  }  // class LRSWorkflowRules

}  // namespace Empiria.Land.Registration.Transactions
