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
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Ontology;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Contains extensible data for a land registration system transaction.</summary>
  public class LRSTransactionExtData : IExtensibleData {

    #region Constructors and parsers

    public LRSTransactionExtData() {
      this.RequesterEmail = String.Empty;
      this.RequesterPhone = String.Empty;
      this.RequesterNotes = String.Empty;
      this.OfficeNotes = String.Empty;
      this.ClosingNotes = String.Empty;
      this.DeliveryNotes = String.Empty;
    }

    static internal LRSTransactionExtData Parse(string jsonString) {
      if (String.IsNullOrWhiteSpace(jsonString)) {
        return LRSTransactionExtData.Empty;
      }

      var json = Empiria.Json.JsonConverter.ToJsonObject(jsonString);

      var data = new LRSTransactionExtData();
      data.RequesterEmail = json.Get<String>("RequesterEmail", String.Empty);
      data.RequesterPhone = json.Get<String>("RequesterPhone", String.Empty);
      data.RequesterNotes = json.Get<String>("RequesterNotes", String.Empty);
      data.OfficeNotes = json.Get<String>("OfficeNotes", String.Empty);
      data.ClosingNotes = json.Get<String>("ClosingNotes", String.Empty);
      data.DeliveryNotes = json.Get<String>("DeliveryNotes", String.Empty);

      return data;
    }

    static private readonly LRSTransactionExtData _empty = new LRSTransactionExtData() {
      IsEmptyInstance = true
    };

    static public LRSTransactionExtData Empty {
      get {
        return _empty;
      }
    }

    #endregion Constructors and parsers

    #region Properties

    public string RequesterEmail {
      get;
      set;
    }

    public string RequesterPhone {
      get;
      set;
    }

    public string RequesterNotes {
      get;
      set;
    }

    public string OfficeNotes {
      get;
      set;
    }

    public string ClosingNotes {
      get;
      set;
    }

    public string DeliveryNotes {
      get;
      set;
    }

    public bool IsEmptyInstance {
      get;
      private set;
    }

    #endregion Properties

    #region Methods

    public T GetObject<T>(string objectKey) {
      return default(T);
    }

    public string ToJson() {
      if (!this.IsEmptyInstance) {
        return Empiria.Json.JsonConverter.ToJson(this.GetObject());
      } else {
        return String.Empty;
      }
    }

    public override string ToString() {
      return this.ToJson().ToString();
    }

    private object GetObject() {
      return new {
        RequesterEmail = this.RequesterEmail,
        RequesterPhone = this.RequesterPhone,
        RequesterNotes = this.RequesterNotes,
        OfficeNotes = this.OfficeNotes,
        ClosingNotes = this.ClosingNotes,
        DeliveryNotes = this.DeliveryNotes,
      };
    }

    #endregion Methods

  }  // class LRSTransactionExtData

} // namespace Empiria.Land.Registration.Transactions
