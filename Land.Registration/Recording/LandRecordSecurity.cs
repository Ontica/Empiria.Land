/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording services                      Component : Recording documents                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Separated entity                      *
*  Type     : LandRecordSecurity                           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains security methods used to protect the integrity of recording documents.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Security;

using Empiria.Land.Data;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Contains security methods used to protect the integrity of recording documents.</summary>
  public class LandRecordSecurity: IProtected {

    private readonly LandRecordValidator _landRecordValidator;

    #region Constructors and parsers

    internal LandRecordSecurity(LandRecord landRecord) {
      this.LandRecord = landRecord;

      _landRecordValidator = new LandRecordValidator(landRecord);
    }

    #endregion Constructors and parsers

    #region Public properties

    internal LandRecord LandRecord {
      get;
    }


    #endregion Public properties

    #region Public methods

    public void GenerateImagingControlID() {
      _landRecordValidator.AssertCanGenerateImagingControlID();

      LandRecord.ImagingControlID = LandRecordsData.GetNextImagingControlID(LandRecord);

      LandRecordsData.SaveImagingControlID(LandRecord);
    }


    public bool IsReadyForEdition() {
      if (this.LandRecord.IsEmptyInstance) {
        return false;
      }
      if (this.LandRecord.Status != RecordableObjectStatus.Incomplete) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditLandRecord(this.LandRecord);
    }


    public bool IsReadyToClose() {
      if (this.LandRecord.IsEmptyInstance) {
        return false;
      }
      if (this.LandRecord.Status != RecordableObjectStatus.Incomplete) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditLandRecord(this.LandRecord);
    }


    public bool IsReadyToOpen() {
      if (this.LandRecord.IsEmptyInstance) {
        return false;
      }
      if (!this.LandRecord.IsClosed) {
        return false;
      }
      if (this.LandRecord.SecurityData.IsSigned) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditLandRecord(this.LandRecord);
    }


    public void AssertCanBeClosed() {
      _landRecordValidator.AssertCanBeClosed();
    }


    public void AssertCanBeOpened() {
      _landRecordValidator.AssertCanBeOpened();
    }


    public void ElectronicSign(LandESignData signData) {
      Assertion.Require(signData, nameof(signData));

      _landRecordValidator.AssertCanBeElectronicallySigned();

      this.LandRecord.SecurityData.SetElectronicSignData(signData);

      LandRecordsData.SaveSecurityData(this.LandRecord);
    }


    // Remove after installation
    public void RefreshDIFHash() {
      LandRecordsData.RefreshDIFHash(this.LandRecord);
    }


    public void ManualSign() {
      _landRecordValidator.AssertCanManualSign();

      this.LandRecord.SecurityData.SetManualSignData(this.LandRecord);

      LandRecordsData.SaveSecurityData(this.LandRecord);
    }


    public void PrepareForElectronicSign() {
      _landRecordValidator.AssertCanBeElectronicallySigned();

      this.LandRecord.SecurityData.PrepareForElectronicSign(this.LandRecord);

      LandRecordsData.SaveSecurityData(this.LandRecord);
    }


    public void RemoveManualSign() {
      _landRecordValidator.AssertCanRemoveManualSign();

      this.LandRecord.SecurityData.RemoveManualSignData();

      LandRecordsData.SaveSecurityData(this.LandRecord);
    }


    public void RevokeSign() {
      _landRecordValidator.AssertCanRevokeSign();

      this.LandRecord.SecurityData.RevokeSignData();

      LandRecordsData.SaveSecurityData(this.LandRecord);
    }


    public void SetElectronicSignerData(Person person) {
      _landRecordValidator.AssertCanSetElectronicSigner();

      this.LandRecord.SecurityData.SetElectronicSignerData(person);

      LandRecordsData.SaveSecurityData(this.LandRecord);
    }


    #endregion Public methods

    #region Integrity methods

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }

    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      var doc = this.LandRecord;
      if (version == 1) {
        return new object[] {
          1, "Id", doc.Id, "UID", doc.UID,
          "Instrument", doc.Instrument.Id,
          "Transaction", doc.Transaction.Id,
          "RecorderOffice", doc.RecorderOffice.Id,
          "PresentationTime", doc.PresentationTime,
          "AuthorizationTime", doc.AuthorizationTime,
          "AuthorizedBy", doc.AuthorizedBy.Id,
          "SignedBy", doc.SecurityData.SignedBy.Id,
          "SignedTime", doc.SecurityData.SignedTime,
          "SignStatus", (char) doc.SecurityData.SignStatus,
          "SignType", (char) doc.SecurityData.SignType,
          "SecurityData", doc.SecurityData.ExtData.ToString(),
          "ImagingControlID", doc.ImagingControlID,
          "PostedBy", doc.PostedBy.Id,
          "PostingTime", doc.PostingTime,
          "Status", (char) doc.Status,
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

    #endregion Integrity methods

  } // class LandRecordSecurity

} // namespace Empiria.Land.Registration
