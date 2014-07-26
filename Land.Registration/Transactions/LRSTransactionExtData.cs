﻿/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration.Transactions         Assembly : Empiria.Land.Registration           *
*  Type      : LRSTransactionExtData                          Pattern  : IExtensibleData class               *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Contains extensible data for a land registration system transaction.                          *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Ontology;

namespace Empiria.Land.Registration.Transactions {

  /// <summary>Contains extensible data for a land registration system transaction.</summary>
  public class LRSTransactionExtData : IExtensibleData {

    #region Fields

    private bool _isEmptyInstance = false;

    #endregion Fields

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

      var json = Empiria.Data.JsonConverter.ToJsonObject(jsonString);

      var data = new LRSTransactionExtData();
      data.RequesterEmail = json.Find<String>("RequesterEmail", String.Empty);
      data.RequesterPhone = json.Find<String>("RequesterPhone", String.Empty);
      data.RequesterNotes = json.Find<String>("RequesterNotes", String.Empty);
      data.OfficeNotes = json.Find<String>("OfficeNotes", String.Empty);
      data.ClosingNotes = json.Find<String>("ClosingNotes", String.Empty);
      data.DeliveryNotes = json.Find<String>("DeliveryNotes", String.Empty);

      return data;
    }

    static public LRSTransactionExtData Empty {
      get {
        var data = new LRSTransactionExtData();
        data._isEmptyInstance = true;
        return data;
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
      get { return _isEmptyInstance; }
    }

    #endregion Properties

    #region Methods

    public string ToJson() {
      if (!this.IsEmptyInstance) {
        return Empiria.Data.JsonConverter.ToJson(this.GetObject());
      } else {
        return String.Empty;
      }
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