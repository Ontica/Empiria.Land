/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Transactions                      Assembly : Empiria.Land.Registration           *
*  Type      : RecorderOfficeTransaction                      Pattern  : Association Class                   *
*  Version   : 1.5        Date: 28/Mar/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents a transaction or process in a land registration office.                            *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;
using Empiria.Contacts;
using Empiria.Documents.Printing;
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
  public class LRSTransaction : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.LRSTransaction";

    private const string newTransactionKey = "Nuevo trámite";

    private LRSTransactionType transactionType = LRSTransactionType.Empty;
    private string key = newTransactionKey;
    private LRSDocumentType documentType = LRSDocumentType.Empty;
    private string documentNumber = String.Empty;
    private string keywords = String.Empty;
    private RecorderOffice recorderOffice = RecorderOffice.Empty;
    private string requestedBy = String.Empty;
    private Contact managementAgency = Organization.Empty;
    private string contactEMail = String.Empty;
    private string contactPhone = String.Empty;
    private string requestNotes = String.Empty;
    private string receiptNumber = String.Empty;
    private decimal receiptTotal = decimal.Zero;
    private DateTime receiptIssueTime = ExecutionServer.DateMaxValue;
    private DateTime presentationTime = ExecutionServer.DateMaxValue;
    private Contact receivedBy = Person.Empty;
    private string officeNotes = String.Empty;
    private RecordingDocument document = RecordingDocument.Empty;
    private decimal complexityIndex = 0m;
    private DateTime lastReentryTime = ExecutionServer.DateMaxValue;
    private DateTime elaborationTime = ExecutionServer.DateMaxValue;
    private Contact elaboratedBy = Person.Empty;
    private DateTime signTime = ExecutionServer.DateMaxValue;
    private Contact signedBy = Person.Empty;
    private Contact closedBy = Person.Empty;
    private DateTime closingTime = ExecutionServer.DateMaxValue;
    private DateTime lastDeliveryTime = ExecutionServer.DateMaxValue;
    private string deliveryNotes = String.Empty;
    private string closingNotes = String.Empty;
    private int nonWorkingTime = 0;
    private DateTime postingTime = DateTime.Now;
    private Contact postedBy = Person.Empty;
    private TransactionStatus status = TransactionStatus.Payment;
    private string integrityHashCode = String.Empty;

    private ObjectList<LRSTransactionTrack> track = null;
    private ObjectList<LRSTransactionAct> recordingActs = null;
    private LRSFee totalFee = null;

    #endregion Fields

    #region Constuctors and parsers

    protected LRSTransaction()
      : base(thisTypeName) {

    }

    public LRSTransaction(LRSTransactionType transactionType)
      : base(thisTypeName) {
      this.transactionType = transactionType;
    }

    protected LRSTransaction(string typeName)
      : base(typeName) {
      // Required by Empiria Framework. Do not delete. Protected in not sealed classes, private otherwise
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

    static public ObjectList<Contact> GetManagementAgenciesList() {
      GeneralList listType = GeneralList.Parse("LRSTransaction.ManagementAgencies.List");

      return listType.GetContacts<Contact>();
    }

    #endregion Constructors and parsers

    #region Public properties

    public LRSTransactionType TransactionType {
      get { return transactionType; }
    }

    public string Key {
      get { return key; }
    }

    public LRSDocumentType DocumentType {
      get { return documentType; }
      set { documentType = value; }
    }

    public string DocumentNumber {
      get { return documentNumber; }
      set { documentNumber = EmpiriaString.TrimAll(value); }
    }

    public RecorderOffice RecorderOffice {
      get { return recorderOffice; }
      set { recorderOffice = value; }
    }

    public string RequestedBy {
      get { return requestedBy; }
      set { requestedBy = EmpiriaString.TrimAll(value); }
    }

    public Contact ManagementAgency {
      get { return managementAgency; }
      set { managementAgency = value; }
    }

    public string ContactEMail {
      get { return contactEMail; }
      set { contactEMail = value; }
    }

    public string ControlNumber {
      get { return contactPhone; }
      internal set { contactPhone = value; }
    }

    public string ContactPhone {
      get { return contactPhone; }
      set { contactPhone = value; }
    }

    public string RequestNotes {
      get { return requestNotes; }
      set { requestNotes = EmpiriaString.TrimAll(value); }
    }

    public string ReceiptNumber {
      get { return receiptNumber; }
      set { receiptNumber = value; }
    }

    public decimal ReceiptTotal {
      get { return receiptTotal; }
      set { receiptTotal = value; }
    }

    public DateTime ReceiptIssueTime {
      get { return receiptIssueTime; }
      set { receiptIssueTime = value; }
    }

    public DateTime PresentationTime {
      get { return presentationTime; }
    }

    public Contact ReceivedBy {
      get { return receivedBy; }
    }

    public string OfficeNotes {
      get { return officeNotes; }
      set { officeNotes = EmpiriaString.TrimAll(value); }
    }

    public RecordingDocument Document {
      get { return document; }
    }

    public decimal ComplexityIndex {
      get {
        if (complexityIndex == 0) {
          UpdateComplexityIndex();
        }
        return complexityIndex;
      }
    }

    public DateTime LastReentryTime {
      get { return lastReentryTime; }
    }

    public DateTime ElaborationTime {
      get { return elaborationTime; }
    }

    public Contact ElaboratedBy {
      get { return elaboratedBy; }
    }

    public DateTime SignTime {
      get { return signTime; }
    }

    public Contact SignedBy {
      get { return signedBy; }
    }

    public Contact ClosedBy {
      get { return closedBy; }
    }

    public DateTime ClosingTime {
      get { return closingTime; }
    }

    public string ClosingNotes {
      get { return closingNotes; }
    }

    public DateTime LastDeliveryTime {
      get { return lastDeliveryTime; }
    }

    public string DeliveryNotes {
      get { return deliveryNotes; }
      set { deliveryNotes = value; }
    }

    public int NonWorkingTime {
      get { return nonWorkingTime; }
    }

    public DateTime PostingTime {
      get { return postingTime; }
    }

    public Contact PostedBy {
      get { return postedBy; }
    }

    public string Keywords {
      get { return keywords; }
    }

    public TransactionStatus Status {
      get { return status; }
    }

    public string IntegrityHashCode {
      get { return integrityHashCode; }
    }

    public bool ReadyForReentry {
      get {
        var user = EmpiriaUser.Current;
        return ((this.Status == TransactionStatus.Returned) ||
        (this.Status == TransactionStatus.Delivered && user.CanExecute("LRSTransaction.ReentryByFails")));
      }
    }

    public ObjectList<LRSTransactionTrack> Track {
      get {
        if (track == null) {
          track = TransactionData.GetLRSTransactionTrack(this);
        }
        return track;
      }
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

    public static bool StatusIsOfficeWork(TransactionStatus currentStatus) {
      if (currentStatus == TransactionStatus.Payment || currentStatus == TransactionStatus.ToDeliver ||
          currentStatus == TransactionStatus.ToReturn || currentStatus == TransactionStatus.Delivered ||
          currentStatus == TransactionStatus.Returned || currentStatus == TransactionStatus.Finished) {
        return false;
      }
      return true;
    }

    public ObjectList<LRSTransactionAct> RecordingActs {
      get {
        if (this.recordingActs == null) {
          this.recordingActs = TransactionData.GetLRSTransactionActs(this);
        }
        return this.recordingActs;
      }
    }

    public LRSFee TotalFee {
      get {
        if (this.totalFee == null) {
          this.totalFee = LRSFee.Parse(this.RecordingActs);
        }
        return this.totalFee;
      }
    }

    public List<string> GetRecordingActsReceipts() {
      ObjectList<LRSTransactionAct> ra = this.RecordingActs;
      List<string> list = new List<string>();

      for (int i = 0; i < ra.Count; i++) {
        if (!list.Contains(ra[i].ReceiptNumber)) {
          list.Add(ra[i].ReceiptNumber);
        }
      }
      list.Sort();

      return list;
    }

    static public List<TransactionStatus> NextStatusList(LRSTransactionType type, LRSDocumentType docType, TransactionStatus status) {
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
            if (type.Id == 701 || type.Id == 704 || docType.Id == 723 || docType.Id == 734) {    // Certificado || Cancelación || Copia simple
              list.Add(TransactionStatus.Elaboration);
            } else if (type.Id == 700 || type.Id == 702 || type.Id == 703) {
              list.Add(TransactionStatus.Qualification);
              list.Add(TransactionStatus.Recording);
              list.Add(TransactionStatus.Elaboration);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            if (IsRecordable(type, docType)) {
              list.Add(TransactionStatus.Recording);
            } else if (IsCertificateIssue(type, docType)) {
              list.Add(TransactionStatus.Elaboration);
            } else {
              list.Add(TransactionStatus.Elaboration);
              list.Add(TransactionStatus.Recording);
            }
            if (IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            list.Add(TransactionStatus.Juridic);
          }
          list.Add(TransactionStatus.Revision);
          list.Add(TransactionStatus.OnSign);
          if (ExecutionServer.LicenseName == "Tlaxcala" && IsRecordable(type, docType)) {
            list.Add(TransactionStatus.Safeguard);
          }
          list.Add(TransactionStatus.ToReturn);
          if (ExecutionServer.LicenseName == "Zacatecas" || IsCertificateIssue(type, docType)) {
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
            if (IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            if (docType.Id == 728) {
              list.Add(TransactionStatus.OnSign);
            }
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(TransactionStatus.Juridic);
            if (IsArchivable(type, docType)) {
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
            if (IsArchivable(type, docType)) {
              list.Add(TransactionStatus.Finished);
            }
            list.Add(TransactionStatus.Revision);
            list.Add(TransactionStatus.Control);
            list.Add(TransactionStatus.ToReturn);
          } else if (ExecutionServer.LicenseName == "Tlaxcala") {
            list.Add(TransactionStatus.OnSign);
            list.Add(TransactionStatus.Control);
            if (IsArchivable(type, docType)) {
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
            if (IsRecordable(type, docType)) {
              list.Add(TransactionStatus.Safeguard);
            } else if (IsCertificateIssue(type, docType)) {
              list.Add(TransactionStatus.ToDeliver);
            }
            list.Add(TransactionStatus.ToReturn);
            list.Add(TransactionStatus.Control);
            list.Add(TransactionStatus.Juridic);

            //list.Add(TransactionStatus.ToDeliver);
            //list.Add(TransactionStatus.ToReturn);
            //list.Add(TransactionStatus.Control);
            //list.Add(TransactionStatus.Juridic);
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

    static private bool IsArchivable(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Zacatecas") {
        if (docType.Id == 722 || docType.Id == 761) {
          return true;
        }
      }
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 706 && (docType.Id == 733 || docType.Id == 734 || docType.Id == 736 || docType.Id == 737 ||
                               docType.Id == 738 || docType.Id == 739 || docType.Id == 740 || docType.Id == 741 ||
                               docType.Id == 742 || docType.Id == 744 || docType.Id == 755 || docType.Id == 756)) {
          return true;
        }
      }
      return false;
    }

    static private bool IsRecordable(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 700 || type.Id == 704 || type.Id == 707) {
          return true;
        } else if (docType.Id == 719 || docType.Id == 721 || docType.Id == 728 || docType.Id == 733 || docType.Id == 736 ||
                   docType.Id == 737 || docType.Id == 738 || docType.Id == 739 || docType.Id == 740 || docType.Id == 741 ||
                   docType.Id == 742 || docType.Id == 744 || docType.Id == 755 || docType.Id == 756) {
          return true;
        }
      }
      return false;
    }

    static private bool IsCertificateIssue(LRSTransactionType type, LRSDocumentType docType) {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        if (type.Id == 702) {    // Certificados
          return true;
        } else if (docType.Id == 709 || docType.Id == 735 || docType.Id == 743 ||
                   docType.Id == 745 || docType.Id == 746 || docType.Id == 747) {
          return true;
        }
      }
      return false;
    }

    #endregion Public properties

    #region Public methods

    public void AttachDocument(RecordingDocument document) {
      Assertion.AssertObject(document, "document");

      document.Save();
      this.document = document;
      this.Save();
    }

    public string GetDigitalSign() {
      return Empiria.Security.Cryptographer.CreateDigitalSign(GetDigitalString());
    }

    public string GetDigitalString() {
      string temp = String.Empty;

      temp = "||" + this.Id.ToString() + "|" + this.Key + "|";
      if (this.PresentationTime != ExecutionServer.DateMaxValue) {
        temp += this.PresentationTime.ToString("yyyyMMddTHH:mm:ss") + "|";
        temp += this.ReceiptNumber + "|" + this.ReceiptTotal.ToString("F2") + "|";
      } else {
        temp += this.PostingTime.ToString("yyyyMMddTHH:mm:ss") + "|";
      }
      temp += this.PostedBy.Id.ToString() + "|" + this.PostingTime.ToString("yyyyMMddTHH:mm");

      foreach (LRSTransactionAct act in this.RecordingActs) {
        temp += "|" + act.Id.ToString() + "^" + act.RecordingActType.Id.ToString() + "^";
        if (act.OperationValue.Amount != decimal.Zero) {
          temp += "B" + act.OperationValue.Amount.ToString("F4") + "^";
        }
        if (!act.Unit.IsEmptyInstance) {
          temp += "Q" + act.Quantity.ToString("N2") + "^";
          temp += "U" + act.Unit.Id.ToString() + "^";
        }
        temp += act.LawArticle.Id.ToString();
        if (ExecutionServer.LicenseName == "Tlaxcala") {
          temp += "^" + "S" + act.Fee.SubTotal.ToString("N2") + "^";
          temp += "D" + act.Fee.Discount.ToString("N2") + "^";
          temp += "T" + act.Fee.Total.ToString("N2");
        }
      }
      temp += "||";

      return temp;
    }

    public LRSTransactionTrack GetLastTransactionTack() {
      return TransactionData.GetLastTransactionTrack(this);
    }

    public void PrintTicket() {
      string TicketPrinterName = ConfigurationData.GetString("Ticket.PrinterName");
      string TicketDefaultFontName = ConfigurationData.GetString("Ticket.DefaultFontName");
      int TicketDefaultFontSize = ConfigurationData.GetInteger("Ticket.DefaultFontSize");
      string ReportsTemplatesPath = ConfigurationData.GetString("Reports.TemplatesPath");

      Document document = new Document(TicketDefaultFontName, TicketDefaultFontSize);

      document.LoadTemplate(ReportsTemplatesPath + "transaction.ticket.ert");

      this.FillPrinterDocument(document);

      Ticket ticket = new Ticket();
      ticket.Load(document);
      ticket.Print(TicketPrinterName);
    }

    public void DoReentry(string notes) {
      if (!this.ReadyForReentry) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction, this.Key);
      }
      this.status = TransactionStatus.Reentry;
      this.signedBy = Person.Empty;
      this.signTime = ExecutionServer.DateMaxValue;
      this.closedBy = Person.Empty;
      this.closingTime = ExecutionServer.DateMaxValue;
      this.lastReentryTime = DateTime.Now;
      LRSTransactionTrack lastTrack = this.GetLastTransactionTack();
      lastTrack.NextStatus = TransactionStatus.Reentry;
      lastTrack = lastTrack.CreateNext(notes);
      lastTrack.NextStatus = TransactionStatus.Control;
      lastTrack.Status = TrackStatus.OnDelivery;
      lastTrack.EndProcessTime = lastTrack.CheckInTime;
      lastTrack.AssignedBy = LRSTransaction.InterestedContact;
      lastTrack.Save();
      this.Save();
      this.ResetTrack();
    }

    public void SetNextStatus(TransactionStatus nextStatus, Contact nextContact, string notes) {
      if (nextStatus == TransactionStatus.Returned || nextStatus == TransactionStatus.Delivered || nextStatus == TransactionStatus.Finished) {
        this.Close(nextStatus, notes);
        return;
      }
      LRSTransactionTrack lastTrack = this.GetLastTransactionTack();

      lastTrack.SetNextStatus(nextStatus, nextContact, notes);

      if (nextStatus == TransactionStatus.OnSign || nextStatus == TransactionStatus.Revision) {
        this.elaboratedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.elaborationTime = DateTime.Now;
        this.Save();
      } else if (this.Status == TransactionStatus.OnSign &&
                (nextStatus == TransactionStatus.ToDeliver || nextStatus == TransactionStatus.Finished)) {
        this.signedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.signTime = DateTime.Now;
        this.Save();
      }
    }

    public void Receive(string notes) {
      if (this.Status != TransactionStatus.Payment) {
        throw new LandRegistrationException(LandRegistrationException.Msg.CantReEntryTransaction, this.Key);
      }
      if (this.ReceiptNumber.Length == 0) {
        throw new NotImplementedException("Este trámite todavía no tiene número de recibo.");
      }

      this.receivedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      this.presentationTime = DateTime.Now;
      this.signedBy = Person.Empty;
      this.signTime = ExecutionServer.DateMaxValue;
      this.closedBy = Person.Empty;
      this.closingTime = ExecutionServer.DateMaxValue;
      this.status = TransactionStatus.Received;
      if (this.ControlNumber.Length == 0) {
        this.ControlNumber = BuildControlNumber();
      }

      LRSTransactionTrack lastTrack = this.GetLastTransactionTack();
      lastTrack.NextStatus = TransactionStatus.Received;
      lastTrack.NextContact = LRSTransaction.InterestedContact;
      lastTrack = lastTrack.CreateNext(notes);
      lastTrack.NextStatus = TransactionStatus.Control;
      lastTrack.Status = TrackStatus.OnDelivery;
      lastTrack.EndProcessTime = lastTrack.CheckInTime;
      lastTrack.AssignedBy = LRSTransaction.InterestedContact;
      lastTrack.Save();

      this.Save();
      this.ResetTrack();
    }

    public void Take(string notes) {
      LRSTransactionTrack lastTrack = this.GetLastTransactionTack();

      if (lastTrack.NextStatus == TransactionStatus.EndPoint) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint, lastTrack.Id);
      }
      this.status = lastTrack.NextStatus;
      lastTrack.CreateNext(notes);
      ResetTrack();

      if (this.Status == TransactionStatus.ToDeliver || this.Status == TransactionStatus.ToReturn) {
        this.closingTime = lastTrack.EndProcessTime;
        this.closedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.closingNotes = EmpiriaString.TrimAll(notes);
      }

      this.Save();
    }

    public void Undelete() {
      LRSTransactionTrack lastTrack = this.GetLastTransactionTack();

      if (lastTrack.Status == TrackStatus.OnDelivery) {
        this.status = lastTrack.CurrentStatus;
      } else if (lastTrack.Status == TrackStatus.Pending) {
        this.status = lastTrack.CurrentStatus;
      } else if (lastTrack.Status == TrackStatus.Closed) {
        throw new LandRegistrationException(LandRegistrationException.Msg.NextStatusCantBeEndPoint, lastTrack.Id);
      }
      this.Save();
    }

    public void ReturnToMe() {
      LRSTransactionTrack lastTrack = this.GetLastTransactionTack();

      TransactionStatus nextStatus = lastTrack.NextStatus;
      if (nextStatus == TransactionStatus.OnSign || nextStatus == TransactionStatus.Revision) {
        this.elaboratedBy = Person.Empty;
        this.elaborationTime = ExecutionServer.DateMaxValue;
      } else if (this.Status == TransactionStatus.OnSign &&
                (nextStatus == TransactionStatus.ToDeliver || nextStatus == TransactionStatus.Finished)) {
        this.signedBy = Person.Empty;
        this.signTime = ExecutionServer.DateMaxValue;
      }
      lastTrack.SetPending();

      this.Save();
      ResetTrack();
    }

    internal void OnRecordingActsUpdated() {
      recordingActs = null;
      totalFee = null;
      complexityIndex = 0;
    }

    protected override void ImplementsLoadObjectData(DataRow row) {
      this.transactionType = LRSTransactionType.Parse((int) row["TransactionTypeId"]);
      this.key = (string) row["TransactionKey"];
      this.documentType = LRSDocumentType.Parse((int) row["DocumentTypeId"]);
      this.documentNumber = (string) row["DocumentNumber"];
      this.keywords = (string) row["TransactionKeywords"];
      this.recorderOffice = RecorderOffice.Parse((int) row["RecorderOfficeId"]);
      this.requestedBy = (string) row["RequestedBy"];
      this.managementAgency = Contact.Parse((int) row["ManagementAgencyId"]);
      this.contactEMail = (string) row["ContactEMail"];
      this.contactPhone = (string) row["ContactPhone"];
      this.requestNotes = (string) row["RequestNotes"];
      this.receiptNumber = (string) row["ReceiptNumber"];
      this.receiptTotal = (decimal) row["ReceiptTotal"];
      this.receiptIssueTime = (DateTime) row["ReceiptIssueTime"];
      this.presentationTime = (DateTime) row["PresentationTime"];
      this.receivedBy = Contact.Parse((int) row["ReceivedById"]);
      this.officeNotes = (string) row["OfficeNotes"];
      this.document = RecordingDocument.Parse((int) row["DocumentId"]);
      this.complexityIndex = (decimal) row["ComplexityIndex"];
      this.lastReentryTime = (DateTime) row["LastReentryTime"];
      this.elaborationTime = (DateTime) row["ElaborationTime"];
      this.elaboratedBy = Contact.Parse((int) row["ElaboratedById"]);
      this.signTime = (DateTime) row["SignTime"];
      this.signedBy = Contact.Parse((int) row["SignedById"]);
      this.closedBy = Contact.Parse((int) row["ClosedById"]);
      this.closingTime = (DateTime) row["ClosingTime"];
      this.closingNotes = (string) row["ClosingNotes"];
      this.lastDeliveryTime = (DateTime) row["LastDeliveryTime"];
      this.deliveryNotes = (string) row["DeliveryNotes"];
      this.nonWorkingTime = (int) row["NonWorkingTime"];
      this.postingTime = (DateTime) row["PostingTime"];
      this.postedBy = Contact.Parse((int) row["PostedById"]);
      this.status = (TransactionStatus) Convert.ToChar(row["TransactionStatus"]);
      this.integrityHashCode = (string) row["TransactionRIHC"];
    }

    protected override void ImplementsSave() {
      bool isnew = this.postedBy.IsEmptyInstance;
      PrepareForSave();
      TransactionData.WriteTransaction(this);
      if (isnew) {
        LRSTransactionTrack track = LRSTransactionTrack.CreateFirst(this);
        ResetTrack();
      }
    }

    internal void PrepareForSave() {
      bool isnew = this.postedBy.IsEmptyInstance;
      if (isnew) {
        this.key = TransactionData.GenerateTransactionKey();
        this.postingTime = DateTime.Now;
        this.postedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      this.keywords = EmpiriaString.BuildKeywords(this.Key, this.ReceiptNumber, this.Document.DocumentKey, this.ControlNumber, this.DocumentNumber, this.RequestedBy,
                                                  this.ManagementAgency.FullName, this.receiptTotal.ToString("N2"), this.requestNotes, this.TransactionType.Name,
                                                  this.RecorderOffice.Alias, this.OfficeNotes);

      this.integrityHashCode = EmpiriaString.BuildDigitalString(this.Id, this.TransactionType.Id, this.Key, this.DocumentType.Id, this.DocumentNumber,
                                                                this.RecorderOffice.Id, this.RequestedBy, this.ManagementAgency.Id,
                                                                this.ContactEMail, this.ContactPhone, this.RequestNotes,
                                                                this.ReceiptNumber, this.ReceiptTotal, this.ReceiptIssueTime,
                                                                this.PresentationTime, this.ReceivedBy.Id, this.OfficeNotes, this.ComplexityIndex,
                                                                this.LastReentryTime, this.ElaborationTime, this.ElaboratedBy.Id, this.SignTime, this.SignedBy.Id,
                                                                this.ClosedBy.Id, this.ClosingTime, this.ClosingNotes, this.LastDeliveryTime, this.DeliveryNotes,
                                                                this.NonWorkingTime, this.PostingTime, this.PostedBy.Id, this.Status);

    }

    public string ValidateStatusChange(TransactionStatus newStatus) {
      if (newStatus == TransactionStatus.Received) {
        if (this.ReceiptNumber.Length == 0) {
          return "Este trámite todavía no tiene número de recibo.";
        }
      }
      if (IsRecordable(this.transactionType, this.documentType)) {
        if (this.transactionType.Id == 704 || this.documentType.Id == 721) {
          return String.Empty;
        }
      }
      if (IsRecordable(this.transactionType, this.documentType)) {
        if (newStatus == TransactionStatus.Revision || newStatus == TransactionStatus.OnSign ||
            newStatus == TransactionStatus.Safeguard || newStatus == TransactionStatus.ToDeliver) {
          if (document.IsEmptyInstance) {
            return "Necesito que primero se ingrese la información del documento a inscribir.";
          }
        }
      }
      return String.Empty;
    }

    #endregion Public methods

    #region Private methods

    private string BuildControlNumber() {
      if (ExecutionServer.LicenseName == "Tlaxcala") {
        return String.Empty;
      }
      int current = TransactionData.GetLastControlNumber(this.recorderOffice);

      current++;

      return current.ToString();
    }

    private void Close(TransactionStatus closeStatus, string notes) {
      LRSTransactionTrack lastTrack = this.GetLastTransactionTack();

      lastTrack.NextStatus = closeStatus;
      lastTrack = lastTrack.CreateNext(notes);

      ResetTrack();

      lastTrack.Notes = notes;
      lastTrack.Close();

      this.lastDeliveryTime = lastTrack.EndProcessTime;
      this.deliveryNotes = notes;
      this.status = closeStatus;
      this.Save();
    }

    private void FillPrinterDocument(Document document) {
      string temp = this.RequestedBy;
      string[] vector = EmpiriaString.DivideLongString(temp, 29, 2);
      document.Replace("<@REQUESTED_BY_1@>", vector[0]);
      document.Replace("<@REQUESTED_BY_2@>", vector.Length > 1 ? vector[1] : String.Empty);
      document.Replace("<@REQUESTED_BY_3@>", vector.Length > 2 ? vector[2] : String.Empty);
      document.Replace("<@REQUESTED_BY_4@>", vector.Length > 3 ? vector[3] : String.Empty);
      document.Replace("<@REQUESTED_BY_5@>", vector.Length > 4 ? vector[4] : String.Empty);
      document.Replace("<@TRANSACTION_NUMBER@>", this.Key);
      document.Replace("<@POSTED_BY@>", this.PostedBy.Alias);
      document.Replace("<@DATE_TIME@>", DateTime.Now.ToString("dd/MMM/yyyy HH:mm"));

      document.Replace("<@TOTAL@>", this.ReceiptTotal.ToString("C2"));
      temp = EmpiriaString.SpeechMoney(this.ReceiptTotal);
      temp = temp.Replace(" 00/100 M.N.", String.Empty);
      vector = EmpiriaString.DivideLongString(temp, 40, 2);
      document.Replace("<@TOTAL_STRING_1@>", vector[0]);
      document.Replace("<@TOTAL_STRING_2@>", vector.Length > 1 ? vector[1] : String.Empty);
      document.Replace("<@TOTAL_STRING_3@>", vector.Length > 2 ? vector[2] : String.Empty);
      document.Replace("<@TOTAL_STRING_4@>", vector.Length > 3 ? vector[3] : String.Empty);


      temp = "||" + this.Id.ToString() + "|" + this.Key + "|" + this.ReceiptTotal.ToString("N2") + "||";
      document.Replace("<@DIGITAL_STRING@>", temp);
      temp = Empiria.Security.Cryptographer.CreateDigitalSign(temp);
      document.Replace("<@DIGITAL_SIGNATURE_1@>", temp.Substring(0, Math.Min(temp.Length, 32)));
      document.Replace("<@DIGITAL_SIGNATURE_2@>", String.Empty);
      document.Replace("<@DIGITAL_SIGNATURE_3@>", String.Empty);
      document.Replace("<@DIGITAL_SIGNATURE_4@>", String.Empty);

      document.BarcodeString = this.Key;
    }

    private void ResetTrack() {
      this.track = null;
    }

    private void UpdateComplexityIndex() {
      complexityIndex = 0;
      foreach (LRSTransactionAct act in this.RecordingActs) {
        complexityIndex += act.ComplexityIndex;
      }
    }

    #endregion Private methods

  } // class LRSTransaction

} // namespace Empiria.Land.Registration.Transactions
