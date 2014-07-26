﻿/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderOfficeTransaction                      Pattern  : Association Class                   *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a transaction or process in a land registration office.                            *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Land.Registration.Data;
using Empiria.Security;

namespace Empiria.Land.Registration.Transactions {

  public enum TransactionStatus {
    Payment = 'Y',
    Received = 'R',
    Reentry = 'N',
    Control = 'K',
    Qualification = 'F',
    Recording = 'G',
    Elaboration = 'E',
    Revision = 'V',

    Juridic = 'J',

    Process = 'P',
    OnSign = 'S',

    Safeguard = 'A',
    ToDeliver = 'D',
    Delivered = 'C',
    ToReturn = 'L',
    Returned = 'Q',
    Deleted = 'X',

    Finished = 'H',

    Undefined = 'U',
    EndPoint = 'Z',
  }

  /// <summary>Represents a transaction or process on a land registration office.</summary>
  public class LRSTransaction : BaseObject, IExtensible<LRSTransactionExtData>, IProtected {

    #region Fields

    private const string thisTypeName = "ObjectType.LRSTransaction";

    private Lazy<LRSTransactionItemList> _recordingActs = null;
    private Lazy<LRSPaymentList> _payments = null;
    private Lazy<LRSTransactionTaskList> _taskList = null;

    #endregion Fields

    #region Constuctors and parsers

    protected LRSTransaction(): base(thisTypeName) {
      Initialize();
    }

    public LRSTransaction(LRSTransactionType transactionType) : base(thisTypeName) {
      Initialize();
      this.TransactionType = transactionType;
    }

    protected LRSTransaction(string typeName) : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
      Initialize();
    }

    private void Initialize() {
      this.TransactionType = LRSTransactionType.Empty;
      this.UniqueCode = "Nuevo trámite";
      this.DocumentType = LRSDocumentType.Empty;
      this.DocumentDescriptor = String.Empty;
      this.Document = RecordingDocument.Empty;
      this.RecorderOffice = RecorderOffice.Empty;
      this.RequestedBy = String.Empty;
      this.ManagementAgency = Organization.Empty;
      this.ExtensionData = LRSTransactionExtData.Empty;
      this.Keywords = String.Empty;
      this.PresentationTime = ExecutionServer.DateMaxValue;
      this.ExpectedDelivery = ExecutionServer.DateMaxValue;
      this.LastReentryTime = ExecutionServer.DateMaxValue;
      this.ClosingTime = ExecutionServer.DateMaxValue;
      this.LastDeliveryTime = ExecutionServer.DateMaxValue;
      this.Status = TransactionStatus.Payment;

      _recordingActs = new Lazy<LRSTransactionItemList>(() => LRSTransactionItemList.Parse(this));
      _payments = new Lazy<LRSPaymentList>(() => LRSPaymentList.Parse(this));
      _taskList = new Lazy<LRSTransactionTaskList>(() => LRSTransactionTaskList.Parse(this));

    }

    static public LRSTransaction Parse(int id) {
      return BaseObject.Parse<LRSTransaction>(thisTypeName, id);
    }

    static public LRSTransaction ParseWithNumber(string transactionKey) {
      DataRow row = TransactionData.GetLRSTransactionWithKeyRow(transactionKey);

      if (row != null) {
        return LRSTransaction.Parse(row);
      } else {
        return null;
      }
    }

    static internal LRSTransaction Parse(DataRow dataRow) {
      return BaseObject.Parse<LRSTransaction>(thisTypeName, dataRow);
    }

    static public LRSTransaction Empty {
      get { return BaseObject.ParseEmpty<LRSTransaction>(thisTypeName); }
    }

    static public Contact InterestedContact {
      get {
        return Person.Parse(-6);
      }
    }

    static public FixedList<Contact> GetManagementAgenciesList() {
      GeneralList listType = GeneralList.Parse("LRSTransaction.ManagementAgencies.List");

      return listType.GetContacts<Contact>();
    }

    static public string StatusName(TransactionStatus status) {
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

    static public bool StatusIsOfficeWork(TransactionStatus currentStatus) {
      if (currentStatus == TransactionStatus.Payment || currentStatus == TransactionStatus.ToDeliver ||
          currentStatus == TransactionStatus.ToReturn || currentStatus == TransactionStatus.Delivered ||
          currentStatus == TransactionStatus.Returned || currentStatus == TransactionStatus.Finished) {
        return false;
      }
      return true;
    }

    #endregion Constructors and parsers

    #region Public properties

    public LRSTransactionType TransactionType {
      get;
      private set;
    }

    public string UniqueCode {
      get;
      private set;
    }

    public LRSDocumentType DocumentType {
      get;
      set;
    }

    public string DocumentDescriptor {
      get;
      set;
    }

    public RecordingDocument Document {
      get;
      private set;
    }

    public RecorderOffice RecorderOffice {
      get;
      set;
    }

    public string RequestedBy {
      get;
      set;
    }

    public Contact ManagementAgency {
      get;
      set;
    }

    public LRSTransactionExtData ExtensionData {
      get;
      set;
    }

    public string Keywords {
      get;
      private set;
    }

    public DateTime PresentationTime {
      get;
      private set;
    }
    
    public DateTime ExpectedDelivery {
      get;
      private set;
    }

    public DateTime LastReentryTime {
      get;
      private set;
    }

    public DateTime ClosingTime {
      get;
      private set;
    }

    public DateTime LastDeliveryTime {
      get;
      private set;
    }

    public int NonWorkingTime {
      get;
      private set;
    }

    public decimal ComplexityIndex {
      get;
      private set;
    }

    public bool IsArchived {
      get;
      private set;
    }

    public TransactionStatus Status {
      get;
      private set;
    }

    public bool ReadyForReentry {
      get {
        var user = EmpiriaUser.Current;
        return ((this.Status == TransactionStatus.Returned) ||
        (this.Status == TransactionStatus.Delivered && user.CanExecute("LRSTransaction.ReentryByFails")));
      }
    }

    public LRSTransactionItemList Items {
      get {
        return _recordingActs.Value;
      }
    }

    public LRSPaymentList Payments {
      get { 
        return _payments.Value;
      }
    }

    public bool IsFeeWaiverApplicable {
      get {
        return (this.TransactionType.Id == 704 || (this.TransactionType.Id == 700 && this.DocumentType.Id == 722));
      }
    }

    public LRSTransactionTaskList Tasks {
      get {
        return _taskList.Value;
      }
    }

    public Contact PostedBy {
      get {
        if (this.Tasks.Count != 0) {
          return this.Tasks[0].Responsible;
        } else {
          return Person.Empty;
        }        
      }
    }

    public DateTime PostingTime {
      get {
        if (this.Tasks.Count != 0) {
          return this.Tasks[0].CheckInTime;
        } else {
          return ExecutionServer.DateMaxValue;
        }
      }
    }

    public Contact ReceivedBy {
      get {
        var task = this.Tasks.Find((x) => x.CurrentStatus == TransactionStatus.Received);

        if (task != null) {
          return task.Responsible;
        } else {
          return Person.Empty;
        }
      } // get
    }

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "Id", this.Id, "TransactionTypeId", this.TransactionType.Id,
          "UniqueCode", this.UniqueCode, "DocumentTypeId", this.DocumentType.Id,
          "DocumentDescriptor", this.DocumentDescriptor, "DocumentId", this.Document.Id,
          "RecorderOfficeId", this.RecorderOffice.Id, "RequestedBy", this.RequestedBy, 
          "ManagementAgencyId", this.ManagementAgency.Id, "ExtensionData", this.ExtensionData.ToJson(),
          "PresentationTime", this.PresentationTime, "ExpectedDelivery", this.ExpectedDelivery,
          "LastReentryTime", this.LastReentryTime, "ClosingTime", this.ClosingTime,
          "LastDeliveryTime", this.LastDeliveryTime, "NonWorkingTime", this.NonWorkingTime,
          "ComplexityIndex", this.ComplexityIndex, "Status", (char) this.Status
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }

    private IntegrityValidator _validator = null;
    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    #endregion Public properties

    #region Public methods

    public LRSTransactionItem AddItem(RecordingActType transactionItemType,
                                      LRSLawArticle treasuryCode, Money operationValue, 
                                      Quantity quantity, LRSFee fee) {
      this.AssertAddItem();
      var item = new LRSTransactionItem(this, transactionItemType, treasuryCode,
                                        operationValue, quantity, fee);
      this.Items.Add(item);

      return item;
    }

    public LRSTransactionItem AddItem(RecordingActType transactionItemType,
                                      LRSLawArticle treasuryCode, Quantity quantity,
                                      Money operationValue) {
      this.AssertAddItem();
      var item = new LRSTransactionItem(this, transactionItemType, treasuryCode,
                                        operationValue, quantity);
      this.Items.Add(item);

      return item;
    }

    public LRSPayment AddPayment(string receiptNo, decimal receiptTotal) {
      this.AssertAddPayment();
      var payment = new LRSPayment(this, receiptNo, receiptTotal);

      this.Payments.Add(payment);

      return payment;
    }

    public void ApplyFeeWaiver() {
      this.Payments.Add(LRSPayment.FeeWaiver);
    }

    public void RemoveItem(LRSTransactionItem item) {
      this.Items.Remove(item);
    }

    public LRSTransaction MakeCopy() {
      LRSTransaction copy = new LRSTransaction(this.TransactionType);
      copy.RecorderOffice = this.RecorderOffice;
      copy.DocumentDescriptor = this.DocumentDescriptor;
      copy.DocumentType = this.DocumentType;
      copy.RequestedBy = this.RequestedBy;
      copy.ExtensionData.RequesterNotes = this.ExtensionData.RequesterNotes;

      if (this.IsFeeWaiverApplicable) {
        copy.ApplyFeeWaiver();
      }
      copy.Save();

      foreach (LRSTransactionItem item in this.Items) {
        LRSTransactionItem itemCopy = item.MakeCopy();
        if (this.IsFeeWaiverApplicable) {
          // OOJJOO Apply Fee Waiver on payment to each itemCopy ???
        }
        itemCopy.Save();
      }

      return copy;
    }

    public void AttachDocument(RecordingDocument document) {
      Assertion.AssertObject(document, "document");
 
      document.Save();
      this.Document = document;
      this.Save();
    }

    public void DoReentry(string notes) {
      if (!this.ReadyForReentry) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            this.UniqueCode);
      }
      this.Status = TransactionStatus.Reentry;
      this.ClosingTime = ExecutionServer.DateMaxValue;
      this.LastReentryTime = DateTime.Now;
      LRSTransactionTask currentTask = this.GetCurrentTask();
      currentTask.NextStatus = TransactionStatus.Reentry;
      
      currentTask = currentTask.CreateNext(notes);
      currentTask.NextStatus = TransactionStatus.Control;
      currentTask.Status = TrackStatus.OnDelivery;
      currentTask.EndProcessTime = currentTask.CheckInTime;
      currentTask.AssignedBy = LRSTransaction.InterestedContact;
      currentTask.Save();
      this.Save();
      this.ResetTasksList();
    }

    public LRSTransactionTask GetCurrentTask() {
      return TransactionData.GetTransactionLastTask(this);
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
            if (LRSTransaction.IsRecordable(type, docType)) {
              list.Add(TransactionStatus.Recording);
            } else if (LRSTransaction.IsCertificateIssue(type, docType)) {
              list.Add(TransactionStatus.Elaboration);
            } else {
              list.Add(TransactionStatus.Elaboration);
              list.Add(TransactionStatus.Recording);
            }
            if (LRSTransaction.IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            list.Add(TransactionStatus.Juridic);
          }
          list.Add(TransactionStatus.Revision);
          list.Add(TransactionStatus.OnSign);
          if (ExecutionServer.LicenseName == "Tlaxcala" && LRSTransaction.IsRecordable(type, docType)) {
            list.Add(TransactionStatus.Safeguard);
          }
          list.Add(TransactionStatus.ToReturn);
          if (ExecutionServer.LicenseName == "Zacatecas" || LRSTransaction.IsCertificateIssue(type, docType)) {
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
            if (LRSTransaction.IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            if (docType.Id == 728) {
              list.Add(TransactionStatus.OnSign);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(TransactionStatus.Juridic);
            if (LRSTransaction.IsArchivable(type, docType)) {
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
            if (LRSTransaction.IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            list.Add(TransactionStatus.Revision);
            list.Add(TransactionStatus.Control);
            list.Add(TransactionStatus.ToReturn);
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(TransactionStatus.OnSign);
            list.Add(TransactionStatus.Control);
            if (LRSTransaction.IsArchivable(type, docType)) {
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
            if (LRSTransaction.IsRecordable(type, docType)) {
              list.Add(TransactionStatus.Safeguard);
            } else if (LRSTransaction.IsCertificateIssue(type, docType)) {
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
          list.Add(TransactionStatus.Safeguard);
          list.Add(TransactionStatus.Control);
          break;
        case TransactionStatus.ToReturn:
          list.Add(TransactionStatus.Returned);
          list.Add(TransactionStatus.Control);
          break;
      }
      return list;
    }

    public string GetDigitalSign() {
      return Empiria.Security.Cryptographer.CreateDigitalSign(GetDigitalString());
    }

    public string GetDigitalString() {
      string temp = String.Empty;

      temp = "||" + this.Id.ToString() + "|" + this.UniqueCode + "|";
      if (this.PresentationTime != ExecutionServer.DateMaxValue) {
        temp += this.PresentationTime.ToString("yyyyMMddTHH:mm:ss") + "|";
      }
      foreach (LRSTransactionItem act in this.Items) {
        temp += "|" + act.Id.ToString() + "^" + act.TransactionItemType.Id.ToString() + "^";
        if (act.OperationValue.Amount != decimal.Zero) {
          temp += "B" + act.OperationValue.Amount.ToString("F4") + "^";
        }
        if (!act.Quantity.Unit.IsEmptyInstance) {
          temp += "Q" + act.Quantity.Amount.ToString("N2") + "^";
          temp += "U" + act.Quantity.Unit.Id.ToString() + "^";
        }
        temp += act.TreasuryCode.Id.ToString();
        if (ExecutionServer.LicenseName == "Tlaxcala") {
          temp += "^" + "S" + act.Fee.SubTotal.ToString("N2") + "^";
          temp += "D" + act.Fee.Discount.Amount.ToString("N2") + "^";
          temp += "T" + act.Fee.Total.ToString("N2");
        }
      }
      temp += "||";

      return temp;
    }

    protected override void ImplementsSave() {
      if (base.IsNew) {
        this.UniqueCode = TransactionData.GenerateTransactionKey();
      }
      this.Keywords = EmpiriaString.BuildKeywords(this.UniqueCode, this.Document.UniqueCode,
                                                  this.DocumentDescriptor, this.RequestedBy, 
                                                  this.ManagementAgency.FullName,
                                                  this.TransactionType.Name, 
                                                  this.RecorderOffice.Alias);
      TransactionData.WriteTransaction(this);
      if (base.IsNew) {
        LRSTransactionTask track = LRSTransactionTask.CreateFirst(this);
        ResetTasksList();
      }
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.TransactionType = LRSTransactionType.Parse((int) row["TransactionTypeId"]);
      this.UniqueCode = (string) row["TransactionUniqueCode"];
      this.DocumentType = LRSDocumentType.Parse((int) row["DocumentTypeId"]);
      this.DocumentDescriptor = (string) row["DocumentDescriptor"];
      this.Document = RecordingDocument.Parse((int) row["DocumentId"]);
      this.RecorderOffice = RecorderOffice.Parse((int) row["RecorderOfficeId"]);
      this.RequestedBy = (string) row["RequestedBy"];
      this.ManagementAgency = Contact.Parse((int) row["ManagementAgencyId"]);
      this.ExtensionData = LRSTransactionExtData.Parse((string) row["TransactionExtData"]);
      this.Keywords = (string) row["TransactionKeywords"];
      this.PresentationTime = (DateTime) row["PresentationTime"];
      this.ExpectedDelivery = (DateTime) row["ExpectedDelivery"];
      this.LastReentryTime = (DateTime) row["LastReentryTime"];
      this.ClosingTime = (DateTime) row["ClosingTime"];
      this.LastDeliveryTime = (DateTime) row["LastDeliveryTime"];
      this.NonWorkingTime = (int) row["NonWorkingTime"];
      this.ComplexityIndex = (decimal) row["ComplexityIndex"];
      this.IsArchived = (bool) row["IsArchived"];
      this.Status = (TransactionStatus) Convert.ToChar(row["TransactionStatus"]);

      Integrity.Assert((string) row["TransactionDIF"]);
    }

    internal void OnRecordingActsUpdated() {
      _recordingActs = new Lazy<LRSTransactionItemList>(() => LRSTransactionItemList.Parse(this));
      this.UpdateComplexityIndex();
    }

    public void Receive(string notes) {
      if (this.Status != TransactionStatus.Payment) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction,
                                            this.UniqueCode);
      }
      if (this.Payments.Count == 0) {
        throw new NotImplementedException("Este trámite todavía no tiene registrada una boleta de pago.");
      }

      this.PresentationTime = DateTime.Now;
      this.ClosingTime = ExecutionServer.DateMaxValue;
      this.Status = TransactionStatus.Received;

      LRSTransactionTask currentTask = this.GetCurrentTask();
      currentTask.NextStatus = TransactionStatus.Received;
      currentTask.NextContact = LRSTransaction.InterestedContact;

      currentTask = currentTask.CreateNext(notes);
      currentTask.NextStatus = TransactionStatus.Control;
      currentTask.Status = TrackStatus.OnDelivery;
      currentTask.EndProcessTime = currentTask.CheckInTime;
      currentTask.AssignedBy = LRSTransaction.InterestedContact;
      currentTask.Save();

      this.Save();
      this.ResetTasksList();
    }

    public void ReturnToMe() {
      LRSTransactionTask currentTask = this.GetCurrentTask();

      TransactionStatus nextStatus = currentTask.NextStatus;
      currentTask.SetPending();

      this.Save();
      ResetTasksList();
    }

    public void SetNextStatus(TransactionStatus nextStatus, Contact nextContact, string notes) {
      if (nextStatus == TransactionStatus.Returned || nextStatus == TransactionStatus.Delivered ||
          nextStatus == TransactionStatus.Finished) {
        this.Close(nextStatus, notes);
        return;
      }
      LRSTransactionTask currentTask = this.GetCurrentTask();

      currentTask.SetNextStatus(nextStatus, nextContact, notes);

      if (nextStatus == TransactionStatus.OnSign || nextStatus == TransactionStatus.Revision) {
 
        this.Save();
      } else if (this.Status == TransactionStatus.OnSign &&
                (nextStatus == TransactionStatus.ToDeliver || nextStatus == TransactionStatus.Finished)) {
        this.Save();
      }
    }

    public void Take(string notes) {
      LRSTransactionTask currentTask = this.GetCurrentTask();

      if (currentTask.NextStatus == TransactionStatus.EndPoint) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint,
                                            currentTask.Id);
      }
      this.Status = currentTask.NextStatus;
      currentTask.CreateNext(notes);
      ResetTasksList();

      if (this.Status == TransactionStatus.ToDeliver || 
          this.Status == TransactionStatus.ToReturn || this.Status == TransactionStatus.Finished) {
        this.ClosingTime = currentTask.EndProcessTime;
      }
      this.Save();
    }

    public void Undelete() {
      LRSTransactionTask currentTask = this.GetCurrentTask();

      if (currentTask.Status == TrackStatus.OnDelivery) {
        this.Status = currentTask.CurrentStatus;
      } else if (currentTask.Status == TrackStatus.Pending) {
        this.Status = currentTask.CurrentStatus;
      } else if (currentTask.Status == TrackStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint,
                                            currentTask.Id);
      }
      this.Save();
    }

    public string ValidateStatusChange(TransactionStatus newStatus) {
      if (newStatus == TransactionStatus.Received) {
        if (this.Payments.Count == 0) {
          return "Este trámite todavía no tiene registrada una boleta de pago.";
        }
      }
      if (IsRecordable(this.TransactionType, this.DocumentType)) {
        if (this.TransactionType.Id == 704 || this.DocumentType.Id == 721) {
          return String.Empty;
        }
      }
      if (IsRecordable(this.TransactionType, this.DocumentType)) {
        if (newStatus == TransactionStatus.Revision || newStatus == TransactionStatus.OnSign ||
            newStatus == TransactionStatus.Safeguard || newStatus == TransactionStatus.ToDeliver) {
          if (Document.IsEmptyInstance) {
            return "Necesito primero se ingrese la información del documento a inscribir.";
          }
        }
      }
      return String.Empty;
    }

    #endregion Public methods

    #region Private methods

    private void AssertAddItem() {
      Assertion.Assert(this.Status == TransactionStatus.Payment,
              "The transaction is in an status that doesn't permit aggregate new services or products.");
    }

    private void AssertAddPayment() {
      Assertion.Assert(this.Status == TransactionStatus.Payment,
              "The transaction is in an status that doesn't permit aggregate new payments.");
    }

    private string BuildControlNumber() {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        return String.Empty;
      }
      int current = TransactionData.GetLastControlNumber(this.RecorderOffice);

      current++;

      return current.ToString();
    }

    private void Close(TransactionStatus closeStatus, string notes) {
      LRSTransactionTask currentTask = this.GetCurrentTask();

      currentTask.NextStatus = closeStatus;
      currentTask = currentTask.CreateNext(notes);

      ResetTasksList();

      currentTask.Notes = notes;
      currentTask.Close();

      this.LastDeliveryTime = currentTask.EndProcessTime;
      this.Status = closeStatus;
      this.Save();
    }

    static private bool IsArchivable(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Zacatecas") {
        if (docType.Id == 722 || docType.Id == 761) {
          return true;
        }
      }
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 706 && 
           EmpiriaMath.IsMemberOf(docType.Id, new int[] { 733, 734, 736, 737, 738, 739, 740, 
                                                          741, 742, 744, 755, 756 })) {
          return true;
        }
      }
      return false;
    }

    static private bool IsCertificateIssue(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 702) {    // Certificados
          return true;
        } else if (EmpiriaMath.IsMemberOf(docType.Id, new int[] {709, 735, 743, 745, 746, 747 })) {
          return true;
        }
      }
      return false;
    }

    static private bool IsRecordable(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 700 || type.Id == 704 || type.Id == 707) {
          return true;
        } else if (EmpiriaMath.IsMemberOf(docType.Id, new int[] {719, 721, 728, 733, 736, 737, 738, 739,
                                                                 740, 741, 742, 744, 755, 756 })) {
          return true;
        }
      }
      return false;
    }

    private void ResetTasksList() {
      _taskList = new Lazy<LRSTransactionTaskList>(() => LRSTransactionTaskList.Parse(this));
    }

    private void UpdateComplexityIndex() {
      this.ComplexityIndex = 0;
      foreach (LRSTransactionItem act in this.Items) {
        this.ComplexityIndex += act.ComplexityIndex;
      }
    }

    #endregion Private methods

  } // class LRSTransaction

} // namespace Empiria.Land.Registration.Transactions