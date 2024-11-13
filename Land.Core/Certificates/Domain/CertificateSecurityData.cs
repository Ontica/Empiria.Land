/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Domain Layer                            *
*  Assembly : Empiria.Land.Registration.dll              Pattern   : Information Holder                      *
*  Type     : CertificateSecurityData                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds security data for land certificates.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Security;

namespace Empiria.Land.Certificates {

  /// <summary>Holds security data for land certificates.</summary>
  public class CertificateSecurityData {

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

    internal void PrepareForElectronicSign(Certificate certificate) {
      this.SignGuid = Guid.NewGuid().ToString().ToUpperInvariant();
      this.DigitalSealVersion = GenerateDigitalSealVersion();
      this.DigitalSeal = GenerateDigitalSeal(certificate);
      this.SecurityHash = GenerateSecurityHash(certificate);

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


    #endregion Methods

    #region Helpers

    private string GenerateDigitalSeal(Certificate certificate) {
      if (DigitalSealVersion == "5.1") {
        return GenerateDigitalSealV51(certificate);
      }

      throw Assertion.EnsureNoReachThisCode($"Unrecognized digital seal version '{DigitalSealVersion}'.");
    }


    static private string GenerateDigitalSealVersion() {
      return "5.1";
    }


    private string GenerateSecurityHash(Certificate certificate) {
      if (DigitalSealVersion == "5.1") {
        var hc = Cryptographer.CreateHashCode(certificate.Id.ToString("00000000") +
                                              certificate.IssueTime.ToString("yyyyMMddTHH:mm:ss") +
                                              certificate.Transaction.PresentationTime.ToString("yyyyMMddTHH:mm:ss") +
                                              SignGuid + DigitalSeal,
                                              certificate.UID);
       return (hc.Substring(0, 5) + "-" + hc.Substring(5, 5)).ToUpperInvariant();
      }

      throw Assertion.EnsureNoReachThisCode($"Unrecognized digital seal version '{DigitalSealVersion}'.");
    }


    #endregion Helpers

    #region Digital seals generators


    static private string GenerateDigitalSealV51(Certificate certificate) {

      string s = $"||{certificate.Transaction.UID}|{certificate.UID}|{certificate.CertificateID}|";

      s += $"{certificate.CertificateType.Id}|{certificate.IssuedBy.Id}|{certificate.IssueMode}|" +
           $"{certificate.OnLandRecord.Id}|{certificate.OnPersonName}|{certificate.OnRecordableSubject.UID}|";

      s += $"{certificate.AsText}";

      s += "||";

      return Cryptographer.SignTextWithSystemCredentials(s);
    }

    #endregion Digital seals generators

  }  // class CertificateSecurityData

}  // namespace Empiria.Land.Certificates
