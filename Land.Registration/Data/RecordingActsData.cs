/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                 System   : Land Registration System              *
*  Namespace : Empiria.Land.Data                            Assembly : Empiria.Land.Registration             *
*  Type      : RecordingActsData                            Pattern  : Data Services                         *
*  Version   : 3.0                                          License  : Please read license.txt file          *
*                                                                                                            *
*  Summary   : Provides database read and write methods for recording acts.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Registration;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording acts.</summary>
  static public class RecordingActsData {

    #region Public methods

    static public FixedList<RecordingAct> GetBookEntryRecordedActs(BookEntry bookEntry) {
      if (bookEntry.IsEmptyInstance) {
        return new FixedList<RecordingAct>();
      }
      var operation = DataOperation.Parse("qryLRSPhysicalRecordingRecordedActs", bookEntry.Id);

      return DataReader.GetFixedList<RecordingAct>(operation);
    }


    internal static void UpdateRecordingActResourceSnapshot(RecordingAct recordingAct) {
      var op = DataOperation.Parse("doLRSUpdateRecordingActResourceExtData",
                                   recordingAct.Id, recordingAct.ResourceShapshotData.ToString());

      DataWriter.Execute(op);
    }


    static internal void WriteRecordingAct(RecordingAct o) {
      Assertion.Require(o.Id != 0, "RecordingAct.Id can't be zero");
      Assertion.Require(!o.Resource.IsEmptyInstance && !o.Resource.IsNew,
                       "Resource can't be new or the empty instance.");
      Assertion.Require(!o.RelatedResource.IsNew, "Related resource was not saved.");
      Assertion.Require(!o.LandRecord.IsEmptyInstance, "Document can't be the empty instance.");
      Assertion.Require(!o.LandRecord.IsNew, "Document should be saved before add recording acts to it.");

      var op = DataOperation.Parse("writeLRSRecordingAct", o.Id, o.UID,
                      o.RecordingActType.Id, o.LandRecord.Id, o.Index,
                      o.Resource.Id, (char) o.ResourceRole, o.RelatedResource.Id, o.Percentage,
                      o.Kind, o.OperationAmount, o.OperationCurrency.Id, o.Summary,
                      o.Notes, /* o.ResourceExtData, o.ExtensionData.ToString(), */ o.Keywords,
                      o.AmendmentOf.Id, o.AmendedBy.Id, o.BookEntry.Id,
                      o.RegisteredBy.Id, o.RegistrationTime, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    static internal void WriteRecordingActParty(RecordingActParty o) {
      var op = DataOperation.Parse("writeLRSRecordingActParty", o.Id, o.UID,
                      o.RecordingAct.Id, o.Party.Id, o.PartyRole.Id, o.PartyOf.Id,
                      o.OwnershipPart.Amount, o.OwnershipPart.Unit.Id, o.IsOwnershipStillActive,
                      o.Notes, o.AsText, o.ExtendedData, o.PostedBy.Id,
                      (char) o.Status, o.IntegrityHashCode);

      DataWriter.Execute(op);
    }

    internal static void UpdateRecordingActType(RecordingAct recordingAct) {
      Assertion.Require(recordingAct.IsEditable,
                        $"Recording act '{recordingAct.UID}' cannot be edited.");

      var sql = "UPDATE LRSRecordingActs " +
               $"SET RecordingActTypeId = {recordingAct.RecordingActType.Id} " +
               $"WHERE RecordingActId = {recordingAct.Id}";

      DataWriter.Execute(DataOperation.Parse(sql));
    }

    #endregion Public methods

  } // class RecordingActsData

} // namespace Empiria.Land.Data
