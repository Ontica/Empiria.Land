/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : RecordingActsData                            Pattern  : Data Services Static Class            *
*  Version   : 1.5        Date: 25/Jun/2014                 License  : GNU AGPLv3  (See license.txt)         *
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

    static public ObjectList<RecordingAct> GetPropertyRecordingActList(Property property) {
      var view = DataReader.GetDataView(DataOperation.Parse("qryLRSPropertyRecordingActs", property.Id));

      return new ObjectList<RecordingAct>((x) => RecordingAct.Parse(x), view);
    }

    static public ObjectList<RecordingAct> GetPropertyRecordingActListUntil(Property property, RecordingAct breakAct, 
                                                                            bool includeBreakAct) {
      DataOperation op = DataOperation.Parse("qryLRSPropertyRecordingActs", property.Id);
      DataView view = DataReader.GetDataView(op);
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
      return new ObjectList<RecordingAct>(list);
    }

    static public ObjectList<RecordingAct> GetRecordingActs(Recording recording) {
      DataView view = DataReader.GetDataView(DataOperation.Parse("qryLRSRecordingRecordedActs", recording.Id));

      return new ObjectList<RecordingAct>((x) => RecordingAct.Parse(x), view);
    }

    static public ObjectList<RecordingAct> GetRecordingActs(LRSTransaction transaction) {
      string sql = "SELECT * FROM vwLRSRecordingActs" + 
                   " WHERE TransactionId = "+ transaction.Id + 
                   " ORDER BY RecordingActIndex, PostingTime";

      var view = DataReader.GetDataView(DataOperation.Parse(sql));

      return new ObjectList<RecordingAct>((x) => RecordingAct.Parse(x), view);
    }

    static public ObjectList<TractIndexItem> GetPropertiesEventsList(RecordingAct recordingAct) {
      DataOperation operation = DataOperation.Parse("qryLRSRecordingActPropertiesEvents", recordingAct.Id);

      DataView view = DataReader.GetDataView(operation);

      return new ObjectList<TractIndexItem>((x) => TractIndexItem.Parse(x), view);
    }

    static internal ObjectList<RecordingActParty> GetInvolvedDomainParties(RecordingAct recordingAct) {
      string sql = String.Empty;
      if (!recordingAct.IsAnnotation) {
        sql = "SELECT * FROM LRSRecordingActParties " +
              "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
              "AND PartyRoleId <> -1 AND LinkStatus <> 'X'";

      } else {
        string ids = String.Empty;
        ObjectList<TractIndexItem> events = recordingAct.PropertiesEvents;
        for (int i = 0; i < events.Count; i++) {
          ObjectList<RecordingAct> acts = events[i].Property.GetRecordingActsTract();

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
      DataOperation operation = DataOperation.Parse(sql);

      return new ObjectList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal ObjectList<RecordingActParty> GetRecordingActPartiesList(RecordingAct recordingAct) {
      string sql = String.Empty;

      sql = "SELECT * FROM LRSRecordingActParties " +
            "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
            "AND LinkStatus <> 'X'";
      DataOperation operation = DataOperation.Parse(sql);

      return new ObjectList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal ObjectList<RecordingActParty> GetDomainPartyList(RecordingAct recordingAct) {
      string sql = String.Empty;

      sql = "SELECT * FROM LRSRecordingActParties " +
            "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
            "AND PartyRoleId <> -1 AND LinkStatus <> 'X'";

      DataOperation operation = DataOperation.Parse(sql);

      return new ObjectList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }

    static internal ObjectList<Property> GetRecordingActPropertiesList(RecordingAct recordingAct) {
      DataOperation operation = DataOperation.Parse("qryLRSRecordingActProperties", recordingAct.Id);

      return new ObjectList<Property>((x) => Property.Parse(x), DataReader.GetDataView(operation));
    }

    static internal ObjectList<RecordingActParty> GetSecondaryPartiesList(RecordingAct recordingAct) {
      string sql = "SELECT * FROM LRSRecordingActParties " +
                   "WHERE RecordingActId = " + recordingAct.Id.ToString() + " " +
                   "AND SecondaryPartyRoleId <> -1 AND LinkStatus <> 'X'";

      DataOperation operation = DataOperation.Parse(sql);

      return new ObjectList<RecordingActParty>((x) => RecordingActParty.Parse(x), DataReader.GetDataView(operation));
    }


    static internal int WriteRecordingAct(RecordingAct o) {
      Assertion.Require(o.Id != 0, "RecordingAct.Id can't be zero");
      var dataOperation = DataOperation.Parse("writeLRSRecordingAct", o.Id, o.RecordingActType.Id,
                                               o.Transaction.Id, o.Document.Id, o.TargetRecordingAct.Id, 
                                               o.Recording.Id, o.Index, o.Notes, o.ExtensionData.ToJson(), 
                                               o.Keywords, o.CanceledBy.Id, o.CancelationTime, 
                                               o.PostedBy.Id, o.PostingTime, (char) o.Status,
                                               o.Integrity.GetUpdatedHashCode(),
                                               o.ExtensionData.AppraisalAmount.Amount,
                                               o.ExtensionData.AppraisalAmount.Currency.Id,
                                               o.ExtensionData.OperationAmount.Amount,
                                               o.ExtensionData.OperationAmount.Currency.Id,
                                               o.ExtensionData.Contract.Interest.TermPeriods,
                                               o.ExtensionData.Contract.Interest.TermUnit.Id,
                                               o.ExtensionData.Contract.Interest.Rate,
                                               o.ExtensionData.Contract.Interest.RateType.Id,
                                               o.ExtensionData.Contract.Date,
                                               o.ExtensionData.Contract.Place.Id,
                                               o.ExtensionData.Contract.Number);
      return DataWriter.Execute(dataOperation);
    }

    #endregion Public methods


    internal static List<TractIndexItem> GetTractIndex(RecordingAct recordingAct) {
      return new List<TractIndexItem>();
    }

  } // class RecordingActsData

} // namespace Empiria.Land.Registration.Registration.Data
