﻿/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : RecordingActsData                            Pattern  : Data Services                         *
*  Version   : 2.0        Date: 04/Jan/2015                 License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording acts.                                  *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
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

      return DataReader.GetList<RecordingAct>(operation,
                                              (x) => BaseObject.ParseList<RecordingAct>(x)).ToFixedList();
    }

    static public FixedList<RecordingAct> GetPropertyRecordingActListUntil(Property property, RecordingAct breakAct,
                                                                           bool includeBreakAct) {
      var operation = DataOperation.Parse("qryLRSPropertyRecordingActs", property.Id);
      DataTable table = DataReader.GetDataTable(operation);
      List<RecordingAct> list = new List<RecordingAct>();
      foreach (DataRow row in table.Rows) {
        var recordingAct = BaseObject.ParseDataRow<RecordingAct>(row);
        Assertion.Assert(recordingAct.Document.Id > 0, "DocId not valid");
        if (recordingAct.Equals(breakAct)) {
          if (includeBreakAct) {
            list.Add(BaseObject.ParseDataRow<RecordingAct>(row));
          }
          break;
        } else {
          list.Add(BaseObject.ParseDataRow<RecordingAct>(row));
        }
      }
      return list.ToFixedList();
    }

    static public FixedList<RecordingAct> GetRecordingActs(Recording recording) {
      var operation = DataOperation.Parse("qryLRSPhysicalRecordingRecordedActs", recording.Id);

      return DataReader.GetList<RecordingAct>(operation,
                                              (x) => BaseObject.ParseList<RecordingAct>(x)).ToFixedList();
    }

    static internal List<RecordingAct> GetRecordingActs(RecordingDocument document) {
      if (document.IsEmptyInstance) {
        return new List<RecordingAct>();
      }

      string sql = "SELECT * FROM LRSRecordingActs " +
                   "WHERE DocumentId = {0} AND RecordingActStatus <> 'X' " +
                   "ORDER BY RecordingActIndex, RegistrationTime";
      sql = String.Format(sql, document.Id);

      var operation = DataOperation.Parse(sql);

      return DataReader.GetList<RecordingAct>(operation, (x) => BaseObject.ParseList<RecordingAct>(x));
    }

    static internal List<TractIndexItem> GetTractIndex(RecordingAct recordingAct) {
      if (recordingAct.IsEmptyInstance) {
        return new List<TractIndexItem>();
      }

      var operation = DataOperation.Parse("qryLRSRecordingActTractIndex", recordingAct.Id);

      return DataReader.GetList<TractIndexItem>(operation, (x) => BaseObject.ParseList<TractIndexItem>(x));
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

      return DataReader.GetList<RecordingActParty>(DataOperation.Parse(sql),
                                        (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }

    static internal FixedList<RecordingActParty> GetRecordingActPartiesList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND LinkStatus <> 'X'";

      return DataReader.GetList<RecordingActParty>(DataOperation.Parse(sql),
                                        (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }

    static internal FixedList<RecordingActParty> GetDomainPartyList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND PartyRoleId <> -1 AND LinkStatus <> 'X'";

      return DataReader.GetList<RecordingActParty>(DataOperation.Parse(sql),
                                        (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }

    static internal FixedList<Property> GetRecordingActPropertiesList(RecordingAct recordingAct) {
      var operation = DataOperation.Parse("qryLRSRecordingActProperties", recordingAct.Id);


      return DataReader.GetList<Property>(operation,
                                         (x) => BaseObject.ParseList<Property>(x)).ToFixedList();
    }

    static internal FixedList<RecordingActParty> GetSecondaryPartiesList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND SecondaryPartyRoleId <> -1 AND LinkStatus <> 'X'";

      return DataReader.GetList<RecordingActParty>(DataOperation.Parse(sql),
                                        (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
    }


    static internal int WriteRecordingAct(RecordingAct o) {
      Assertion.Assert(o.Id != 0, "RecordingAct.Id can't be zero");
      Assertion.Assert(!o.Document.IsEmptyInstance, "Document can't be the empty instance.");
      Assertion.Assert(!o.Document.IsNew, "Document should be saved before add recording acts to it.");

      var operation = DataOperation.Parse("writeLRSRecordingAct", o.Id, o.RecordingActType.Id,
                                          o.Document.Id, o.Index, o.Notes, o.ExtensionData.ToJson(),
                                          o.Keywords, o.AmendmentOf.Id, o.AmendedBy.Id, o.Recording.Id,
                                          o.RegisteredBy.Id, o.RegistrationTime,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    #endregion Public methods

  } // class RecordingActsData

} // namespace Empiria.Land.Registration.Data
