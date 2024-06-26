﻿/* Empiria Land **********************************************************************************************
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
  internal class Certificate : BaseObject, IResourceTractItem, IProtected {

    #region Constructors and parsers

    private Certificate(CertificateType powerType) : base(powerType) {
      // Required by Empiria Framework for all partitioned types.
    }


    static internal Certificate Parse(int id) {
      return BaseObject.ParseId<Certificate>(id);
    }


    static internal Certificate Parse(string uid) {
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


    [DataField("PropertyId")]
    public Resource OnRecordableSubject {
      get;
      private set;
    }


    //[DataField("OnRecordingId")]
    public LandRecord OnRecording {
      get;
      private set;
    }


    [DataField("OwnerName")]
    public string OnOwnerName {
      get;
      private set;
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
                                           this.OnRecording.Keywords, this.Transaction.UID,
                                           this.Transaction.RequestedBy);
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


    [DataField("PostingTime", Default = "DateTime.Now")]
    public DateTime PostingTime {
      get;
      private set;
    }


    [DataField("CertificateStatus", Default = CertificateStatus.Pending)]
    public CertificateStatus Status {
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
          "OnRecordableSubject", this.OnRecordableSubject.Id,
          "OnRecording", this.OnRecording.Id, "OnOwnerName", this.OnOwnerName,
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
      get;
      internal set;
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


    protected override void OnSave() {
      CertificatesData.WriteCertificate(this);
    }


    internal void SetStatus(CertificateStatus newStatus) {
      EnsureCanChangeStatusTo(newStatus);

      this.Status = newStatus;
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

    #endregion Helpers

  } // class Certificate

} // namespace Empiria.Land.Certificates
