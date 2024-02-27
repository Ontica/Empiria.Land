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

using Empiria.Security;

using Empiria.Land.Data;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Contains security methods used to protect the integrity of recording documents.</summary>
  public class LandRecordSecurity: IProtected {

    #region Constructors and parsers

    private LandRecordSecurity() {
      // Used by Empiria Framework
    }

    internal LandRecordSecurity(LandRecord landRecord) {
      this.LandRecord = landRecord;
    }

    #endregion Constructors and parsers

    #region Public properties

    internal LandRecord LandRecord {
      get;
    }


    #endregion Public properties

    #region Public methods

    public void GenerateImagingControlID() {
      Assertion.Require(!LandRecord.IsEmptyInstance, "Land record can't be the empty instance.");
      Assertion.Require(LandRecord.IsClosed, "Document is not closed.");

      Assertion.Require(LandRecord.ImagingControlID.Length == 0,
                        "Land record has already assigned an imaging control number.");

      Assertion.Require(LandRecord.RecordingActs.Count > 0, "Land record should have recording acts.");
      Assertion.Require(LandRecord.RecordingActs.CountAll((x) => !x.BookEntry.IsEmptyInstance) == 0,
                        "Land record can't have any recording acts that are related to physical book entries.");


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
      if (!this.IsReadyToClose()) {
        Assertion.RequireFail("El usuario no tiene permisos para cerrar la inscripción o ésta no tiene un estado válido.");
      }

      //this.AssertGraceDaysForEdition();

      Assertion.Require(this.LandRecord.RecordingActs.Count > 0, "La inscripción no tiene actos jurídicos.");

      foreach (var recordingAct in this.LandRecord.RecordingActs) {
        recordingAct.Validator.AssertCanBeClosed();
      }
    }


    public void AssertCanBeOpened() {
      if (!this.IsReadyToOpen()) {
        Assertion.RequireFail("El usuario no tiene permisos para abrir esta inscripción.");
      }

      //this.AssertGraceDaysForEdition();

      foreach (var recordingAct in this.LandRecord.RecordingActs) {
        recordingAct.Validator.AssertCanBeOpened();
      }
    }

    private void AssertGraceDaysForEdition() {
      var transaction = this.LandRecord.Transaction;

      if (transaction.IsEmptyInstance) {
        return;
      }

      const int graceDaysForEdition = 45;
      DateTime lastDate = transaction.PresentationTime;
      if (transaction.LastReentryTime != ExecutionServer.DateMaxValue) {
        lastDate = transaction.LastReentryTime;
      }
      if (lastDate.AddDays(graceDaysForEdition) < DateTime.Today) {
        Assertion.RequireFail("Por motivos de seguridad y calidad en el registro de la información, " +
                             "no es posible modificar inscripciones de documentos en trámites de más de 45 días.\n\n" +
                             "En su lugar se puede optar por registrar un nuevo trámite, " +
                             "o quizás se pueda hacer un reingreso si no han transcurrido los " +
                             "90 días de gracia.");
      }
    }


    // Remove after installation
    public void RefreshSecurityData() {
      this.LandRecord.SecurityData.RefreshSignData(this.LandRecord);

      LandRecordsData.SaveSecurityData(this.LandRecord);
    }


    public void Sign() {
      Assertion.Require(this.LandRecord.Status == RecordableObjectStatus.Closed,
                        "No se pude firmar una inscripción que no está cerrada.");

      this.LandRecord.SecurityData.SetSignData(this.LandRecord);

      LandRecordsData.SaveSecurityData(this.LandRecord);
    }


    public void Unsign() {
      Assertion.Require(this.LandRecord.SecurityData.IsSigned,
                        "No se pude desfirmar una inscripción que no está firmada.");

      this.LandRecord.SecurityData.RemoveSignData();

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
          "InstrumentId", doc.Instrument.Id,
          "TransactionId", doc.Transaction.Id,
          "PresentationTime", doc.PresentationTime,
          "AuthorizationTime", doc.AuthorizationTime,
          "AuthorizedById", doc.AuthorizedBy.Id,
          "SignedById", doc.SecurityData.SignedBy.Id,
          "SignedTime", doc.SecurityData.SignedTime,
          "SecurityData", doc.SecurityData.ExtData.ToString(),
          "ImagingControlID", doc.ImagingControlID,
          "PostedBy", doc.PostedBy.Id, "PostingTime", doc.PostingTime,
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
