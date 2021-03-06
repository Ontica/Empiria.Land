﻿/* Empiria Land **********************************************************************************************
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
using Empiria.Land.Registration.Adapters;

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

    protected Party(string fullName) {
      this.FullName = EmpiriaString.TrimAll(fullName);
    }


    static public Party Parse(int id) {
      return BaseObject.ParseId<Party>(id);
    }


    static public Party Parse(string uid) {
      return BaseObject.ParseKey<Party>(uid);
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

    static public FixedList<Party> GetList(SearchPartiesCommand command) {
      DataTable table = PartyData.GetParties(command);

      return BaseObject.ParseList<Party>(table).ToFixedList();
    }


    #endregion Constructors and parsers

    #region Public properties

    [DataField("PartyFullName")]
    public string FullName {
      get;
      private set;
    } = String.Empty;


    [DataField("PartyNotes")]
    public string Notes {
      get;
      protected set;
    } = String.Empty;

    [DataField("PartyPrimaryID")]
    public string RFC {
      get;
      protected set;
    } = String.Empty;


    protected internal virtual string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.FullName, this.RFC);
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

  } // class Party

} // namespace Empiria.Land.Registration
