/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : DataServices                                   Pattern  : Data Services Static Class          *
*  Version   : 2.0        Date: 25/Jun/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Imaging service for Empiria Land System.                                                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Data;
using Empiria.Json;
using Empiria.Land.Registration;

namespace Empiria.Land.Documentation {

  /// <summary>Imaging service for Empiria Land System.</summary>
  static internal class DataServices {

    #region Public methods

    static internal bool DocumentWasDigitalized(RecordingDocument document, DocumentImageType imageType) {
      string sql = "SELECT * FROM vwLRSImagingItems " +
                   "WHERE DocumentUID = '{0}' AND ImageType = '{1}'";

      sql = String.Format(sql, document.UID, (char) imageType);

      return (DataReader.Count(DataOperation.Parse(sql)) > 0);
    }

    static internal bool DocumentWasDigitalized(RecordingDocument document, RecordingBook recordingBook,
                                                string recordingNo, DocumentImageType imageType) {
      string sql = "SELECT * FROM vwLRSImagingItems " +
                   "WHERE DocumentUID = '{0}' AND PhysicalBookId = {1} " +
                   "AND RecordingNo = '{2}' AND ImageType = '{3}'";

      sql = String.Format(sql, document.UID, recordingBook.Id, recordingNo, (char) imageType);

      return (DataReader.Count(DataOperation.Parse(sql)) > 0);
    }

    static internal RecordingBook TryGetRecordingBook(string recorderOfficeName,
                                                      string sectionNo, string bookNo) {
                    string sql = "SELECT PhysicalBookId FROM OldRecordingModelData " +
                   "WHERE RecorderOffice = '{0}' AND SectionNo = '{1}' AND " +
                   "PhysicalBookNo = '{2}'";
      sql = String.Format(sql, recorderOfficeName, sectionNo, bookNo);

      var row = DataReader.GetDataRow(DataOperation.Parse(sql));
      if (row != null) {
        return RecordingBook.Parse((int) row["PhysicalBookId"]);
      } else {
        return RecordingBook.Empty;
      }
    }

    static internal string TryGetOldRecordingNo(RecordingBook recordingBook, RecordingDocument document) {
      string sql = "SELECT OldRecordingId, PhysicalRecordingNo FROM OldRecordingModelData " +
                   "WHERE PhysicalBookId = {0} AND DocumentId = {1}";
      sql = String.Format(sql, recordingBook.Id, document.Id);

      var row = DataReader.GetDataRow(DataOperation.Parse(sql));
      if (row != null) {
        return (string) row["PhysicalRecordingNo"];
      } else {
        return null;
      }
    }

    static internal int WriteImagingItem(DocumentImage o) {
      var operation = DataOperation.Parse("writeLRSImagingItem", o.Id, o.GetEmpiriaType().Id, o.Document.Id,
                                          RecordingBook.Empty.Id, String.Empty, Recording.Empty.Id,
                                          (char) o.DocumentImageType, o.BaseFolder.Id, o.ItemPath,
                                          o.ImagingItemExtData.ToString(), o.FilesCount, o.FilesTotalSize,
                                          String.Empty);
      return DataWriter.Execute(operation);
    }

    static internal int WriteImagingItem(RecordingImage o) {
      var operation = DataOperation.Parse("writeLRSImagingItem", o.Id, o.GetEmpiriaType().Id, o.Document.Id,
                                          o.PhysicalBook.Id, o.RecordingNo, o.PhysicalRecording.Id,
                                          (char) o.DocumentImageType, o.BaseFolder.Id, o.ItemPath,
                                          o.ImagingItemExtData.ToString(), o.FilesCount, o.FilesTotalSize,
                                          String.Empty);
      return DataWriter.Execute(operation);
    }

    #endregion Public methods

  } // class DataServices

} // namespace Empiria.Land.Documentation
