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

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Registration {

  /// <summary>Contains security methods used to protect the integrity of recording documents.</summary>
  public class RecordingDocumentSecurity: IProtected {

    #region Constructors and parsers

    internal RecordingDocumentSecurity(RecordingDocument document) {
      this.Document = document;
    }

    #endregion Constructors and parsers

    #region Public properties

    internal RecordingDocument Document {
      get;
    }


    public bool UseESign {
      get {
        return this.Document.AuthorizationTime >= DateTime.Parse("2018-07-10");
      }
    }


    #endregion Public properties

    #region Public methods


    public void Close() {
      this.AssertCanBeClosed();
      this.Document.Close();
    }


    public void Open() {
      this.AssertCanBeOpened();
      this.Document.Open();
    }


    public bool Signed() {
      return Data.DocumentsData.IsSigned(this.Document);
    }


    public bool Unsigned() {
      return !this.Signed();
    }


    public bool IsReadyForEdition() {
      if (this.Document.IsEmptyInstance) {
        return false;
      }
      if (this.Document.Status != RecordableObjectStatus.Incomplete) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditDocument(this.Document);
    }


    public bool IsReadyToClose() {
      if (this.Document.IsEmptyInstance) {
        return false;
      }
      if (this.Document.Status != RecordableObjectStatus.Incomplete) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditDocument(this.Document);
    }


    public bool IsReadyToOpen() {
      if (this.Document.IsEmptyInstance) {
        return false;
      }
      if (this.Document.Status != RecordableObjectStatus.Closed) {
        return false;
      }
      if (this.Document.Security.Signed()) {
        return false;
      }
      return LRSWorkflowRules.UserCanEditDocument(this.Document);
    }

    public void AssertCanBeClosed() {
      if (!this.IsReadyToClose()) {
        Assertion.AssertFail("El usuario no tiene permisos para cerrar el documento o éste no tiene un estado válido.");
      }

      //this.AssertGraceDaysForEdition();

      Assertion.Assert(this.Document.RecordingActs.Count > 0, "El documento no tiene actos jurídicos.");

      foreach (var recordingAct in this.Document.RecordingActs) {
        recordingAct.AssertCanBeClosed();
      }
    }


    public void AssertCanBeOpened() {
      if (!this.IsReadyToOpen()) {
        Assertion.AssertFail("El usuario no tiene permisos para abrir este documento.");
      }

      //this.AssertGraceDaysForEdition();

      foreach (var recordingAct in this.Document.RecordingActs) {
        recordingAct.AssertCanBeOpened();
      }
    }

    private void AssertGraceDaysForEdition() {
      var transaction = this.Document.GetTransaction();

      if (transaction.IsEmptyInstance) {
        return;
      }

      const int graceDaysForEdition = 45;
      DateTime lastDate = transaction.PresentationTime;
      if (transaction.LastReentryTime != ExecutionServer.DateMaxValue) {
        lastDate = transaction.LastReentryTime;
      }
      if (lastDate.AddDays(graceDaysForEdition) < DateTime.Today) {
        Assertion.AssertFail("Por motivos de seguridad y calidad en el registro de la información, " +
                             "no es posible modificar documentos de trámites de más de 45 días.\n\n" +
                             "En su lugar se puede optar por registrar un nuevo trámite, " +
                             "o quizás se pueda hacer un reingreso si no han transcurrido los " +
                             "90 días de gracia.");
      }
    }


    public string GetDigitalSeal() {
      var transaction = this.Document.GetTransaction();

      string s = "||" + transaction.UID + "|" + this.Document.UID;
      for (int i = 0; i < this.Document.RecordingActs.Count; i++) {
        s += "|" + this.Document.RecordingActs[i].Id.ToString();
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
        return Data.DocumentsData.GetDigitalSignature(this.Document)
                                 .Substring(0, 64);
      }
    }


    public Person GetSignedBy() {
      if (UseESign) {
        return Data.DocumentsData.GetDigitalSignatureSignedBy(this.Document);
      } else {
        return Person.Parse(36);
      }
    }


    public string QRCodeSecurityHash() {
      if (this.Document.IsNew) {
        return String.Empty;
      }

      return FormerCryptographer.CreateHashCode(this.Document.Id.ToString("00000000") +
                                                this.Document.AuthorizationTime.ToString("yyyyMMddTHH:mm"),
                                                this.Document.UID)
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
      var doc = this.Document;
      if (version == 1) {
        return new object[] {
          1, "Id", doc.Id, "DocumentTypeId", doc.DocumentType.Id,
          "SubtypeId", doc.Subtype.Id, "UID", doc.UID,
          "IssuePlaceId", doc.IssuePlace.Id, "IssueOfficeId", doc.IssueOffice.Id,
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
