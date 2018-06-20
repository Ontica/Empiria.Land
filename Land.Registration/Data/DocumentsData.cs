﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  System   : Empiria Land                                 Module  : Recording Services                      *
*  Assembly : Empiria.Land.Registration.dll                Pattern : Data Services                           *
*  Type     : DocumentsData                                License : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides database read and write methods for recording documents.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Data;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording documents.</summary>
  static internal class DocumentsData {

    #region Public methods

    static internal string GenerateDocumentUID() {
      while (true) {
        string newDocumentUID = UIDGenerators.CreateDocumentUID();

        var checkIfExistDocument = RecordingDocument.TryParse(newDocumentUID);

        if (checkIfExistDocument == null) {
          return newDocumentUID;
        }
      }
    }


    static internal LRSTransaction GetDocumentTransaction(RecordingDocument document) {
      if (document.IsEmptyInstance) {
        return LRSTransaction.Empty;
      }

      var sql = $"SELECT * FROM LRSTransactions WHERE DocumentId = {document.Id}";

      var dataRow = DataReader.GetDataRow(DataOperation.Parse(sql));
      if (dataRow != null) {
        return BaseObject.ParseDataRow<LRSTransaction>(dataRow);
      } else {
        return LRSTransaction.Empty;
      }
    }


    static internal string GetNextImagingControlID(RecordingDocument document) {
      string prefix = document.AuthorizationTime.ToString("yyyy-MM");

      var sql = "SELECT MAX(ImagingControlID) " +
                $"FROM LRSDocuments WHERE ImagingControlID LIKE '{prefix}-%'";

      var imagingControlID = DataReader.GetScalar<String>(DataOperation.Parse(sql), String.Empty);

      if (imagingControlID != String.Empty) {
        var counter = int.Parse(imagingControlID.Split('-')[2]);
        counter++;
        return prefix + "-" + counter.ToString("00000");
      } else {
        return prefix + "-" + 1.ToString("00000");
      }
    }


    static internal DataRow GetRecordingMainDocument(Recording recording) {
      return DataReader.GetDataRow(DataOperation.Parse("getLRSRecordingMainDocument", recording.Id));
    }


    static internal FixedList<RecordingDocument> SearchClosedDocuments(string filter, string keywords) {
      filter += " AND (DocumentStatus = 'C')";

      if (keywords.Length != 0) {
        filter += " AND ";
        filter += SearchExpression.ParseAndLike("DocumentKeywords",
                                                EmpiriaString.BuildKeywords(keywords));
      }

      return BaseObject.GetList<RecordingDocument>(filter, "AuthorizationTime")
                       .ToFixedList();
    }


    static internal void SaveImagingControlID(RecordingDocument document) {
      var op = DataOperation.Parse("setLRSDocumentImagingControlID",
                                   document.Id, document.Imaging.ImagingControlID);

      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class DocumentsData

} // namespace Empiria.Land.Data
