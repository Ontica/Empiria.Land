/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration.Transactions         Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionExtData                          Pattern  : IExtensibleData class               *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Contains extensible data for a land registration system transaction.                          *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Json;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Contains extensible data for a land registration system transaction.</summary>
  public class LRSTransactionExtData {

    #region Fields

    private readonly JsonObject _json = new JsonObject();

    #endregion Fields

    #region Constructors and parsers

    private LRSTransactionExtData(JsonObject json) {
      this._json = json;
    }

    public LRSTransactionExtData(Resource baseResource) {
      this.BaseResource = baseResource;
    }

    static internal LRSTransactionExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return LRSTransactionExtData.Empty;
      }

      var json = JsonConverter.ToJsonObject(jsonString);

      return new LRSTransactionExtData(json);
    }

    static private readonly LRSTransactionExtData _empty = new LRSTransactionExtData(JsonObject.Empty);

    static public LRSTransactionExtData Empty {
      get {
        return _empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public Resource BaseResource {
      get {
        return _json.Get<Resource>("BaseResourceId", Resource.Empty);
      }
      private set {
        _json.SetIfValue("BaseResourceId", value.Id);
      }
    }

    public string RequesterEmail {
      get {
        return _json.Get<string>("RequesterEmail", String.Empty);
      }
      private set {
        _json.SetIfValue("RequesterEmail", value);
      }
    }

    public string RequesterPhone {
      get {
        return _json.Get<string>("RequesterPhone", String.Empty);
      }
      private set {
        _json.SetIfValue("RequesterPhone", value);
      }
    }

    public string RequesterNotes {
      get {
        return _json.Get<string>("RequesterPhone", String.Empty);
      }
      private set {
        _json.SetIfValue("RequesterPhone", value);
      }
    }

    public string OfficeNotes {
      get {
        return _json.Get<string>("OfficeNotes", String.Empty);
      }
      private set {
        _json.SetIfValue("OfficeNotes", value);
      }
    }

    public string ClosingNotes {
      get {
        return _json.Get<string>("ClosingNotes", String.Empty);
      }
      private set {
        _json.SetIfValue("ClosingNotes", value);
      }
    }

    public string DeliveryNotes {
      get {
        return _json.Get<string>("DeliveryNotes", String.Empty);
      }
      private set {
        _json.SetIfValue("DeliveryNotes", value);
      }
    }

    public LRSExternalTransaction ExternalTransaction {
      get {
        return LRSExternalTransaction.Parse(_json.Slice("ExternalTransaction", false));
      }
      internal set {
        _json.SetIfValue("ExternalTransaction", value);
      }
    }

    public bool IsEmptyInstance {
      get;
    } = false;

    #endregion Properties

    #region Methods

    public override string ToString() {
      return _json.ToString();
    }

    #endregion Methods

  }  // class LRSTransactionExtData

} // namespace Empiria.Land.Registration.Transactions
