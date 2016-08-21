/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : DataServices                                   Pattern  : Data Services Static Class          *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Database read and write services specific for the Land Documentation component.               *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Data;
using Empiria.Land.Registration;

namespace Empiria.Land.Documentation {

  /// <summary>Database read and write services specific for the Land Documentation component.</summary>
  static internal class DataServices {

    #region Public methods

    static internal bool DocumentWasDigitalized(RecordingDocument document, DocumentImageType imageType) {
      string sql = "SELECT * FROM LRSImagingItems " +
                   "WHERE DocumentId = {0} AND ImageType = '{1}'";

      sql = String.Format(sql, document.Id, (char) imageType);

      return (DataReader.Count(DataOperation.Parse(sql)) > 0);
    }

    static internal int WriteImagingItem(DocumentImage o) {
      var operation = DataOperation.Parse("writeLRSImagingItem", o.Id, o.GetEmpiriaType().Id, o.Document.Id,
                                          RecordingBook.Empty.Id, (char) o.DocumentImageType,
                                          o.BaseFolder.Id, o.ItemPath, o.ImagingItemExtData.ToString(),
                                          o.FilesCount, o.Integrity.GetUpdatedHashCode());
      return DataWriter.Execute(operation);
    }

    static internal void WriteImageProcessingLog(DocumentImage o, string message) {
      var op = DataOperation.Parse("apdLRSImageProcessingTrail",
                                   o.MainImageFileName, (char) o.DocumentImageType,
                                   DateTime.Now, message, o.Document.Id, o.Id,
                                   o.MainImageFilePath, 'A', String.Empty);

      DataWriter.Execute(op);
    }

    static internal void WriteImageProcessingLogException(CandidateImage o, string message,
                                                          Exception exception) {
      var op = DataOperation.Parse("apdLRSImageProcessingTrail",
                                   o.FileName, (char) o.DocumentImageType,
                                   DateTime.Now, message, o.Document.Id, -1,
                                   o.SourceFile.DirectoryName, 'E', exception.ToString());

      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class DataServices

} // namespace Empiria.Land.Documentation
