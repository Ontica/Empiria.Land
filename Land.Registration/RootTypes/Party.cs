﻿/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Party                                          Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Abstract class that represents a recording act party.                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.Geography;
using Empiria.Land.Registration.Data;
using Empiria.Ontology;

namespace Empiria.Land.Registration {

  public enum PartyStatus {
    Pending = 'P',
    Registered = 'R',
    Closed = 'C',
    Deleted = 'X'
  }

  public enum PartyFilterType {
    ByKeywords = 1,
    OnInscription = 2,
    OnRecordingBook = 3,
    Involved = 4,
  }

  /// <summary>Abstract class that represents a recording act party.</summary>
  public abstract class Party : BaseObject {

    #region Fields

    private const string thisTypeName = "ObjectType.Party";

    #endregion Fields

    #region Constructors and parsers

    protected Party(string typeName) : base(typeName) {
      // Empiria Object Type pattern classes always has this constructor. Don't delete
    }

    static public Party Parse(int id) {
      return BaseObject.Parse<Party>(thisTypeName, id);
    }

    static internal Party Parse(DataRow dataRow) {
      return BaseObject.Parse<Party>(thisTypeName, dataRow);
    }

    static public FixedList<Party> GetList(ObjectTypeInfo partyType, string keywords) {
      DataTable table = PropertyData.GetParties(partyType, keywords);

      return new FixedList<Party>((x) => Party.Parse(x), table);
    }

    static public FixedList<Party> GetList(PartyFilterType partyFilterType, ObjectTypeInfo partyType,
                                            RecordingAct recordingAct, string keywords) {
      DataTable table = PropertyData.GetParties(partyFilterType, partyType, recordingAct, keywords);

      return new FixedList<Party>((x) => Party.Parse(x), table);
    }

    #endregion Constructors and parsers

    #region Public properties

    public string ExtendedName {
      get {
        string temp = this.FullName;

        if (this.RegistryID.Length != 0) {
          temp += " (" + this.RegistryID + ")";
        }
        if (this.RegistryLocation.IsEmptyInstance || 
            this.RegistryLocation.Equals(GeographicRegionItem.Unknown)) {
          return temp;
        } else {
          return temp + " " + this.RegistryLocation.FullName;
        }
      }
    }

    [DataField("FullName")]
    public virtual string FullName {
      get;
      set;
    }

    public virtual string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.FullName, this.Nicknames, this.ExtendedName);
      }
    }

    [DataField("Nicknames")]
    public string Nicknames {
      get;
      set;
    }

    [DataField("PartyNotes")]
    public string Notes {
      get;
      set;
    }

    [DataField("PostedById", Default = "Contacts.Person.Empty")]
    public Contact PostedBy {
      get;
      private set;
    }

    [DataField("PostingTime", Default = "DateTime.Now")]
    public DateTime PostingTime {
      get;
      private set;
    }

    [DataField("RegistryDate")]
    public DateTime RegistryDate {
      get;
      set;
    }

    public abstract string RegistryID {
      get;
    }

    [DataField("RegistryLocationId")]
    public GeographicRegionItem RegistryLocation {
      get;
      set;
    }

    [DataField("ReplacedById")]
    public int ReplacedById {
      get;
      private set;
    }

    [DataField("PartyShortName")]
    public string ShortName {
      get;
      set;
    }

    [DataField("PartyStatus", Default = PartyStatus.Pending)]
    public PartyStatus Status {
      get;
      private set;
    }

    public string StatusName {
      get {
        switch (this.Status) {
          case PartyStatus.Pending:
            return "Pendiente";
          case PartyStatus.Registered:
            return "Registrada";
          case PartyStatus.Closed:
            return "Cerrada";
          case PartyStatus.Deleted:
            return "Eliminada";
          default:
            return "No determinado";
        }
      }
    }

    [DataField("PartyTags")]
    public string Tags {
      get;
      set;
    }

    [DataField("TaxIDNumber")]
    public string TaxIDNumber {
      get;
      set;
    }

    #endregion Public properties

    #region Public methods

    internal void Delete() {
      this.Status = PartyStatus.Deleted;
      this.Save();
    }

    public RecordingActParty GetLastRecordingActParty(DateTime searchStartDate) {
      return PropertyData.GetLastRecordingActParty(this, searchStartDate);
    }

    protected override void OnSave() {
      if (base.IsNew) {
        this.PostedBy = Contact.Parse(ExecutionServer.CurrentUserId);
        this.PostingTime = DateTime.Now;
      }
    }

    #endregion Public methods

  } // class Party

} // namespace Empiria.Land.Registration
