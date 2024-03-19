/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recording services                           Component : Data services                         *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Data services provider                *
*  Type     : LandRecordsData                              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides database read and write methods for land records.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Data;

using Empiria.Land.Registration;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for land records.</summary>
  static internal class LandRecordsData {

    #region Methods

    static public FixedList<RecordingActParty> GetLandRecordPartyList(LandRecord landRecord, Party party) {
      string sql = "SELECT LRSRecordingActParties.* " +
                   "FROM LRSRecordingActParties INNER JOIN LRSRecordingActs " +
                   "ON LRSRecordingActParties.RecordingActId = LRSRecordingActs.RecordingActId " +
                  $"WHERE (LRSRecordingActs.DocumentId = {landRecord.Id}) " +
                   "AND (LRSRecordingActParties.RecActPartyStatus <> 'X') " +
                  $"AND (LRSRecordingActParties.PartyId = {party.Id} " +
                  $"OR LRSRecordingActParties.PartyOfId = {party.Id})";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<RecordingActParty>(operation);
    }


    static internal List<RecordingAct> GetLandRecordRecordingActs(LandRecord landRecord) {
      if (landRecord.IsEmptyInstance) {
        return new List<RecordingAct>();
      }

      string sql = "SELECT * FROM LRSRecordingActs " +
                   $"WHERE DocumentId = {landRecord.Id} AND RecordingActStatus <> 'X' " +
                   "ORDER BY RecordingActIndex, RegistrationTime";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetList<RecordingAct>(operation);
    }


    static internal string GetNextImagingControlID(LandRecord landRecord) {
      string prefix = landRecord.AuthorizationTime.ToString("yyyy-MM");

      var sql = "SELECT MAX(ImagingControlID) " +
                $"FROM LRSLandRecords " +
                $"WHERE ImagingControlID LIKE '{prefix}-%'";

      var imagingControlID = DataReader.GetScalar<String>(DataOperation.Parse(sql), String.Empty);

      if (imagingControlID != String.Empty) {
        var counter = int.Parse(imagingControlID.Split('-')[2]);
        counter++;
        return prefix + "-" + counter.ToString("00000");
      } else {
        return prefix + "-" + 1.ToString("00000");
      }
    }


    static internal void SaveImagingControlID(LandRecord landRecord) {
      var op = DataOperation.Parse("setLRSLandRecordImagingControlID",
                                   landRecord.Id, landRecord.ImagingControlID);

      DataWriter.Execute(op);
    }


    static internal void SaveSecurityData(LandRecord o) {
      var op = DataOperation.Parse("setLRSLandRecordSecurityData", o.Id, o.GUID,
                                   (char) o.SecurityData.SignStatus, (char) o.SecurityData.SignType,
                                   o.SecurityData.SignedBy.Id, o.SecurityData.SignedTime,
                                   Security.Cryptographer.Encrypt(Security.EncryptionMode.Standard,
                                                                  o.SecurityData.ExtData.ToString()),
                                   o.Security.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    static internal void WriteLandRecord(LandRecord o) {
      var op = DataOperation.Parse("writeLRSLandRecord", o.Id, o.GUID, o.UID, o.Instrument.Id,
                                   o.Transaction.Id, o.PresentationTime, o.AuthorizationTime,
                                   o.AuthorizedBy.Id, o.Keywords, o.PostedBy.Id, o.PostingTime,
                                   (char) o.Status, o.Security.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

    #endregion Methods

  } // class LandRecordsData

} // namespace Empiria.Land.Data
