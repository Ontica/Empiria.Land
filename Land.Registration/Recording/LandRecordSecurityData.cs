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

namespace Empiria.Land.Registration {

  /// <summary>Holds security data for land instrument records.</summary>
  public class LandRecordSecurityData {

    #region Constructors and parsers

    #endregion Constructors and parsers

    #region Properties

    public bool IsSigned {
      get {
        return !this.SignedBy.IsEmptyInstance;
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

    public string SignedByJobPosition {
      get {
        return this.ExtData.Get("signedByJobPosition", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("signedByJobPosition", value);
      }
    }

    [DataField("SignedTime", Default = "ExecutionServer.DateMinValue")]
    public DateTime SignedTime {
      get;
      private set;
    }

    public string SignatureType {
      get {
        return this.ExtData.Get("signatureType", string.Empty);
      }
      private set {
        this.ExtData.SetIfValue("signatureType", value);
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

    #endregion Properties

    internal void RemoveSignData() {
      this.SignedBy = Person.Empty;
      this.SignedByJobPosition = string.Empty;
      this.SignedTime = ExecutionServer.DateMinValue;

      this.SecurityHash = string.Empty;
      this.DigitalSeal = string.Empty;
      this.DigitalSignature = string.Empty;
      this.DigitalSignatureToken = string.Empty;
      this.SignatureType = string.Empty;
    }


    internal void SetSignData(LandRecord landRecord) {
      this.SignedBy = landRecord.RecorderOffice.GetSigner();
      this.SignedByJobPosition = SignedBy.JobPosition;
      this.SignedTime = DateTime.Now;

      var security = landRecord.Security;

      this.SecurityHash = security.QRCodeSecurityHash();
      this.DigitalSeal = security.GetDigitalSeal();
      this.DigitalSignature = security.GetDigitalSignature();
      this.DigitalSignatureToken = security.GetDigitalSignatureToken();
      this.SignatureType = security.GetSignatureType();
    }

  }  // class LandRecordSecurityData

}  // namespace Empiria.Land.Registration
