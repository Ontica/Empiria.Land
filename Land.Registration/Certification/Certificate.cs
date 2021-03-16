/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : Certificate                                    Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Holds information about a land certificate.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Text;

using Empiria.Contacts;
using Empiria.Ontology;
using Empiria.Security;

using Empiria.Land.Data;
using Empiria.Land.Providers;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Certification {

  /// <summary>Holds information about a land certificate.</summary>
  [PartitionedType(typeof(CertificateType))]
  public partial class Certificate : BaseObject, IResourceTractItem, IProtected {

    #region Constructors and parsers

    private Certificate(CertificateType powerType) : base(powerType) {
      // Required by Empiria Framework for all partitioned types.
    }

    static public Certificate Parse(int id) {
      return BaseObject.ParseId<Certificate>(id, true);
    }


    static public Certificate ParseGuid(string guid) {
      return BaseObject.TryParse<Certificate>($"CertificateGUID = '{guid}'");
    }


    static public Certificate TryParse(string certificateUID, bool reload = false) {
      return BaseObject.TryParse<Certificate>($"CertificateUID = '{certificateUID}'", reload);
    }


    #endregion Constructors and parsers

    #region Properties

    public CertificateType CertificateType {
      get {
        return (CertificateType) base.GetEmpiriaType();
      }
    }

    [DataField("CertificateGUID", IsOptional = false)]
    public string GUID {
      get;
      private set;
    }


    [DataField("CertificateUID", IsOptional = false)]
    private string _certificateUID = String.Empty;

    public override string UID {
      get {
        return _certificateUID;
      }
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
                                           this.ExtensionData.Keywords);
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


    public Person SignedBy {
      get {
        if (this.UseESign) {
          return CertificatesData.GetDigitalSignatureSignedBy(this);
        } else {
          return Person.Parse(36);
        }
      }
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


    public string StatusName {
      get {
        switch (this.Status) {
          case CertificateStatus.Canceled:
            return "Cancelado";
          case CertificateStatus.Closed:
            return "Cerrado";
          case CertificateStatus.Deleted:
            return "Eliminado";
          case CertificateStatus.Pending:
            return "En elaboración";
          default:
            throw Assertion.AssertNoReachThisCode("Unrecognized certificate status.");
        }
      }
    }


    string IResourceTractItem.TractPrelationStamp {
      get {
        return this.Transaction.PresentationTime.ToString("yyyyMMddTHH:mm@") +
               this.IssueTime.ToString("yyyyMMddTHH:mm@") +
               this.PostingTime.ToString("yyyyMMddTHH:mm") +
               this.Id.ToString("000000000000");
      }
    }


    public bool IsClosed {
      get {
        return this.Status == CertificateStatus.Closed;
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
          "AsText", this.AsText, "IssueTime", this.IssueTime,
          "IssuedById", this.IssuedBy.Id, "IssueMode", (char) this.IssueMode,
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

      this.UserNotes += "Cancelado por " + EmpiriaUser.Current.AsContact().Alias +
                        " el " + DateTime.Now.ToShortDateString() + " a las " +
                        DateTime.Now.ToShortTimeString() + @"\n\n";
      this.Status = CertificateStatus.Canceled;

      this.Save();
    }


    public void Close() {
      Assertion.Assert(this.Status == CertificateStatus.Pending,
                      "This certificate can't be closed. It's not in pending status.");

      this.IssueTime = DateTime.Now;
      this.IssuedBy = EmpiriaUser.Current.AsContact();
      this.Status = CertificateStatus.Closed;

      this.Save();
    }


    public void Delete() {
      Assertion.Assert(this.Status == CertificateStatus.Pending,
                      "This certificate can't be deleted. It's not in pending status.");

      this.UserNotes += "Eliminado por " + EmpiriaUser.Current.AsContact().Alias +
                        " el " + DateTime.Now.ToShortDateString() + " a las " +
                        DateTime.Now.ToShortTimeString() + @"\n\n";
      this.Status = CertificateStatus.Deleted;
      this.Save();
    }


    public string GetDigitalString() {
      if (this.Status == CertificateStatus.Pending) {
        return "* * * * CERTIFICADO EN PROCESO DE ELABORACIÓN * * * *";
      }

      var seal = new StringBuilder("||1|" + this.UID +
                                      "|" + this.Transaction.UID);

      if (!this.Transaction.FormerPaymentOrderData.IsEmptyInstance) {
        seal.Append("|" + this.Transaction.FormerPaymentOrderData.RouteNumber);
      } else {
        seal.Append("|" + this.Transaction.Payments.ReceiptNumbers);
      }

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


    public string GetDigitalSeal() {
      if (this.Status == CertificateStatus.Pending) {
        return "SIN VALOR LEGAL * * * * * SIN VALOR LEGAL";
      }

      string s = Cryptographer.SignTextWithSystemCredentials(this.GetDigitalString());

      int removeThisCharacters = 72;

      if (this.IssueTime < DateTime.Parse("2016-11-20")) {
        return s.Substring(s.Length - removeThisCharacters);
      } else {
        return s.Substring(0, 64);
      }
    }


    public bool UseESign {
      get {
        return this.IssueTime >= DateTime.Parse("2018-07-10");
      }
    }


    public bool Signed() {
      return Data.CertificatesData.IsSigned(this);
    }


    public bool Unsigned() {
      return !Signed();
    }


    public string GetDigitalSignature() {
      if (!UseESign) {
        return "Documento firmado de forma autógrafa. Requiere también sello oficial.";
      }
      if (this.Unsigned()) {
        return "NO SE HA FIRMADO ELECTRÓNICAMENTE";
      } else {
        return Data.CertificatesData.GetDigitalSignature(this)
                                    .Substring(0, 64);
      }
    }


    public void Open() {
      Assertion.Assert(this.Status == CertificateStatus.Closed ||
                       this.Status == CertificateStatus.Canceled ||
                       this.Status == CertificateStatus.Deleted,
                      "This certificate can't be opened. It's not in closed, " +
                      "deleted or canceled status.");

      this.UserNotes += "Abierto por " + EmpiriaUser.Current.AsContact().Alias +
                        " el " + DateTime.Now.ToShortDateString() + " a las " +
                        DateTime.Now.ToShortTimeString() + @"\n\n";

      this.IssueTime = ExecutionServer.DateMaxValue;
      this.IssuedBy = Contact.Empty;
      this.Status = CertificateStatus.Pending;

      this.Save();
    }


    public string QRCodeSecurityHash() {
      if (!this.IsNew) {
        return FormerCryptographer.CreateHashCode(this.Id.ToString("00000000") +
                                                  this.IssueTime.ToString("yyyyMMddTHH:mm"), this.UID)
                                  .Substring(0, 8)
                                  .ToUpperInvariant();
      } else {
        return String.Empty;
      }
    }

    #endregion Public methods

    #region Protected methods

    protected override void OnInitialize() {
      this.ExtensionData = new CertificateExtData();
    }

    protected override void OnLoadObjectData(System.Data.DataRow row) {
      this.ExtensionData = CertificateExtData.Parse((string) row["CertificateExtData"]);
    }

    protected override void OnBeforeSave() {
      if (this.IsNew) {
        this._certificateUID = ExternalProviders.UniqueIDGeneratorProvider.GenerateCertificateUID();
      }
    }

    protected override void OnSave() {
      if (this.IsNew) {
        this.GUID = Guid.NewGuid().ToString().ToLower();
        this.PostingTime = DateTime.Now;
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
      }
      if (this.Status != CertificateStatus.Deleted &&
          this.Status != CertificateStatus.Canceled) {
        this.AsText = CertificateBuilder.Build(this);
      }
      CertificatesData.WriteCertificate(this);
    }

    #endregion Protected methods

  } // class Certificate

} // namespace Empiria.Land.Certification
