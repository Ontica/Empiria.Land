/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Partitioned type                        *
*  Type     : Certificate                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Partitioned type that represents a Land certificate.                                           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Ontology;
using Empiria.Security;

using Empiria.Land.Registration;
using Empiria.Land.Transactions;

using Empiria.Land.Certificates.Data;

namespace Empiria.Land.Certificates {

  /// <summary>Partitioned type that represents a Land certificate.</summary>
  [PartitionedType(typeof(CertificateType))]
  public class Certificate : BaseObject, IResourceTractItem, IProtected {

    #region Constructors and parsers

    private Certificate(CertificateType powerType) : base(powerType) {
      // Required by Empiria Framework for all partitioned types.
    }


    static internal Certificate Parse(int id) {
      return BaseObject.ParseId<Certificate>(id);
    }


    static public Certificate Parse(string uid) {
      return BaseObject.ParseKey<Certificate>(uid);
    }


    static internal Certificate Create(CertificateType certificateType,
                                       LRSTransaction transaction,
                                       Resource onRecordableSubject) {
      Assertion.Require(certificateType, nameof(certificateType));
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(onRecordableSubject, nameof(onRecordableSubject));

      var certificate = new Certificate(certificateType) {
        CertificateID = certificateType.CreateCertificateID(),
        Transaction = transaction,
        OnRecordableSubject = onRecordableSubject
      };

      return certificate;
    }


    static internal Certificate CreateOnRealEstateDescription(CertificateType certificateType,
                                                              LRSTransaction transaction,
                                                              string realEstateDescription) {
      Assertion.Require(certificateType, nameof(certificateType));
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(realEstateDescription, nameof(realEstateDescription));

      var certificate = new Certificate(certificateType) {
        CertificateID = certificateType.CreateCertificateID(),
        Transaction = transaction,
        OnRealEstateDescription = realEstateDescription
      };

      return certificate;
    }


    static internal Certificate CreateOnPersonName(CertificateType certificateType,
                                                   LRSTransaction transaction,
                                                   string onPersonName) {
      Assertion.Require(certificateType, nameof(certificateType));
      Assertion.Require(transaction, nameof(transaction));
      Assertion.Require(onPersonName, nameof(onPersonName));

      var certificate = new Certificate(certificateType) {
        CertificateID = certificateType.CreateCertificateID(),
        Transaction = transaction,
        OnPersonName = onPersonName
      };

      return certificate;
    }

    #endregion Constructors and parsers

    #region Properties

    public CertificateType CertificateType {
      get {
        return (CertificateType) base.GetEmpiriaType();
      }
    }

    [DataField("CertificateUID", IsOptional = false)]
    public string CertificateID {
      get;
      private set;
    }


    [DataField("RecorderOfficeId")]
    public RecorderOffice RecorderOffice {
      get;
      private set;
    }


    [DataField("TransactionId", IsOptional = false)]
    public LRSTransaction Transaction {
      get;
      private set;
    }


    [DataField("OnRecordableSubjectId")]
    public Resource OnRecordableSubject {
      get;
      private set;
    }


    [DataField("OnLandRecordId")]
    public LandRecord OnLandRecord {
      get;
      private set;
    }


    public string OnPersonName {
      get {
        return ExtensionData.Get("onPersonName", string.Empty);
      }
      private set {
        ExtensionData.SetIfValue("onPersonName", EmpiriaString.Clean(value));
      }
    }


    public string OnRealEstateDescription {
      get {
        return ExtensionData.Get("onRealEstateDescription", string.Empty);
      }
      private set {
        ExtensionData.SetIfValue("onRealEstateDescription", EmpiriaString.Clean(value));
      }
    }


    [DataField("CertificateNotes")]
    public string Notes {
      get;
      private set;
    }


    [DataField("CertificateExtData")]
    internal protected JsonObject ExtensionData {
      get;
      private set;
    }


    [DataField("CertificateAsText")]
    public string AsText {
      get;
      private set;
    }


    public string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.UID, this.OnRecordableSubject.UID,
                                           this.OnLandRecord.Keywords, this.OnPersonName,
                                           this.Transaction.Keywords);
      }
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


    [DataField("IssueMode", Default = CertificateIssueMode.Manual)]
    internal CertificateIssueMode IssueMode {
      get;
      private set;
    }


    [DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }


    [DataField("PostingTime")]
    public DateTime PostingTime {
      get;
      private set;
    }


    [DataField("CertificateStatus", Default = CertificateStatus.Pending)]
    public CertificateStatus Status {
      get;
      private set;
    }

    [DataObject]
    public CertificateSecurityData SecurityData {
      get;
      private set;
    }

    public string TractPrelationStamp {
      get {
        return this.Transaction.PresentationTime.ToString("yyyyMMddTHH:mm@") +
               this.IssueTime.ToString("yyyyMMddTHH:mm@") +
               this.Id.ToString("000000000000");
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
      if (version != 1) {
        throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
      }

      return new object[] {
          version, "Id", this.Id, "CertificateTypeId", this.CertificateType.Id,
          "CertificateID", this.CertificateID, "TransactionId", this.Transaction.Id,
          "RecorderOffice", this.RecorderOffice.Id,
          "OnRecordableSubjectId", this.OnRecordableSubject.Id,
          "OnLandRecordId", this.OnLandRecord.Id, "OnPersonName", this.OnPersonName,
          "OnRealEstateDesscripton", this.OnRealEstateDescription,
          "ExtensionData", this.ExtensionData.ToString(), "AsText", this.AsText,
          "IssueTime", this.IssueTime, "IssuedById", this.IssuedBy.Id,
          "IssueMode", (char) this.IssueMode, "PostedBy", this.PostedBy.Id,
          "PostingTime", this.PostingTime, "Status", (char) this.Status
        };
    }


    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    public bool IsClosed {
      get {
        return this.Status == CertificateStatus.Closed;
      }
    }

    #endregion IProtected implementation

    #region Methods

    internal bool CanChangeStatusTo(CertificateStatus newStatus) {
      CertificateStatus currentStatus = this.Status;

      if (currentStatus == CertificateStatus.Pending &&
         newStatus == CertificateStatus.Deleted) {
        return true;
      }
      if (currentStatus == CertificateStatus.Pending &&
          newStatus == CertificateStatus.Closed) {
        return true;
      }
      if (currentStatus == CertificateStatus.Closed &&
          newStatus == CertificateStatus.Pending) {
        return true;
      }
      return false;
    }


    internal void Close() {
      EnsureCanChangeStatusTo(CertificateStatus.Closed);

      this.IssueTime = DateTime.Now;
      this.Status = CertificateStatus.Closed;
    }


    internal void Delete() {
      EnsureCanChangeStatusTo(CertificateStatus.Deleted);

      this.Status = CertificateStatus.Deleted;
    }


    protected override void OnSave() {
      if (IsNew) {
        this.RecorderOffice = Transaction.RecorderOffice;
        this.PostedBy = ExecutionServer.CurrentContact;
        this.PostingTime = DateTime.Now;
      }
      this.AsText = GenerateCertificateText();
      CertificatesData.WriteCertificate(this);
    }


    internal void Open() {
      EnsureCanChangeStatusTo(CertificateStatus.Pending);

      this.IssueTime = ExecutionServer.DateMaxValue;
      this.Status = CertificateStatus.Pending;
    }

    #endregion Methods

    #region Helpers

    private void EnsureCanChangeStatusTo(CertificateStatus newStatus) {
      if (CanChangeStatusTo(newStatus)) {
        return;
      }
      Assertion.RequireFail($"The status of the certificate with ID '{this.CertificateID}' " +
                            $"cannot be changed to {newStatus}.");
    }


    private string GenerateCertificateText() {
      var builder = new CertificateTextBuilder(this);

      return builder.Build();
    }

    #endregion Helpers

  } // class Certificate

} // namespace Empiria.Land.Certificates
