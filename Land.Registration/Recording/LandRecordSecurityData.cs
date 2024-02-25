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

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  public enum SignatureType {

    Unsigned = 0,

    Manual = 1,

    Electronic = 2,

  }

  /// <summary>Holds security data for land instrument records.</summary>
  public class LandRecordSecurityData {

    static readonly bool USE_ESIGN = ConfigurationData.Get<bool>("UseESignature", false);

    #region Constructors and parsers

    #endregion Constructors and parsers

    #region Properties

    public bool IsSigned {
      get {
        return !this.SignedBy.IsEmptyInstance;
      }
    }


    public bool IsUnsigned {
      get {
        return !this.IsSigned;
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


    public string DigitalSignature {
      get {
        return this.ExtData.Get("digitalSignature", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("digitalSignature", value);
      }
    }


    public string DigitalSignatureToken {
      get {
        return this.ExtData.Get("digitalSignatureToken", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("digitalSignatureToken", value);
      }
    }


    [DataField("SecurityExtData")]
    internal JsonObject ExtData {
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

    public SignatureType SignatureType {
      get {
        return this.ExtData.Get("signatureType", SignatureType.Unsigned);
      }
      private set {
        this.ExtData.SetIf("signatureType", value, value != SignatureType.Unsigned);
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

    public bool UsesESign {
      get {
        return SignatureType == SignatureType.Electronic;
      }
    }

    #endregion Properties

    #region Methods

    internal void RemoveSignData() {
      this.SignedBy = Person.Empty;
      this.SignedByJobTitle = string.Empty;
      this.SignedTime = ExecutionServer.DateMinValue;

      this.SecurityHash = string.Empty;
      this.DigitalSeal = string.Empty;
      this.DigitalSignature = string.Empty;
      this.DigitalSignatureToken = string.Empty;
      this.SignatureType = SignatureType.Unsigned;
    }


    internal void SetSignData(LandRecord landRecord) {
      this.SignedBy = landRecord.RecorderOffice.GetSigner();
      this.SignedByJobTitle = SignedBy.JobTitle;
      this.SignedTime = DateTime.Now;

      this.SecurityHash = GenerateSecurityHash(landRecord);
      this.DigitalSeal = GenerateDigitalSeal(landRecord);
      this.DigitalSignature = GenerateDigitalSignature(landRecord);
      this.DigitalSignatureToken = GenerateDigitalSignatureToken();
      this.SignatureType = GenerateSignatureType();
    }

    #endregion Methods

    #region Helpers

    static public string GenerateDigitalSeal(LandRecord landRecord) {
      var transaction = landRecord.Transaction;

      string s = "||" + transaction.UID + "|" + landRecord.UID;
      for (int i = 0; i < landRecord.RecordingActs.Count; i++) {
        s += "|" + landRecord.RecordingActs[i].Id.ToString();
      }
      s += "||";

      return Cryptographer.SignTextWithSystemCredentials(s);
    }


    static public string GenerateSecurityHash(LandRecord landRecord) {
      if (landRecord.IsNew) {
        return String.Empty;
      }

      return Cryptographer.CreateHashCode(landRecord.Id.ToString("00000000") +
                                          landRecord.AuthorizationTime.ToString("yyyyMMddTHH:mm"),
                                          landRecord.UID)
                          .Substring(0, 8)
                          .ToUpperInvariant();
    }


    private SignatureType GenerateSignatureType() {
      if (!IsSigned) {
        return SignatureType.Unsigned;
      }
      if (USE_ESIGN) {
        return SignatureType.Electronic;
      }

      return SignatureType.Manual;
    }


    static private string GenerateDigitalSignatureToken() {
      if (USE_ESIGN) {
        return Guid.NewGuid().ToString();
      }
      return string.Empty;
    }

    private string GenerateDigitalSignature(LandRecord landRecord) {
      if (!this.IsSigned) {
        return "ESTE DOCUMENTO NO HA SIDO FIRMADO";
      }
      if (!USE_ESIGN) {
        return "Documento firmado de forma autógrafa.";
      }
      return DigitalSignatureData.GetDigitalSignature(landRecord)
                                 .Substring(0, 64);
    }

    #endregion Helpers

  }  // class LandRecordSecurityData

}  // namespace Empiria.Land.Registration
