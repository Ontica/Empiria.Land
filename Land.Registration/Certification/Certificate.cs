/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : Certificate                                    Pattern  : Empiria Object Type                 *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Certificate emission and information search acts.                                             *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Contacts;
using Empiria.Ontology;
using Empiria.Security;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Certification {

  /// <summary>Represents a certificate for property ownership or non ownership.</summary>
  [PartitionedType(typeof(CertificateType))]
  public partial class Certificate : BaseObject, IProtected {

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
    public Property Property {
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

    private string Keywords {
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
    private CertificateIssueMode IssueMode {
      get;
      set;
    }

    [DataField("PostedById")]
    private Contact PostedBy {
      get;
      set;
    }

    [DataField("PostingTime", Default = "DateTime.Now")]
    private DateTime PostingTime {
      get;
      set;
    }

    [DataField("CertificateStatus", Default = CertificateStatus.Pending)]
    public CertificateStatus Status {
      get;
      private set;
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

      //return this.Status == CertificateStatus.Closed &&
      //      EmpiriaUser.Current.Id == 19;
    }

    public bool CanDelete() {
      return this.Status == CertificateStatus.Pending;
    }

    public bool CanOpen() {
      return this.Status == CertificateStatus.Closed;

      //return this.Status == CertificateStatus.Closed &&
      //      EmpiriaUser.Current.Id == 19;
    }

    public void Cancel() {
      //Assertion.Assert(EmpiriaUser.Current.Id == 19,
      //                 "The certificate can be canceled only by an authorized user.");
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

    public void Open() {
      //Assertion.Assert(EmpiriaUser.Current.Id == 19,
      //                 "The certificate can be opened only by an authorized user.");
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
        this.UID = Certificate.BuildCertificateUID();
      }
      if (this.Status != CertificateStatus.Deleted ||
          this.Status != CertificateStatus.Canceled) {
        this.AsText = CertificateBuilder.Build(this);
      }
      this.Write();
    }

    #endregion Protected methods

    #region Private methods

    static private string BuildCertificateUID() {
      string temp = String.Empty;
      int hashCode = 0;
      bool useLetters = false;
      for (int i = 0; i < 7; i++) {
        if (useLetters) {
          temp += EmpiriaMath.GetRandomCharacter(temp);
          temp += EmpiriaMath.GetRandomCharacter(temp);
        } else {
          temp += EmpiriaMath.GetRandomDigit(temp);
          temp += EmpiriaMath.GetRandomDigit(temp);
        }
        hashCode += ((Convert.ToInt32(temp[temp.Length - 2]) +
                      Convert.ToInt32(temp[temp.Length - 1])) % ((int) Math.Pow(i + 1, 2)));
        useLetters = !useLetters;
      }
      string prefix = ExecutionServer.LicenseName == "Zacatecas" ? "ZS" : "TL";
      temp = "CE" + temp.Substring(0, 4) + "-" + temp.Substring(4, 6) + "-" + temp.Substring(10, 4);

      temp += "ABCDEFHJKMNPRTWXYZ".Substring((hashCode * Convert.ToInt32(prefix[0])) % 17, 1);
      temp += "9A8B7CD5E4F2".Substring((hashCode * Convert.ToInt32(prefix[1])) % 11, 1);

      return temp;
    }

    private void Write() {
      var op = Empiria.Data.DataOperation.Parse("writeLRSCertificate",
                            Id, CertificateType.Id, UID,
                            Transaction.Id, RecorderOffice.Id, Property.Id, OwnerName,
                            UserNotes, ExtensionData.ToJson(), AsText, Keywords,
                            IssueTime, IssuedBy.Id, SignedBy.Id, (char) IssueMode, PostedBy.Id,
                            PostingTime, (char) Status, Integrity.GetUpdatedHashCode());

      Empiria.Data.DataWriter.Execute(op);
    }

    #endregion Private methods

  } // class Certificate

} // namespace Empiria.Land.Certification
