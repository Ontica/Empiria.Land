﻿/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : Party                                          Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 25/Jun/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Abstract class that represents a recording act party.                                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
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
    //OnInscription = 2,
    Involved = 4,
  }

  /// <summary>Abstract class that represents a recording act party.</summary>
  public abstract class Party : BaseObject {

    #region Constructors and parsers

    protected Party() {
      // Required by Empiria Framework.
    }

    protected Party(string UID, string fullName) {
      this.UID = UID;
      this.FullName = fullName;
    }

    static public Party Parse(int id) {
      return BaseObject.ParseId<Party>(id);
    }

    static public FixedList<Party> GetList(ObjectTypeInfo partyType, string keywords) {
      DataTable table = PropertyData.GetParties(partyType, keywords);

      return BaseObject.ParseList<Party>(table).ToFixedList();
    }

    static public FixedList<Party> GetList(PartyFilterType partyFilterType, ObjectTypeInfo partyType,
                                           RecordingAct recordingAct, string keywords) {
      DataTable table = PropertyData.GetParties(partyFilterType, partyType, recordingAct, keywords);

      return BaseObject.ParseList<Party>(table).ToFixedList();
    }

    #endregion Constructors and parsers

    #region Public properties

    public string ExtendedName {
      get {
        string temp = this.FullName;

        if (this.UID.Length != 0) {
          temp += " (" + this.UID + ")";
        }
        return temp;
      }
    }

    [DataField("PartyFullName")]
    public string FullName {
      get;
      private set;
    }

    [DataField("PartyUID")]
    public string UID {
      get;
      private set;
    }

    [DataField("PartyExtData")]
    public string ExtendedData {
      get;
      private set;
    }

    protected internal virtual string Keywords {
      get {
        return EmpiriaString.BuildKeywords(this.UID, this.FullName);
      }
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

    #endregion Public properties

    #region Public methods

    internal void Delete() {
      this.Status = PartyStatus.Deleted;
      this.Save();
    }

    public RecordingActParty GetLastRecordingActParty(DateTime searchStartDate) {
      return PropertyData.TryGetLastRecordingActParty(this, searchStartDate);
    }

    #endregion Public methods

  } // class Party

} // namespace Empiria.Land.Registration
