/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : Certificate                                    Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Certificate emission and information search acts.                                             *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Text;

using Empiria.Contacts;
using Empiria.Ontology;
using Empiria.Security;

using Empiria.Land.Data;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Certification {

  /// <summary>Represents a certificate for property ownership or non ownership.</summary>
  [PartitionedType(typeof(CertificateType))]
  public partial class Certificate : BaseObject, IResourceTractItem, IProtected {

    #region Constructors and parsers

    private Certificate(CertificateType powerType) : base(powerType) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public Certificate Parse(int id) {
      return BaseObject.ParseId<Certificate>(id);
    }

    static public Certificate TryParse(string certificateUID) {
      return BaseObject.TryParse<Certificate>("CertificateUID = '" + certificateUID + "'");
    }

    #endregion Constructors and parsers

    #region Properties

    public CertificateType CertificateType {
      get {
        return (CertificateType) base.GetEmpiriaType();
      }
    }

    [DataField("CertificateUID", IsOptional = false)]
    public string UID {
      get;
      private set;
    }

    [DataField("TransactionId", IsOptional = false)]
    public LRSTransaction Transaction {
      get;
      private set;
    }

    [DataField("RecorderOfficeId")]
    public RecorderOffice RecorderOffice {
      get;
      private set;
    }

    [DataField("PropertyId")]
    public RealEstate Property {
      get;
      private set;
    }

    [DataField("OwnerName")]
    public string OwnerName {
      get;
      private set;
    }

    [DataField("CertificateAsText")]
    public string AsText {
      get;
      private set;
    }

    public CertificateExtData ExtensionData {
      get;
      private set;
    }

    internal string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.UID, this.Property.UID,
                                           this.OwnerName, this.Transaction.UID,
                                           this.Transaction.RequestedBy,
                                           this.Property.AsText, this.ExtensionData.Keywords);
      }
    }

    [DataField("CertificateNotes")]
    public string UserNotes {
      get;
      private set;
    }

    [DataField("IssueTime", Default = "ExecutionServer.DateMaxValue")]
    public DateTime IssueTime {
      get;
      private set;
    }

    [DataField("IssuedById")]
    public Contact IssuedBy {
      get;
      private set;
    }

    [DataField("SignedById")]
    public Contact SignedBy {
      get;
      private set;
    }

    public bool IsSigned {
      get {
        return !this.SignedBy.IsEmptyInstance;
      }
    }

    [DataField("IssueMode", Default = CertificateIssueMode.Manual)]
    internal CertificateIssueMode IssueMode {
      get;
      private set;
    }

    [DataField("PostedById")]
    internal Contact PostedBy {
      get;
      private set;
    }

    [DataField("PostingTime", Default = "DateTime.Now")]
    internal DateTime PostingTime {
      get;
      private set;
    }

    [DataField("CertificateStatus", Default = CertificateStatus.Pending)]
    public CertificateStatus Status {
      get;
      private set;
    }

    string IResourceTractItem.TractPrelationStamp {
      get {
        return this.Transaction.PresentationTime.ToString("yyyyMMddTHH:mm@") +
               this.IssueTime.ToString("yyyyMMddTHH:mm@") +
               this.PostingTime.ToString("yyyyMMddTHH:mm");
      }
    }

    #endregion Properties

    #region IProtected implementation

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    private IntegrityValidator _validator = null;
    IntegrityValidator IProtected.Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          version, "Id", this.Id, "CertificateTypeId", this.CertificateType.Id,
          "UID", this.UID, "TransactionId", this.Transaction.Id,
          "RecorderOffice", this.RecorderOffice.Id, "PropertyId", this.Property.Id,
          "OwnerName", this.OwnerName, "ExtensionData", this.ExtensionData.ToJson(),
          "AsText", this.AsText, "IssueTime", this.IssueTime, "IssuedById", this.IssuedBy.Id,
          "SignedBy", this.SignedBy.Id, "IssueMode", (char) this.IssueMode,
          "PostedBy", this.PostedBy.Id, "PostingTime", this.PostingTime,
          "Status", (char) this.Status
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }

    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    #endregion IProtected implementation

    #region Public methods

    public bool CanCancel() {
      return this.Status == CertificateStatus.Closed;
    }

    public bool CanDelete() {
      return this.Status == CertificateStatus.Pending;
    }

    public bool CanOpen() {
      return this.Status == CertificateStatus.Closed;
    }

    public void Cancel() {
      Assertion.Assert(this.Status == CertificateStatus.Closed,
                      "The certificate is not closed so it can't be canceled. Use delete instead.");

      this.UserNotes += "Cancelado por " + Contact.Parse(EmpiriaUser.Current.Id).Alias +
                        " el " + DateTime.Now.ToShortDateString() + " a las " +
                        DateTime.Now.ToShortTimeString() + @"\n\n";
      this.Status = CertificateStatus.Canceled;

      this.Save();
    }

    public void Close() {
      Assertion.Assert(this.Status == CertificateStatus.Pending,
                      "This certificate can't be closed. It's not in pending status.");

      this.IssueTime = DateTime.Now;
      this.IssuedBy = Contact.Parse(EmpiriaUser.Current.Id);
      this.SignedBy = Contact.Parse(36);
      this.Status = CertificateStatus.Closed;

      this.Save();
    }

    public string GetDigitalSeal() {
      if (this.Status == CertificateStatus.Pending) {
        return "* * * * CERTIFICADO EN PROCESO DE ELABORACIÓN * * * *";
      }

      var seal = new StringBuilder("||1|" + this.UID +
                                      "|" + this.Transaction.UID);
      seal.Append("|" + this.Transaction.Payments.ReceiptNumbers);
      if (!this.Property.IsEmptyInstance) {
        seal.Append("|" + this.Property.UID);
      } else if (this.OwnerName.Length != 0) {
        seal.Append("|" + this.OwnerName.ToUpperInvariant());
      }
      if (this.ExtensionData.UseMarginalNotesAsFullBody) {
        seal.Append("|" + "Manual");
      }
      seal.Append("|" + this.IssueTime.ToString("yyyyMMddTHH:mm"));
      seal.Append("|" + this.SignedBy.Id + "|" + this.IssuedBy.Id);
      seal.Append("|" +
            this.Integrity.GetUpdatedHashCode().Substring(0, 12).ToUpperInvariant() + "||");

      return EmpiriaString.DivideLongString(seal.ToString(), 72, "&#8203;");
    }

    public string GetDigitalSignature() {
      if (this.Status == CertificateStatus.Pending) {
        return "SIN VALOR LEGAL * * * * * SIN VALOR LEGAL";
      }

      string s = Cryptographer.CreateDigitalSign(this.GetDigitalSeal());

      int removeThisCharacters = 72;

      return s.Substring(s.Length - removeThisCharacters);
    }

    public void Open() {
      Assertion.Assert(this.Status == CertificateStatus.Closed ||
                       this.Status == CertificateStatus.Canceled ||
                       this.Status == CertificateStatus.Deleted,
                      "This certificate can't be opened. It's not in closed, " +
                      "deleted or canceled status.");

      this.UserNotes += "Abierto por " + Contact.Parse(EmpiriaUser.Current.Id).Alias +
                        " el " + DateTime.Now.ToShortDateString() + " a las " +
                        DateTime.Now.ToShortTimeString() + @"\n\n";

      this.IssueTime = ExecutionServer.DateMaxValue;
      this.IssuedBy = Contact.Empty;
      this.SignedBy = Contact.Empty;
      this.Status = CertificateStatus.Pending;

      this.Save();
    }

    public void Delete() {
      Assertion.Assert(this.Status == CertificateStatus.Pending,
                      "This certificate can't be deleted. It's not in pending status.");

      this.UserNotes += "Eliminado por " + Contact.Parse(EmpiriaUser.Current.Id).Alias +
                        " el " + DateTime.Now.ToShortDateString() + " a las " +
                        DateTime.Now.ToShortTimeString() + @"\n\n";
      this.Status = CertificateStatus.Deleted;
      this.Save();
    }

    #endregion Public methods

    #region Protected methods

    protected override void OnInitialize() {
      this.ExtensionData = new CertificateExtData();
    }

    protected override void OnLoadObjectData(System.Data.DataRow row) {
      this.ExtensionData = CertificateExtData.Parse((string) row["CertificateExtData"]);
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.PostingTime = DateTime.Now;
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.UID = CertificatesData.BuildCertificateUID();
      }
      if (this.Status != CertificateStatus.Deleted ||
          this.Status != CertificateStatus.Canceled) {
        this.AsText = CertificateBuilder.Build(this);
      }
      CertificatesData.WriteCertificate(this);
    }

    #endregion Protected methods

  } // class Certificate

} // namespace Empiria.Land.Certification
