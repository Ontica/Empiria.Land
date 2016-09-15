/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Registration.Data               Assembly : Empiria.Land.Registration             *
*  Type      : RecordingActsData                            Pattern  : Data Services                         *
*  Version   : 2.1                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording acts.                                  *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Data;

using Empiria.Land.Data;

namespace Empiria.Land.Registration.Data {

  /// <summary>Provides database read and write methods for recording acts.</summary>
  static public class RecordingActsData {

    #region Public methods

    /// <summary>Gets those recording acts where the given resource appears as
    ///  the main resource (ResourceId field).</summary>
    static public FixedList<RecordingAct> GetResourceRecordingActList(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var operation = DataOperation.Parse("qryLRSResourceRecordingActs", resource.Id);

      return DataReader.GetList<RecordingAct>(operation,
                                              (x) => BaseObject.ParseList<RecordingAct>(x)).ToFixedList();
    }

    /// <summary>Gets those recording acts where the given resource appears as
    ///  the main resource or as the related resource.</summary>
    internal static FixedList<RecordingAct> GetResourceFullTractIndex(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var operation = DataOperation.Parse("qryLRSResourceFullTractIndex", resource.Id);

      return DataReader.GetList<RecordingAct>(operation,
                                              (x) => BaseObject.ParseList<RecordingAct>(x)).ToFixedList();
    }

    static internal FixedList<IResourceTractItem> GetResourceFullTractIndexWithCertificates(Resource resource) {
      var list = new List<IResourceTractItem>();

      var recordingActs = RecordingActsData.GetResourceFullTractIndex(resource);
      var certificates = CertificatesData.ResourceEmittedCertificates(resource);

      list.AddRange(recordingActs);
      list.AddRange(certificates);
      list.Sort((x, y) => x.TractPrelationStamp.CompareTo(y.TractPrelationStamp));

      return list.ToFixedList();
    }

    static public FixedList<RecordingAct> GetResourceRecordingActListUntil(Resource resource, RecordingAct breakAct,
                                                                           bool includeBreakAct) {
      if (resource.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }

      var operation = DataOperation.Parse("qryLRSResourceRecordingActs", resource.Id);
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

    static internal int WriteRecordingAct(RecordingAct o) {
      Assertion.Assert(o.Id != 0, "RecordingAct.Id can't be zero");
      Assertion.Assert(!o.Resource.IsEmptyInstance && !o.Resource.IsNew,
                       "Resource can't be new or the empty instance.");
      Assertion.Assert(!o.RelatedResource.IsNew, "Related resource was not saved.");
      Assertion.Assert(!o.Document.IsEmptyInstance, "Document can't be the empty instance.");
      Assertion.Assert(!o.Document.IsNew, "Document should be saved before add recording acts to it.");

      var op = DataOperation.Parse("writeLRSRecordingAct", o.Id,
                      o.RecordingActType.Id, o.Document.Id, o.Index,
                      o.Resource.Id, (char) o.ResourceRole, o.RelatedResource.Id, o.Percentage,
                      o.Notes, o.ExtensionData.ToString(), o.Keywords,
                      o.AmendmentOf.Id, o.AmendedBy.Id, o.PhysicalRecording.Id,
                      o.RegisteredBy.Id, o.RegistrationTime, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      return DataWriter.Execute(op);
    }

    static internal int WriteRecordingActParty(RecordingActParty o) {
      var op = DataOperation.Parse("writeLRSRecordingActParty", o.Id,
                    o.RecordingAct.Id, o.Party.Id, o.PartyRole.Id, o.PartyOf.Id,
                    o.OwnershipPart.Amount, o.OwnershipPart.Unit.Id, o.IsOwnershipStillActive,
                    o.Notes, o.AsText, o.ExtendedData, o.PostedBy.Id,
                    (char) o.Status, o.IntegrityHashCode);

      return DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class RecordingActsData

} // namespace Empiria.Land.Registration.Data
