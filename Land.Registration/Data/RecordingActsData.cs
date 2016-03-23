/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : RecordingActsData                            Pattern  : Data Services                         *
*  Version   : 2.0                                          License  : Please read license.txt file          *
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

    static public FixedList<RecordingAct> GetResourceRecordingActList(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }
      var operation = DataOperation.Parse("qryLRSPropertyRecordingActs", resource.Id);

      return DataReader.GetList<RecordingAct>(operation,
                                              (x) => BaseObject.ParseList<RecordingAct>(x)).ToFixedList();
    }

    static public FixedList<RecordingAct> GetResourceRecordingActListUntil(Resource resource, RecordingAct breakAct,
                                                                           bool includeBreakAct) {
      var operation = DataOperation.Parse("qryLRSPropertyRecordingActs", resource.Id);
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
      if (recording.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }
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

    static internal List<TractItem> GetRecordingActTargets(RecordingAct recordingAct) {
      if (recordingAct.IsEmptyInstance) {
        return new List<TractItem>();
      }

      var operation = DataOperation.Parse("qryLRSTractIndexByRecordingAct", recordingAct.Id);

      return DataReader.GetList<TractItem>(operation, (x) => BaseObject.ParseList<TractItem>(x));
    }

    static internal FixedList<RecordingActParty> GetInvolvedDomainParties(RecordingAct recordingAct) {
      throw new NotImplementedException("OOJJOO");

      //string sql = String.Empty;
      //if (!recordingAct.IsAnnotation) {
      //  sql = "SELECT * FROM LRSRecordingActParties " +
      //        "WHERE RecordingActId = " + recordingAct.Id + " " +
      //        "AND PartyRoleId <> -1 AND LinkStatus <> 'X'";
      //} else {
      //  string ids = String.Empty;
      //  for (int i = 0; i < recordingAct.Targets.Count; i++) {
      //    FixedList<RecordingAct> acts = recordingAct.Targets[i].Resource.GetRecordingActsTract();

      //    for (int j = 0; j < acts.Count; j++) {
      //      if (ids.Length != 0) {
      //        ids += ",";
      //      }
      //      ids += acts[j].Id.ToString();
      //    }
      //  }
      //  sql = "SELECT DISTINCT * " +
      //        "FROM LRSRecordingActParties " +
      //        "WHERE (RecordingActId IN (" + ids + ") AND PartyRoleId <> -1) " +
      //        "AND (LinkStatus <> 'X')";
      //}

      //return DataReader.GetList<RecordingActParty>(DataOperation.Parse(sql),
      //                                            (x) => BaseObject.ParseList<RecordingActParty>(x)).ToFixedList();
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

    static internal FixedList<RealEstate> GetRecordingActPropertiesList(RecordingAct recordingAct) {
      var operation = DataOperation.Parse("qryLRSRecordingActProperties", recordingAct.Id);


      return DataReader.GetList<RealEstate>(operation,
                                         (x) => BaseObject.ParseList<RealEstate>(x)).ToFixedList();
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
                                          o.Keywords, o.AmendmentOf.Id, o.AmendedBy.Id, o.PhysicalRecording.Id,
                                          o.RegisteredBy.Id, o.RegistrationTime,
                                          (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    static internal int WriteRecordingActParty(RecordingActParty o) {
      var dataOperation = DataOperation.Parse("writeLRSRecordingActParty", o.Id, o.RecordingAct.Id,
                                               o.Party.Id, o.PartyRole.Id, o.SecondaryParty.Id,
                                               o.SecondaryPartyRole.Id, o.Notes, (char) o.OwnershipMode,
                                               o.OwnershipPart.Amount, o.OwnershipPart.Unit.Id,
                                               (char) o.UsufructMode, o.UsufructTerm,
                                               o.PartyOccupation.Id, o.PartyMarriageStatus.Id,
                                               o.PartyAddress, o.PartyAddressPlace.Id, o.PostedBy.Id,
                                               o.PostingTime, (char) o.Status, o.IntegrityHashCode);
      return DataWriter.Execute(dataOperation);
    }

    static internal int WriteTractItem(TractItem o) {
      Assertion.Assert(o.Id != 0, "TractItem.Id can't be zero");
      Assertion.Assert(o.Resource.Id > 0, "Resource Id must be positive.");
      Assertion.Assert(o.RecordingAct.Id > 0, "Recording act Id must be positive.");

      var operation = DataOperation.Parse("writeLRSTractItem", o.Id, o.GetEmpiriaType().Id,
                      o.RecordingAct.Id, o.Resource.Id, (char) o.ResourceRole,
                      -1, String.Empty, o.RecordingActPercentage,
                      -1, -1, -1, o.LastAmendedBy.Id, o.ExtensionData.ToJson(),
                      o.RegisteredBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    static internal int WriteStructureTractItem(StructureTractItem o) {
      Assertion.Assert(o.Id != 0, "TractItem.Id can't be zero");
      Assertion.Assert(o.Resource.Id > 0, "Resource Id must be positive.");
      Assertion.Assert(o.RecordingAct.Id > 0, "Recording act Id must be positive.");

      var operation = DataOperation.Parse("writeLRSTractItem", o.Id, o.GetEmpiriaType().Id,
                      o.RecordingAct.Id, o.Resource.Id, (char) o.ResourceRole,
                      o.RelatedRealEstate.Id, o.PartitionName, o.RecordingActPercentage,
                      -1, -1, -1, o.LastAmendedBy.Id, o.ExtensionData.ToJson(),
                      o.RegisteredBy.Id, (char) o.Status, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    #endregion Public methods

  } // class RecordingActsData

} // namespace Empiria.Land.Registration.Data
