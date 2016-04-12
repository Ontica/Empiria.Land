/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : LRSWorkflow                                    Pattern  : Micro-workflow                      *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Micro-workflow for the Land Registration System.                                              *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Micro-workflow for the Land Registration System.</summary>
  public class LRSWorkflow {

    #region Fields

    private LRSTransaction _transaction = null;
    private Lazy<LRSTransactionTaskList> taskList = null;

    #endregion Fields

    #region Constructors and parsers

    internal LRSWorkflow(LRSTransaction transaction) {
      _transaction = transaction;
      this.taskList = 
           new Lazy<LRSTransactionTaskList>(() => LRSTransactionTaskList.Parse(_transaction));
      this.CurrentStatus = this.GetCurrentTask().CurrentStatus;
    }

    static public Contact InterestedContact {
      get {
        return Person.Parse(-6);
      }
    }

    static public string GetStatusName(TransactionStatus status) {
      switch (status) {
        case TransactionStatus.Payment:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            return "Precalificación";
          } else {
            return "Calificación";
          }
        case TransactionStatus.Received:
          return "Trámite recibido";
        case TransactionStatus.Reentry:
          return "Trámite reingresado";
        case TransactionStatus.Process:
          return "En mesas de trabajo";
        case TransactionStatus.Control:
          return "En mesa de control";
        case TransactionStatus.Qualification:
          return "En calificación";
        case TransactionStatus.Recording:
          return "En registro en libros";
        case TransactionStatus.Elaboration:
          return "En elaboración";
        case TransactionStatus.Revision:
          return "En revisión";
        case TransactionStatus.Juridic:
          return "En área jurídica";
        case TransactionStatus.OnSign:
          return "En firma";
        case TransactionStatus.Safeguard:
          return "En digitalización y resguardo";
        case TransactionStatus.ToDeliver:
          return "En ventanilla de entregas";
        case TransactionStatus.Delivered:
          return "Entregado al interesado";
        case TransactionStatus.ToReturn:
          return "En ventanilla de devoluciones";
        case TransactionStatus.Returned:
          return "Devuelto al interesado";
        case TransactionStatus.Deleted:
          return "Trámite eliminado";
        case TransactionStatus.Finished:
          return "Archivar trámite / Terminado";
        default:
          return "No determinado";
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public bool IsEmptyItemsTransaction {
      get {
        if (_transaction.TransactionType.Id == 706) {
          if (EmpiriaMath.IsMemberOf(_transaction.DocumentType.Id, new int[] { 733, 738, 734, 742, 756 })) {
            return true;
          }
        }
        return false;
      }
    }

    public bool ReadyForReentry {
      get {
        var user = Empiria.ExecutionServer.CurrentPrincipal;
        return ((this.CurrentStatus == TransactionStatus.Returned) ||
                (this.CurrentStatus == TransactionStatus.Delivered &&
                 user.IsInRole("LRSTransaction.ReentryByFails")));
      }
    }

    public TransactionStatus CurrentStatus {
      get;
      private set;
    } = TransactionStatus.Payment;

    public string CurrentStatusName {
      get {
        return LRSWorkflow.GetStatusName(this.CurrentStatus);
      }
    }

    public LRSTransactionTaskList Tasks {
      get {
        return taskList.Value;
      }
    }

    #endregion Properties

    #region Public methods

    static public bool StatusIsOfficeWork(TransactionStatus currentStatus) {
      if (currentStatus == TransactionStatus.Payment || currentStatus == TransactionStatus.ToDeliver ||
          currentStatus == TransactionStatus.ToReturn || currentStatus == TransactionStatus.Delivered ||
          currentStatus == TransactionStatus.Returned || currentStatus == TransactionStatus.Finished) {
        return false;
      }
      return true;
    }

    public void Receive(string notes) {
      if (this.CurrentStatus != TransactionStatus.Payment) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            _transaction.UID);
      }
      if (_transaction.Payments.Count == 0 && !this.IsEmptyItemsTransaction) {
        throw new NotImplementedException("Este trámite todavía no tiene registrada una boleta de pago.");
      }

      //    using (var context = StorageContext.Open()) {
      _transaction.PresentationTime = DateTime.Now;
      _transaction.ClosingTime = ExecutionServer.DateMaxValue;
      this.CurrentStatus = TransactionStatus.Received;

      LRSWorkflowTask currentTask = this.GetCurrentTask();
      currentTask.NextStatus = TransactionStatus.Received;
      currentTask.NextContact = LRSWorkflow.InterestedContact;

      currentTask = currentTask.CreateNext(notes);
      currentTask.NextStatus = this.GetAfterReceiveNextStatus();
      currentTask.Status = TrackStatus.OnDelivery;
      currentTask.EndProcessTime = currentTask.CheckInTime;
      currentTask.AssignedBy = LRSWorkflow.InterestedContact;
      currentTask.Save();

      _transaction.Save();
      this.ResetTasksList();
      //  }
    }

    public void ReturnToMe() {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      TransactionStatus nextStatus = currentTask.NextStatus;
      currentTask.SetPending();

      _transaction.Save();
      ResetTasksList();
    }

    internal Contact GetPostedBy() {
      if (this.Tasks.Count != 0) {
        return this.Tasks[0].Responsible;
      } else {
        return Person.Empty;
      }
    }

    internal DateTime GetPostingTime() {
      if (this.Tasks.Count != 0) {
        return this.Tasks[0].CheckInTime;
      } else {
        return ExecutionServer.DateMaxValue;
      }
    }

    internal Contact GetReceivedBy() {
      var task = this.Tasks.Find((x) => x.CurrentStatus == TransactionStatus.Received);

      if (task != null) {
        return task.Responsible;
      } else {
        return Person.Empty;
      }
    }

    public void SetNextStatus(TransactionStatus nextStatus, Contact nextContact, string notes) {
      if (nextStatus == TransactionStatus.Returned || nextStatus == TransactionStatus.Delivered ||
          nextStatus == TransactionStatus.Finished) {
        this.Close(nextStatus, notes);
        return;
      }
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      currentTask.SetNextStatus(nextStatus, nextContact, notes);

      if (nextStatus == TransactionStatus.OnSign || nextStatus == TransactionStatus.Revision) {
        _transaction.Save();
      } else if (this.CurrentStatus == TransactionStatus.OnSign &&
                (nextStatus == TransactionStatus.ToDeliver || nextStatus == TransactionStatus.Finished)) {
        _transaction.Save();
      }
    }

    public void Take(string notes) {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      if (currentTask.NextStatus == TransactionStatus.EndPoint) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint,
                                            currentTask.Id);
      }
      this.CurrentStatus = currentTask.NextStatus;
      currentTask.CreateNext(notes);
      ResetTasksList();

      if (this.CurrentStatus == TransactionStatus.ToDeliver ||
          this.CurrentStatus == TransactionStatus.ToReturn || this.CurrentStatus == TransactionStatus.Finished) {
        _transaction.ClosingTime = currentTask.EndProcessTime;
      }
      _transaction.Save();
    }

    public void Undelete() {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      if (currentTask.Status == TrackStatus.OnDelivery) {
        this.CurrentStatus = currentTask.CurrentStatus;
      } else if (currentTask.Status == TrackStatus.Pending) {
        this.CurrentStatus = currentTask.CurrentStatus;
      } else if (currentTask.Status == TrackStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint,
                                            currentTask.Id);
      }
      _transaction.Save();
    }

    public string ValidateStatusChange(TransactionStatus newStatus) {
      if (newStatus == TransactionStatus.Received) {
        if (_transaction.Payments.Count == 0) {
          return "Este trámite todavía no tiene registrada una boleta de pago.";
        }
      }
      if (IsRecordable(_transaction.TransactionType, _transaction.DocumentType)) {
        if (_transaction.TransactionType.Id == 704 || _transaction.DocumentType.Id == 721) {
          return String.Empty;
        }
      }
      if (IsRecordable(_transaction.TransactionType, _transaction.DocumentType)) {
        if (newStatus == TransactionStatus.Revision || newStatus == TransactionStatus.OnSign ||
            newStatus == TransactionStatus.Safeguard || newStatus == TransactionStatus.ToDeliver) {
          if (_transaction.Document.IsEmptyInstance) {
            return "Necesito primero se ingrese la información del documento a inscribir.";
          }
        }
      }
      return String.Empty;
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

    public void DoReentry(string notes) {
      if (!this.ReadyForReentry) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            _transaction.UID);
      }
      this.CurrentStatus = TransactionStatus.Reentry;
      _transaction.ClosingTime = ExecutionServer.DateMaxValue;
      _transaction.LastReentryTime = DateTime.Now;
      LRSWorkflowTask currentTask = this.GetCurrentTask();
      currentTask.NextStatus = TransactionStatus.Reentry;

      currentTask = currentTask.CreateNext(notes);
      currentTask.NextStatus = TransactionStatus.Control;
      currentTask.Status = TrackStatus.OnDelivery;
      currentTask.EndProcessTime = currentTask.CheckInTime;
      currentTask.AssignedBy = LRSWorkflow.InterestedContact;
      currentTask.Save();
      _transaction.Save();
      this.ResetTasksList();
    }

    public LRSWorkflowTask GetCurrentTask() {
      return WorkflowData.GetLRSWorkflowLastTask(_transaction);
    }

    static public List<TransactionStatus> NextStatusList(LRSTransactionType type, LRSDocumentType docType,
                                                         TransactionStatus status) {
      List<TransactionStatus> list = new List<TransactionStatus>();

      switch (status) {
        case TransactionStatus.Payment:
          list.Add(TransactionStatus.Received);
          list.Add(TransactionStatus.Deleted);
          break;
        case TransactionStatus.Received:
        case TransactionStatus.Reentry:
          if (ExecutionServer.LicenseName == "Tlaxcala") {
            if (type.Id == 699 || (type.Id == 706 && (docType.Id == 744))) {
              list.Add(TransactionStatus.Recording);
            }
          }
          list.Add(TransactionStatus.Control);
          break;
        case TransactionStatus.Process:
        case TransactionStatus.Control:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            // Certificado || Cancelación || Copia simple
            if (type.Id == 701 || type.Id == 704 || docType.Id == 723 || docType.Id == 734) {
              list.Add(TransactionStatus.Elaboration);
            } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
              list.Add(TransactionStatus.Qualification);
              list.Add(TransactionStatus.Recording);
              list.Add(TransactionStatus.Elaboration);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            if (LRSWorkflow.IsRecordable(type, docType)) {
              list.Add(TransactionStatus.Recording);
            } else if (LRSWorkflow.IsCertificateIssue(type, docType)) {
              list.Add(TransactionStatus.Elaboration);
            } else {
              list.Add(TransactionStatus.Elaboration);
              list.Add(TransactionStatus.Recording);
            }
            if (LRSWorkflow.IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            list.Add(TransactionStatus.Juridic);
          }
          list.Add(TransactionStatus.Revision);
          list.Add(TransactionStatus.OnSign);
          if (ExecutionServer.LicenseName == "Tlaxcala" && LRSWorkflow.IsSafeguardable(type, docType)) {
            list.Add(TransactionStatus.Safeguard);
          }
          list.Add(TransactionStatus.ToReturn);
          if (ExecutionServer.LicenseName == "Zacatecas" || LRSWorkflow.IsCertificateIssue(type, docType)) {
            list.Add(TransactionStatus.ToDeliver);
          }
          break;
        case TransactionStatus.Juridic:           // Only used in Tlaxcala
          if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(TransactionStatus.Control);
            list.Add(TransactionStatus.Revision);
            list.Add(TransactionStatus.OnSign);
            list.Add(TransactionStatus.ToReturn);
            list.Add(TransactionStatus.Finished);
          }
          break;
        case TransactionStatus.Qualification:       // Only used in Zacatecas
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(TransactionStatus.Recording);
            list.Add(TransactionStatus.Revision);
            list.Add(TransactionStatus.Qualification);
            list.Add(TransactionStatus.Control);
            list.Add(TransactionStatus.ToReturn);
          }
          break;
        case TransactionStatus.Recording:
          list.Add(TransactionStatus.Revision);
          list.Add(TransactionStatus.Recording);
          list.Add(TransactionStatus.Control);
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(TransactionStatus.ToReturn);
            if (LRSWorkflow.IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            if (docType.Id == 728) {
              list.Add(TransactionStatus.OnSign);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(TransactionStatus.Juridic);
            if (LRSWorkflow.IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            if (type.Id == 704) {    // Trámite comercio
              list.Add(TransactionStatus.ToDeliver);
              list.Add(TransactionStatus.ToReturn);
            }
          }
          break;
        case TransactionStatus.Elaboration:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            if (docType.Id == 734) {
              list.Add(TransactionStatus.ToDeliver);
            } else if (type.Id == 704) {
              list.Add(TransactionStatus.OnSign);
            } else {
              list.Add(TransactionStatus.Revision);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(TransactionStatus.Revision);
          }
          list.Add(TransactionStatus.Elaboration);
          list.Add(TransactionStatus.Control);
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(TransactionStatus.ToReturn);
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(TransactionStatus.Juridic);
          }
          break;
        case TransactionStatus.Revision:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(TransactionStatus.OnSign);
            if (type.Id == 701 || docType.Id == 723) {
              list.Add(TransactionStatus.Elaboration);
            } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
              list.Add(TransactionStatus.Recording);
            } else if (type.Id == 704) {
              list.Add(TransactionStatus.Elaboration);
            }
            if (LRSWorkflow.IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            list.Add(TransactionStatus.Revision);
            list.Add(TransactionStatus.Control);
            list.Add(TransactionStatus.ToReturn);
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(TransactionStatus.OnSign);
            list.Add(TransactionStatus.Control);
            if (LRSWorkflow.IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
          }
          break;
        case TransactionStatus.OnSign:
          if (ExecutionServer.LicenseName == "Zacatecas") {
            list.Add(TransactionStatus.ToDeliver);
            list.Add(TransactionStatus.Revision);
            if (type.Id == 701 || docType.Id == 723) {
              list.Add(TransactionStatus.Elaboration);
            } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
              list.Add(TransactionStatus.Recording);
            } else if (type.Id == 704) {
              list.Add(TransactionStatus.Elaboration);
            }
            list.Add(TransactionStatus.Control);
            list.Add(TransactionStatus.ToReturn);
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            if (LRSWorkflow.IsSafeguardable(type, docType)) {
              list.Add(TransactionStatus.Safeguard);
            } else {
              list.Add(TransactionStatus.ToDeliver);
            }
            list.Add(TransactionStatus.ToReturn);
            list.Add(TransactionStatus.Control);
            list.Add(TransactionStatus.Juridic);
          }
          break;
        case TransactionStatus.Safeguard:
          list.Add(TransactionStatus.ToDeliver);
          list.Add(TransactionStatus.ToReturn);
          list.Add(TransactionStatus.Control);
          break;
        case TransactionStatus.ToDeliver:
          list.Add(TransactionStatus.Delivered);
          if (LRSWorkflow.IsSafeguardable(type, docType)) {
            list.Add(TransactionStatus.Safeguard);
          }
          list.Add(TransactionStatus.Control);
          break;
        case TransactionStatus.ToReturn:
          list.Add(TransactionStatus.Returned);
          list.Add(TransactionStatus.Control);
          break;
      }
      return list;
    }

    #endregion Public methods

    #region Private methods

    private TransactionStatus GetAfterReceiveNextStatus() {
      if (ExecutionServer.LicenseName == "Zacatecas") {
        return TransactionStatus.Control;
      }
      if (LRSWorkflow.IsRecordable(_transaction.TransactionType, _transaction.DocumentType)) {
        return TransactionStatus.Recording;
      } else if (LRSWorkflow.IsCertificateIssue(_transaction.TransactionType, _transaction.DocumentType)) {
        return TransactionStatus.Elaboration;
      }
      return TransactionStatus.Control;
    }

    private void Close(TransactionStatus closeStatus, string notes) {
      LRSWorkflowTask currentTask = this.GetCurrentTask();

      currentTask.NextStatus = closeStatus;
      currentTask = currentTask.CreateNext(notes);

      ResetTasksList();

      currentTask.Notes = notes;
      currentTask.Close();

      _transaction.LastDeliveryTime = currentTask.EndProcessTime;
      this.CurrentStatus = closeStatus;
      _transaction.Save();
    }

    static private bool IsArchivable(LRSTransactionType type, LRSDocumentType docType) {
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

    static private bool IsCertificateIssue(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 702) {    // Certificados
          return true;
        } else if (EmpiriaMath.IsMemberOf(docType.Id, new int[] { 709, 735, 743, 745, 746, 747 })) {
          return true;
        }
      }
      return false;
    }

    static private bool IsRecordable(LRSTransactionType type, LRSDocumentType docType) {
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

    internal void ResetTasksList() {
      taskList = new Lazy<LRSTransactionTaskList>(() => LRSTransactionTaskList.Parse(_transaction));
    }

    #endregion Private methods

  }  // class LRSWorkflow

}  // namespace Empiria.Land.Registration.Transactions
