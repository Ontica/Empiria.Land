/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Documentation                           Component : Document Management                   *
*  Assembly : Empiria.Land.Documentation.dll               Pattern   : Data Services                         *
*  Type     : DataServices                                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Database read and write services specific for the Land Documentation component.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;
using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Documentation {

  /// <summary>Database read and write services specific for the Land Documentation component.</summary>
  static internal class DataServices {

    #region Read methods


    static internal bool DocumentWasDigitalized(RecordingDocument document,
                                                DocumentImageType imageType) {
      string sql = "SELECT * FROM LRSImagingItems " +
                   "WHERE DocumentId = {0} AND ImageType = '{1}'";

      sql = String.Format(sql, document.Id, (char) imageType);

      return (DataReader.Count(DataOperation.Parse(sql)) > 0);
    }


    static internal FixedList<TransactionDocument> GetTransactionDocuments(LRSTransaction transaction) {
      string sql = $"SELECT * FROM LRSImagingItems " +
                   $"WHERE TransactionId = {transaction.Id} " +
                   $"AND ImagingItemTypeId = 2004 AND ImagingItemStatus <> 'X'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<TransactionDocument>(op);
    }


    #endregion Read methods

    #region Write methods


    static internal void WriteImagingItem(DocumentImageSet o) {
      var op = DataOperation.Parse("writeLRSImagingItem", o.Id, o.GetEmpiriaType().Id,
                                   o.Document.GetTransaction().Id, o.Document.Id,
                                   RecordingBook.Empty.Id, (char) o.DocumentImageType,
                                   o.BaseFolder.Id, o.ItemPath, o.ImagingItemExtData.ToString(),
                                   o.FilesCount, o.DigitalizedBy.Id, o.DigitalizationDate,
                                   (char) o.Status, o.Integrity.GetUpdatedHashCode());
      DataWriter.Execute(op);
    }


    static internal void WriteImageProcessingLog(DocumentImageSet o, string message) {
      var op = DataOperation.Parse("apdLRSImageProcessingTrail",
                                   o.MainImageFileName, (char) o.DocumentImageType,
                                   DateTime.Now, message, o.Document.Id, o.Id,
                                   o.FullPath, 'A', String.Empty);

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


    static internal void WriteTransactionDocument(TransactionDocument o) {
      var op = DataOperation.Parse("writeLRSImagingItem", o.Id, o.GetEmpiriaType().Id,
                                   o.Transaction.Id, o.Transaction.Document.Id,
                                   RecordingBook.Empty.Id, (char) o.DocumentType,
                                   o.BaseFolder.Id, o.ItemPath, o.ImagingItemExtData.ToString(),
                                   o.FilesCount, o.DigitalizedBy.Id, o.DigitalizationDate,
                                   (char) o.Status, o.Integrity.GetUpdatedHashCode());
      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class DataServices

} // namespace Empiria.Land.Documentation
