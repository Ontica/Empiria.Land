/* Empiria Land **********************************************************************************************
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
using Empiria.Security;

namespace Empiria.Land.Registration {

  public enum SignStatus {

    Unsigned = 'U',

    Signed = 'S',

    Refused = 'F',

    Revoked = 'K',

    Undefined = 'X',
  }


  public enum SignType {

    Undeterminated = 'U',

    Manual = 'M',

    Electronic = 'E',

    Historic = 'H',

  }


  /// <summary>Holds security data for land instrument records.</summary>
  public class LandRecordSecurityData {

    public static readonly bool ESIGN_ENABLED = ConfigurationData.Get<bool>("ElectronicSignatureEnabled", false);

    #region Constructors and parsers

    #endregion Constructors and parsers

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
      this.SecurityHash = GenerateSecurityHash(landRecord);
      this.DigitalSeal = GenerateDigitalSeal(landRecord);
      this.DigitalSealVersion = GenerateDigitalSealVersion(landRecord);

      this.SignedBy = landRecord.RecorderOffice.GetSigner();
      this.SignedByJobTitle = SignedBy.JobTitle;
      this.SignedTime = ExecutionServer.DateMinValue;

      this.SignStatus = SignStatus.Unsigned;
      this.SignType = SignType.Electronic;
    }


    internal void RemoveSignData() {
      this.ExtData = new JsonObject();

      this.SignedBy = Person.Empty;
      this.SignedTime = ExecutionServer.DateMinValue;
      this.SignStatus = SignStatus.Unsigned;
      this.SignType = SignType.Undeterminated;
    }


    internal void SetElectronicSignData(LandESignData signData) {

      this.Digest = signData.Digest;
      this.SignDocumentID = signData.DocumentID;
      this.SignDocumentName = signData.DocumentName;

      this.DigitalSignature = signData.Signature;

      this.SignedTime = signData.SignedTime;
      this.SignStatus = SignStatus.Signed;
    }


    internal void SetManualSignData(LandRecord landRecord) {
      this.SecurityHash = GenerateSecurityHash(landRecord);
      this.DigitalSeal = GenerateDigitalSeal(landRecord);
      this.DigitalSealVersion = GenerateDigitalSealVersion(landRecord);
      this.DigitalSignature = "Documento firmado de forma autógrafa.";

      this.SignedBy = landRecord.RecorderOffice.GetSigner();
      this.SignedByJobTitle = SignedBy.JobTitle;
      this.SignedTime = DateTime.Now;

      this.SignType = SignType.Manual;
      this.SignStatus = SignStatus.Signed;
    }


    // ToDo: Remove after installation
    internal void RefreshSignData(LandRecord landRecord) {
      if (this.SignStatus == SignStatus.Unsigned) {
        this.ExtData = new JsonObject();
        return;
      }

      var jobTitle = SignedByJobTitle;

      this.ExtData = new JsonObject();

      this.SecurityHash = GenerateSecurityHash(landRecord);
      this.DigitalSeal = GenerateDigitalSeal(landRecord);
      this.DigitalSealVersion = GenerateDigitalSealVersion(landRecord);
      this.DigitalSignature = "Documento firmado de forma autógrafa.";
      this.SignedByJobTitle = jobTitle;
    }

    #endregion Methods

    #region Helpers

    static private string GenerateCurrentDigitalSeal(LandRecord landRecord) {
      var transaction = landRecord.Transaction;

      string s = "||" + transaction.UID + "|" + landRecord.UID;

      for (int i = 0; i < landRecord.RecordingActs.Count; i++) {
        s += "|" + landRecord.RecordingActs[i].Id.ToString();
      }
      s += "||";

      return Cryptographer.SignTextWithSystemCredentials(s);
    }


    static private string GenerateDigitalSeal(LandRecord landRecord) {

      var bookEntries = BookEntry.GetBookEntriesForLandRecord(landRecord);

      if (bookEntries.Count == 0) {
        return GenerateCurrentDigitalSeal(landRecord);
      } else {
        return GenerateFormerDigitalSeal(landRecord, bookEntries);
      }
    }


    static private string GenerateFormerDigitalSeal(LandRecord landRecord,
                                                   FixedList<BookEntry> bookEntries) {
      var transaction = landRecord.Transaction;

      string s = "||" + transaction.UID + "|" + transaction.LandRecord.UID;

      for (int i = 0; i < bookEntries.Count; i++) {
        s += "|" + bookEntries[i].Id.ToString();
      }

      s += "||";

      return Cryptographer.SignTextWithSystemCredentials(s);
    }


    static private string GenerateDigitalSealVersion(LandRecord landRecord) {
      var bookEntries = BookEntry.GetBookEntriesForLandRecord(landRecord);

      if (bookEntries.Count != 0) {
        return "1.0";
      } else if (!ESIGN_ENABLED) {
        return "5.0";
      } else {
        return "6.0";
      }
    }


    static private string GenerateSecurityHash(LandRecord landRecord) {
      return Cryptographer.CreateHashCode(landRecord.Id.ToString("00000000") +
                                          landRecord.AuthorizationTime.ToString("yyyyMMddTHH:mm"),
                                          landRecord.UID)
                          .Substring(0, 8)
                          .ToUpperInvariant();
    }


    #endregion Helpers

  }  // class LandRecordSecurityData

}  // namespace Empiria.Land.Registration
