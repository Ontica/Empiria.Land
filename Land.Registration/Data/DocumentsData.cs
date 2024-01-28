/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recording services                           Component : Data services                         *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Data services provider                *
*  Type     : DocumentsData                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides database read and write methods for recording documents.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording documents.</summary>
  static internal class DocumentsData {

    #region Methods

    static internal LRSTransaction GetDocumentTransaction(RecordingDocument document) {
      if (document.IsEmptyInstance) {
        return LRSTransaction.Empty;
      }

      var sql = $"SELECT * FROM LRSTransactions " +
                $"WHERE DocumentId = {document.Id}";

      var op = DataOperation.Parse(sql);

      return DataReader.GetObject(op, LRSTransaction.Empty);
    }


    static internal string GetNextImagingControlID(RecordingDocument document) {
      string prefix = document.AuthorizationTime.ToString("yyyy-MM");

      var sql = "SELECT MAX(ImagingControlID) " +
                $"FROM LRSDocuments " +
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


    static internal void SaveImagingControlID(RecordingDocument document) {
      var op = DataOperation.Parse("setLRSDocumentImagingControlID",
                                   document.Id, document.Imaging.ImagingControlID);

      DataWriter.Execute(op);
    }


    static internal RecordingDocument TryGetBookEntryMainDocument(BookEntry bookEntry) {
      var op = DataOperation.Parse("getLRSRecordingMainDocument", bookEntry.Id);

      return DataReader.GetObject<RecordingDocument>(op, null);
    }

    #endregion Methods

  } // class DocumentsData

} // namespace Empiria.Land.Data
