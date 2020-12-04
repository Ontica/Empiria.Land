/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recording services                           Component : Land Recording Documents              *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Data Services                         *
*  Type     : DocumentsData                                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides database read and write methods for recording documents.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Data;

using Empiria.Data;
using Empiria.Contacts;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for recording documents.</summary>
  static internal class DocumentsData {

    #region Public methods

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

    static internal bool IsSigned(RecordingDocument document) {
      var sql = $"SELECT * FROM vwLRSDocumentSign WHERE DocumentNo = '{document.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      var dataRow = DataReader.GetDataRow(DataOperation.Parse(sql));

      return dataRow != null;
    }


    static internal string GetDigitalSignature(RecordingDocument document) {
      var sql = $"SELECT DigitalSign FROM vwLRSDocumentSign WHERE DocumentNo = '{document.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      return DataReader.GetScalar<string>(DataOperation.Parse(sql),
                                          "NO TIENE FIRMA ELECTRÓNICA.");
    }


    static internal Person GetDigitalSignatureSignedBy(RecordingDocument document) {
      var sql = $"SELECT RequestedToId FROM vwLRSDocumentSign WHERE DocumentNo = '{document.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      var signedById = DataReader.GetScalar<int>(DataOperation.Parse(sql), -1);

      return Person.Parse(signedById);
    }


    static internal DateTime GetLastSignTimeForAllTransactionDocuments(LRSTransaction transaction) {
      var sql = $"SELECT MAX(SignTime) FROM vwLRSDocumentSign WHERE TransactionNo = '{transaction.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      return DataReader.GetScalar<DateTime>(DataOperation.Parse(sql));
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


    static internal DataRow GetRecordingMainDocument(PhysicalRecording recording) {
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
