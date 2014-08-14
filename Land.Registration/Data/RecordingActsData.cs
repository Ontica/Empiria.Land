/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : RecordingActsData                            Pattern  : Data Services Static Class            *
*  Version   : 2.0        Date: 23/Oct/2014                 License  : GNU AGPLv3  (See license.txt)         *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording acts.                                  *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Data;
using Empiria.Land.Registration.Transactions;


namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read and write methods for recording acts.</summary>
  static public class RecordingActsData {

    #region Public methods

    static public FixedList<RecordingAct> GetPropertyRecordingActList(Property property) {
      var operation = DataOperation.Parse("qryLRSPropertyRecordingActs", property.Id);

      return DataReader.GetFixedList<RecordingAct>(operation, (x) => RecordingAct.Parse(x));
    }

    static public FixedList<RecordingAct> GetPropertyRecordingActListUntil(Property property, RecordingAct breakAct, 
                                                                            bool includeBreakAct) {
      var operation = DataOperation.Parse("qryLRSPropertyRecordingActs", property.Id);
      DataView view = DataReader.GetDataView(operation);
      List<RecordingAct> list = new List<RecordingAct>();
      foreach (DataRowView row in view) {
        var recordingAct = RecordingAct.Parse(row.Row);
        if (recordingAct.Equals(breakAct)) {
          if (includeBreakAct) {
            list.Add(RecordingAct.Parse(row.Row));
          }
          break;
        } else {
          list.Add(RecordingAct.Parse(row.Row));
        }
      }
      return new FixedList<RecordingAct>(list);
    }

    static public FixedList<RecordingAct> GetRecordingActs(Recording recording) {
      var operation = DataOperation.Parse("qryLRSRecordingRecordedActs", recording.Id);

      return DataReader.GetFixedList<RecordingAct>(operation, (x) => RecordingAct.Parse(x));
    }

    static public FixedList<RecordingAct> GetRecordingActs(RecordingDocument document) {
      if (document.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      string sql = "SELECT * FROM LRSRecordingActs" +
                  " WHERE DocumentId = " + document.Id +
                  " ORDER BY RecordingActIndex, RegistrationTime";
      var operation = DataOperation.Parse(sql);

      return DataReader.GetList<RecordingAct>(operation, (x) => RecordingAct.Parse(x)).ToFixedList();
    }

    static internal List<TractIndexItem> GetTractIndex(RecordingAct recordingAct) {
      if (recordingAct.IsEmptyInstance) {
        return new List<TractIndexItem>();
      }
     
      var operation = DataOperation.Parse("qryLRSRecordingActTractIndex", recordingAct.Id);   
    
      return DataReader.GetList<TractIndexItem>(operation, (x) => TractIndexItem.Parse(x));
    }

    static internal FixedList<RecordingActParty> GetInvolvedDomainParties(RecordingAct recordingAct) {
      string sql = String.Empty;
      if (!recordingAct.IsAnnotation) {
        sql = "SELECT * FROM LRSRecordingActParties " +
              "WHERE RecordingActId = " + recordingAct.Id + " " +
              "AND PartyRoleId <> -1 AND LinkStatus <> 'X'";
      } else {
        string ids = String.Empty;
        FixedList<TractIndexItem> events = recordingAct.TractIndex;
        for (int i = 0; i < events.Count; i++) {
          FixedList<RecordingAct> acts = events[i].Property.GetRecordingActsTract();

          for (int j = 0; j < acts.Count; j++) {
            if (ids.Length != 0) {
              ids += ",";
            }
            ids += acts[j].Id.ToString();
          }
        }
        sql = "SELECT DISTINCT * " +
              "FROM LRSRecordingActParties " +
              "WHERE (RecordingActId IN (" + ids + ") AND PartyRoleId <> -1) " +
              "AND (LinkStatus <> 'X')";
      }
      var operation = DataOperation.Parse(sql);

      return new FixedList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal FixedList<RecordingActParty> GetRecordingActPartiesList(RecordingAct recordingAct) {
      string sql = String.Empty;

      sql = "SELECT * FROM LRSRecordingActParties " +
            "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
            "AND LinkStatus <> 'X'";
      DataOperation operation = DataOperation.Parse(sql);

      return new FixedList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal FixedList<RecordingActParty> GetDomainPartyList(RecordingAct recordingAct) {
      string sql = String.Empty;

      sql = "SELECT * FROM LRSRecordingActParties " +
            "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
            "AND PartyRoleId <> -1 AND LinkStatus <> 'X'";

      var operation = DataOperation.Parse(sql);

      return new FixedList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal FixedList<Property> GetRecordingActPropertiesList(RecordingAct recordingAct) {
      var operation = DataOperation.Parse("qryLRSRecordingActProperties", recordingAct.Id);

      return new FixedList<Property>((x) => Property.Parse(x), DataReader.GetDataView(operation));
    }

    static internal FixedList<RecordingActParty> GetSecondaryPartiesList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND SecondaryPartyRoleId <> -1 AND LinkStatus <> 'X'";

      var operation = DataOperation.Parse(sql);

      return new FixedList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }


    static internal int WriteRecordingAct(RecordingAct o) {
      Assertion.Assert(o.Id != 0, "RecordingAct.Id can't be zero");
      var operation = DataOperation.Parse("writeLRSRecordingAct", o.Id, o.RecordingActType.Id,
                                          o.Document.Id, o.Recording.Id, o.Index, o.Notes, 
                                          o.ExtensionData.ToJson(), o.Keywords, o.AmendmentOf.Id, 
                                          o.AmendedBy.Id, o.RegisteredBy.Id, o.RegistrationTime, 
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    #endregion Public methods

  } // class RecordingActsData

} // namespace Empiria.Land.Registration.Registration.Data
