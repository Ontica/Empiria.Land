/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Party                                          Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Abstract class that represents a recording act party.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Json;
using Empiria.Ontology;

using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Describes the recording status of a party's data.</summary>
  public enum PartyStatus {
    Pending = 'P',
    Registered = 'R',
    Closed = 'C',
    Deleted = 'X'
  }

  /// <summary>Used to describe the filtering mode to use on parties searching.</summary>
  public enum PartyFilterType {
    ByKeywords = 1,
    OnInscription = 2,
    Involved = 4,
  }

  /// <summary>Abstract class that represents a recording act party.</summary>
  public abstract class Party : BaseObject {

    #region Constructors and parsers

    protected Party() {
      // Required by Empiria Framework.
    }

    protected Party(string fullName, string officialID, string officialIDType) {
      fullName = EmpiriaString.TrimAll(fullName);
      officialID = EmpiriaString.TrimAll(officialID);

      Assertion.AssertObject(fullName, "fullName");
      Assertion.Assert(officialIDType == "None" || officialID.Length != 0, "Party unique ID can't be empty.");

      this.FullName = fullName;
      this.OfficialID = officialID;
      this.OfficialIDType = officialIDType;
    }

    static public Party Parse(int id) {
      return BaseObject.ParseId<Party>(id);
    }

    static public Party Empty {
      get {
        return BaseObject.ParseEmpty<HumanParty>();
      }
    }

    static public FixedList<Party> GetList(ObjectTypeInfo partyType, string keywords) {
      DataTable table = PartyData.GetParties(partyType, keywords);

      return BaseObject.ParseList<Party>(table).ToFixedList();
    }

    static public FixedList<Party> GetList(PartyFilterType partyFilterType, ObjectTypeInfo partyType,
                                           RecordingAct recordingAct, string keywords) {
      DataTable table = PartyData.GetParties(partyFilterType, partyType, recordingAct, keywords);

      return BaseObject.ParseList<Party>(table).ToFixedList();
    }

    protected override void OnLoadObjectData(DataRow row) {
      this.ReadUIDFromJson((string) row["PartyExtData"]);
    }

    #endregion Constructors and parsers

    #region Public properties

    public string ExtendedName {
      get {
        string temp = this.FullName;

        if (this.FullOfficialID.Length != 0) {
          temp += " (" + this.FullOfficialID + ")";
        }
        return temp;
      }
    }


    [DataField("PartyFullName")]
    public string FullName {
      get;
      private set;
    } = String.Empty;


    [DataField("PartyNotes")]
    public string Notes {
      get;
      set;
    } = String.Empty;


    internal string ExtendedData {
      get {
        return this.GetUIDAsJsonString();
      }
    }

    public string OfficialID {
      get;
      private set;
    } = String.Empty;


    public string OfficialIDType {
      get;
      private set;
    } = String.Empty;


    public string FullOfficialID {
      get {
        if (this.OfficialID.Length == 0) {
          return String.Empty;
        }
        return this.OfficialIDType + ": " + this.OfficialID;
      }
    }

    protected internal virtual string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.FullName, this.OfficialID);
      }
    }


    [DataField("PartyStatus", Default = PartyStatus.Pending)]
    public PartyStatus Status {
      get;
      private set;
    } = PartyStatus.Pending;


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

    #endregion Public properties

    #region Public methods

    internal void Delete() {
      this.Status = PartyStatus.Deleted;
      this.Save();
    }

    public FixedList<RecordingActParty> GetRecordingActs() {
      return PartyData.GetRecordingActs(this);
    }

    protected override void OnSave() {
      PartyData.WriteParty(this);
    }

    #endregion Public methods

    #region Private methods

    private void ReadUIDFromJson(string jsonString) {
      var json = JsonObject.Parse(jsonString);

      this.OfficialID = json.Get<string>("UID", String.Empty);
      this.OfficialIDType = json.Get<string>("UIDType", "None");
    }

    private string GetUIDAsJsonString() {
      if (this.OfficialID.Length == 0) {
        return String.Empty;
      }

      var json = new JsonObject();

      json.Add("UID", this.OfficialID);
      json.Add("UIDType", this.OfficialIDType);

      return json.ToString();
    }

    #endregion Private methods

  } // class Party

} // namespace Empiria.Land.Registration
