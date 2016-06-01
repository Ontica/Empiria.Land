/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Party                                          Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Abstract class that represents a recording act party.                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Json;
using Empiria.Land.Registration.Data;
using Empiria.Ontology;

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

    protected Party(string fullName, string uid, string uidType) {
      fullName = EmpiriaString.TrimAll(fullName);
      uid = EmpiriaString.TrimAll(uid);

      Assertion.AssertObject(fullName, "fullName");
      Assertion.Assert(uidType == "None" || uid.Length != 0, "Party unique ID can't be empty.");

      this.FullName = fullName;
      this.UID = uid;
      this.UIDType = uidType;
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

        if (this.FullUID.Length != 0) {
          temp += " (" + this.FullUID + ")";
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

    public string UID {
      get;
      private set;
    } = String.Empty;


    public string UIDType {
      get;
      private set;
    } = String.Empty;


    public string FullUID {
      get {
        if (this.UID.Length == 0) {
          return String.Empty;
        }
        return this.UIDType + ": " + this.UID;
      }
    }

    protected internal virtual string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.FullName, this.UID);
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

      this.UID = json.Get<string>("UID", String.Empty);
      this.UIDType = json.Get<string>("UIDType", "None");
    }

    private string GetUIDAsJsonString() {
      if (this.UID.Length == 0) {
        return String.Empty;
      }

      var json = new JsonObject();

      json.Add(new JsonItem("UID", this.UID));
      json.Add(new JsonItem("UIDType", this.UIDType));

      return json.ToString();
    }

    #endregion Private methods

  } // class Party

} // namespace Empiria.Land.Registration
