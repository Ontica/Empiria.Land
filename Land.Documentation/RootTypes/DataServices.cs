/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : DataServices                                   Pattern  : Data Services Static Class          *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Imaging service for Empiria Land System.                                                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Data;

namespace Empiria.Land.Documentation {

  /// <summary>Imaging service for Empiria Land System.</summary>
  static internal class DataServices {

    #region Public methods

    static internal bool DocumentWasDigitalized(string documentKey) {
      string sql = "SELECT * FROM vwLRSImagingItems WHERE DocumentUniqueCode = '{0}'";

      sql = String.Format(sql, documentKey);

      return (DataReader.Count(DataOperation.Parse(sql)) > 0);
    }

    static internal int TryGetDocumentId(string documentKey) {
      string sql = "SELECT DocumentId FROM LRSDocuments WHERE " +
                   "(DocumentUniqueCode = '{0}') AND (DocumentStatus <> 'X')";
      sql = String.Format(sql, documentKey);

      var row = DataReader.GetDataRow(DataOperation.Parse(sql));
      if (row != null) {
        return (int) row["DocumentId"];
      } else {
        return -1;
      }
    }

    static internal int TryGetRecordingBookId(string recorderOfficeName, string sectionNo, string bookNo) {
      string sql = "SELECT RecordingBookId FROM OldRecordingModelData " +
                   "WHERE RecorderOffice = '{0}' AND SectionNo = '{1}' AND " +
                   "RecordingBookNumber = '{2}'";
      sql = String.Format(sql, recorderOfficeName, sectionNo, bookNo);

      var row = DataReader.GetDataRow(DataOperation.Parse(sql));
      if (row != null) {
        return (int) row["RecordingBookId"];
      } else {
        return -1;
      }
    }

    static internal Tuple<int, string> TryGetOldRecording(int recordingBookId, int documentId) {
      string sql = "SELECT OldRecordingId, RecordingNumber FROM OldRecordingModelData " +
                   "WHERE RecordingBookId = {0} AND DocumentId = {1}";
      sql = String.Format(sql, recordingBookId, documentId);

      var row = DataReader.GetDataRow(DataOperation.Parse(sql));
      if (row != null) {
        return new Tuple<int, string>((int) row["OldRecordingId"], (string) row["RecordingNumber"]);
      } else {
        return null;
      }
    }

    static internal int WriteImagingItem(ImagingItem o) {
      var operation = DataOperation.Parse("writeLRSImagingItem", o.Id, o.GetEmpiriaType().Id, o.RecordingBook.Id,
                                          o.Document.Id, o.ManualRecording.Id, o.BaseFolder.Id, o.RelativePath,
                                          o.ImagingItemExtData.ToString(), o.FilesCount, o.FilesTotalSize,
                                          o.ProtectionSeal, o.DigitalizedBy.Id, String.Empty);
      return DataWriter.Execute(operation);
    }

    #endregion Public methods

  } // class DataServices

} // namespace Empiria.Land.Documentation
