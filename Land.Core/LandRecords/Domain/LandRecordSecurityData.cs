﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording services                      Component : Recording documents                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Information Holder                    *
*  Type     : LandRecordSecurityData                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds security data for land instrument records.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Land.Registration.UseCases;
using Empiria.Security;

namespace Empiria.Land.Registration {

  /// <summary>Holds security data for land instrument records.</summary>
  public class LandRecordSecurityData {

    public static readonly bool ESIGN_ENABLED = true;  // ConfigurationData.Get<bool>("ElectronicSignatureEnabled", false);

    #region Properties

    public bool IsSigned {
      get {
        return !this.SignedBy.IsEmptyInstance &&
               this.SignStatus == SignStatus.Signed &&
               DigitalSignature.Length != 0;
      }
    }


    public bool IsUnsigned {
      get {
        return !this.IsSigned;
      }
    }

    public string Digest {
      get {
        return this.ExtData.Get("electronicSign/digest", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("electronicSign/digest", value);
      }
    }


    public string DigitalSeal {
      get {
        return this.ExtData.Get("digitalSeal", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("digitalSeal", value);
      }
    }


    public string DigitalSealVersion {
      get {
        return this.ExtData.Get("digitalSealVersion", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("digitalSealVersion", value);
      }
    }


    public string DigitalSignature {
      get {
        return this.ExtData.Get("digitalSignature", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("digitalSignature", value);
      }
    }

    [DataField("SecurityExtData", IsEncrypted = true)]
    internal JsonObject ExtData {
      get;
      private set;
    }


    [DataField("SignStatus", Default = SignStatus.Unsigned)]
    public SignStatus SignStatus {
      get;
      private set;
    }


    [DataField("SignType", Default = SignType.Undeterminated)]
    public SignType SignType {
      get;
      private set;
    }


    [DataField("SignedById")]
    public Person SignedBy {
      get;
      private set;
    }


    public string SignedByJobTitle {
      get {
        return this.ExtData.Get("signedByJobTitle", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("signedByJobTitle", value);
      }
    }


    [DataField("SignedTime", Default = "ExecutionServer.DateMinValue")]
    public DateTime SignedTime {
      get;
      private set;
    }


    public string SignGuid {
      get {
        return this.ExtData.Get("electronicSign/signGUID", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("electronicSign/signGUID", value);
      }
    }


    public string SecurityHash {
      get {
        return this.ExtData.Get("securityHash", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("securityHash", value);
      }
    }


    public string SignDocumentID {
      get {
        return this.ExtData.Get("electronicSign/signDocumentID", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("electronicSign/signDocumentID", value);
      }
    }

    public string SignDocumentName {
      get {
        return this.ExtData.Get("electronicSign/signDocumentName", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("electronicSign/signDocumentName", value);
      }
    }

    public bool UsesESign {
      get {
        return SignType == SignType.Electronic;
      }
    }

    #endregion Properties

    #region Methods

    internal void PrepareForElectronicSign(LandRecord landRecord) {
      this.SignGuid = Guid.NewGuid().ToString().ToUpperInvariant();
      this.DigitalSealVersion = GenerateDigitalSealVersion(landRecord);
      this.DigitalSeal = GenerateDigitalSeal(landRecord);
      this.SecurityHash = GenerateSecurityHash(landRecord);

      var currentSigner = ExecutionServer.CurrentContact as Person;

      this.SignedBy = currentSigner;
      this.SignedByJobTitle = currentSigner.JobTitle;
    }


    internal void RemoveSignData() {
      this.ExtData = new JsonObject();

      this.SignedBy = Person.Empty;
      this.SignedTime = ExecutionServer.DateMinValue;
      this.SignStatus = SignStatus.Unsigned;
      this.SignType = SignType.Undeterminated;
    }


    internal void RevokeSignData() {
      this.SignGuid = string.Empty;
      this.DigitalSignature = string.Empty;

      this.SignedTime = ExecutionServer.DateMinValue;
      this.SignStatus = SignStatus.Revoked;
    }


    internal void SetElectronicSignData(LandESignData signData) {
      this.Digest = signData.Digest;
      this.SignDocumentID = signData.DocumentID;
      this.SignDocumentName = signData.DocumentName;

      this.DigitalSignature = signData.Signature;

      this.SignedTime = signData.SignedTime;
      this.SignStatus = SignStatus.Signed;
    }


    internal void SetElectronicSignerData(Person signer) {
      this.SignedBy = signer;
      this.SignedByJobTitle = signer.JobTitle;
      this.SignedTime = ExecutionServer.DateMinValue;

      this.SignStatus = SignStatus.Unsigned;
      this.SignType = SignType.Electronic;
    }


    internal void SetManualSignData(LandRecord landRecord) {
      this.SignGuid = Guid.NewGuid().ToString().ToUpperInvariant();
      this.DigitalSealVersion = GenerateDigitalSealVersion(landRecord);
      this.DigitalSeal = GenerateDigitalSeal(landRecord);
      this.SecurityHash = GenerateSecurityHash(landRecord);

      this.DigitalSignature = "Documento firmado de forma autógrafa.";

      this.SignedBy = landRecord.RecorderOffice.Signer;
      this.SignedByJobTitle = SignedBy.JobTitle;
      this.SignedTime = DateTime.Now;

      this.SignType = SignType.Manual;
      this.SignStatus = SignStatus.Signed;
    }


    #endregion Methods

    #region Helpers

    private string GenerateDigitalSeal(LandRecord landRecord) {
      if (DigitalSealVersion == "5.1") {
        return GenerateDigitalSealV51(landRecord);

      } else if (DigitalSealVersion == "5.0") {
        return GenerateRecordingActsV1_5_DigitalSeal(landRecord);

      } else if (DigitalSealVersion == "1.0") {
        var bookEntries = BookEntry.GetBookEntriesForLandRecord(landRecord);

        return GenerateBookEntriesV1_5_DigitalSeal(landRecord, bookEntries);

      } else {
        throw Assertion.EnsureNoReachThisCode($"Unrecognized digital seal version '{DigitalSealVersion}'.");
      }
    }


    static private string GenerateDigitalSealVersion(LandRecord landRecord) {
      var bookEntries = BookEntry.GetBookEntriesForLandRecord(landRecord);

      if (bookEntries.Count != 0) {
        return "1.0";
      } else if (!ESIGN_ENABLED) {
        return "5.0";
      } else {
        return "5.1";
      }
    }


    private string GenerateSecurityHash(LandRecord landRecord) {
      if (DigitalSealVersion == "5.1") {
        var hc = Cryptographer.CreateHashCode(landRecord.Id.ToString("00000000") +
                                              landRecord.PresentationTime.ToString("yyyyMMddTHH:mm:ss") +
                                              landRecord.AuthorizationTime.ToString("yyyyMMddTHH:mm:ss") +
                                              SignGuid + DigitalSeal,
                                              landRecord.UID);
       return (hc.Substring(0, 5) + "-" + hc.Substring(5, 5)).ToUpperInvariant();
      }

      return Cryptographer.CreateHashCode(landRecord.Id.ToString("00000000") +
                                          landRecord.AuthorizationTime.ToString("yyyyMMddTHH:mm"),
                                          landRecord.UID)
                          .Substring(0, 8)
                          .ToUpperInvariant();
    }


    #endregion Helpers

    #region Digital seals generators

    static private string GenerateBookEntriesV1_5_DigitalSeal(LandRecord landRecord,
                                                              FixedList<BookEntry> bookEntries) {
      var transaction = landRecord.Transaction;

      string s = "||" + transaction.UID + "|" + transaction.LandRecord.UID;

      for (int i = 0; i < bookEntries.Count; i++) {
        s += "|" + bookEntries[i].Id.ToString();
      }

      s += "||";

      return Cryptographer.SignTextWithSystemCredentials(s);
    }

    static private string GenerateDigitalSealV51(LandRecord landRecord) {
      var transaction = landRecord.Transaction;

      string s = "||" + transaction.UID + "|" + landRecord.UID + "|" + landRecord.Instrument.UID;

      for (int i = 0; i < landRecord.RecordingActs.Count; i++) {
        var recordingAct = landRecord.RecordingActs[i];

        s += $"|{recordingAct.RecordingActType.Id},{recordingAct.Id},{recordingAct.Resource.Id}";

        for (int j = 0; j < recordingAct.Parties.PrimaryParties.Count; j++) {
          var party = recordingAct.Parties.PrimaryParties[j];

          s += $"^{party.Id},{party.PartyRole.Name},{party.Party.Id},{party.Party.FullName}";
        }

        for (int j = 0; j < recordingAct.Parties.SecondaryParties.Count; j++) {
          var party = recordingAct.Parties.SecondaryParties[j];

          s += $"#{party.Id},{party.PartyRole.Name},{party.Party.Id},{party.Party.FullName}";
        }
      }
      s += "||";

      return Cryptographer.SignTextWithSystemCredentials(s);
    }

    static private string GenerateRecordingActsV1_5_DigitalSeal(LandRecord landRecord) {
      var transaction = landRecord.Transaction;

      string s = "||" + transaction.UID + "|" + landRecord.UID;

      for (int i = 0; i < landRecord.RecordingActs.Count; i++) {
        s += "|" + landRecord.RecordingActs[i].Id.ToString();
      }
      s += "||";

      return Cryptographer.SignTextWithSystemCredentials(s);
    }

    // ToDo: Remove this method when all opened land records are manually closed
    internal void SetManualSignData(LandRecord landRecord, ManualCloseRecordFields fields) {
      this.SignGuid = Guid.NewGuid().ToString().ToUpperInvariant();
      this.DigitalSealVersion = "5.0";
      this.DigitalSeal = GenerateDigitalSeal(landRecord);
      this.SecurityHash = GenerateSecurityHash(landRecord);

      this.DigitalSignature = "Documento firmado de forma autógrafa.";

      this.SignedBy = fields.SignedBy;
      this.SignedByJobTitle = fields.SignedBy.JobTitle;
      this.SignedTime = fields.AuthorizationTime;

      this.SignType = SignType.Manual;
      this.SignStatus = SignStatus.Signed;
    }

    #endregion Digital seals generators

  }  // class LandRecordSecurityData

}  // namespace Empiria.Land.Registration
