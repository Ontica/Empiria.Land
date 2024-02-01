/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording services                      Component : Recording documents                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Separated entity                      *
*  Type     : RecordingDocumentSecurity                    License   : Please read LICENSE.txt file          *
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
  public class RecordingDocumentSecurity: IProtected {

    private readonly bool USE_E_SIGN = ConfigurationData.Get<bool>("UseESignature", false);

    #region Constructors and parsers

    private RecordingDocumentSecurity() {
      // Used by Empiria Framework
    }

    internal RecordingDocumentSecurity(RecordingDocument landRecord) {
      this.LandRecord = landRecord;
    }

    #endregion Constructors and parsers

    #region Public properties

    internal RecordingDocument LandRecord {
      get;
    }


    public bool UseESign {
      get {
        return USE_E_SIGN;
      }
    }


    #endregion Public properties

    #region Public methods

    public bool Signed() {
      return DigitalSignatureData.IsSigned(this.LandRecord);
    }


    public bool Unsigned() {
      return !this.Signed();
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
      if (this.LandRecord.Status != RecordableObjectStatus.Closed) {
        return false;
      }
      if (this.LandRecord.Security.Signed()) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditLandRecord(this.LandRecord);
    }

    public void AssertCanBeClosed() {
      if (!this.IsReadyToClose()) {
        Assertion.RequireFail("El usuario no tiene permisos para cerrar el documento o éste no tiene un estado válido.");
      }

      //this.AssertGraceDaysForEdition();

      Assertion.Require(this.LandRecord.RecordingActs.Count > 0, "El documento no tiene actos jurídicos.");

      foreach (var recordingAct in this.LandRecord.RecordingActs) {
        recordingAct.Validator.AssertCanBeClosed();
      }
    }


    public void AssertCanBeOpened() {
      if (!this.IsReadyToOpen()) {
        Assertion.RequireFail("El usuario no tiene permisos para abrir este documento.");
      }

      //this.AssertGraceDaysForEdition();

      foreach (var recordingAct in this.LandRecord.RecordingActs) {
        recordingAct.Validator.AssertCanBeOpened();
      }
    }

    private void AssertGraceDaysForEdition() {
      var transaction = this.LandRecord.GetTransaction();

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
                             "no es posible modificar documentos de trámites de más de 45 días.\n\n" +
                             "En su lugar se puede optar por registrar un nuevo trámite, " +
                             "o quizás se pueda hacer un reingreso si no han transcurrido los " +
                             "90 días de gracia.");
      }
    }


    public string GetDigitalSeal() {
      var transaction = this.LandRecord.GetTransaction();

      string s = "||" + transaction.UID + "|" + this.LandRecord.UID;
      for (int i = 0; i < this.LandRecord.RecordingActs.Count; i++) {
        s += "|" + this.LandRecord.RecordingActs[i].Id.ToString();
      }
      s += "||";

      return Cryptographer.SignTextWithSystemCredentials(s);
    }


    public string GetDigitalSignature() {
      if (!UseESign) {
        return "Documento firmado de forma autógrafa.";
      }
      if (this.Unsigned()) {
        return "NO TIENE FIRMA ELECTRÓNICA";
      } else {
        return DigitalSignatureData.GetDigitalSignature(this.LandRecord)
                                   .Substring(0, 64);
      }
    }


    public Person GetSignedBy() {
      if (UseESign) {
        return DigitalSignatureData.GetDigitalSignatureSignedBy(this.LandRecord);
      } else {
        return this.LandRecord.RecorderOffice.GetSigner();
      }
    }


    public string QRCodeSecurityHash() {
      if (this.LandRecord.IsNew) {
        return String.Empty;
      }

      return Cryptographer.CreateHashCode(this.LandRecord.Id.ToString("00000000") +
                                          this.LandRecord.AuthorizationTime.ToString("yyyyMMddTHH:mm"),
                                          this.LandRecord.UID)
                           .Substring(0, 8)
                           .ToUpperInvariant();
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
          1, "Id", doc.Id, "DocumentTypeId", doc.DocumentType.Id,
          "UID", doc.UID, "IssuePlaceId", doc.IssuePlace.Id,
          "IssueOfficeId", doc.IssueOffice.Id,
          "IssuedById", doc.IssuedBy.Id, "IssueDate", doc.IssueDate,
          "AsText", doc.AsText, "SheetsCount", doc.SheetsCount,
          "ExtensionData", doc.ExtensionData.ToJson(),
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

  } // class RecordingDocumentSecurity

} // namespace Empiria.Land.Registration
