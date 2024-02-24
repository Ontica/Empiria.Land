/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Data Layer                              *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Data Service                            *
*  Type     : ESignEngineData                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data read methods for ESign.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;
using Empiria.Land.ESign.Domain;

namespace Empiria.Land.ESign.Data {

  /// <summary>Provides data read methods for ESign.</summary>
  static internal class ESignEngineData {


    internal static FixedList<SignedDocumentEntry> GetSignedDocuments(int recorderOfficeId, string responsibleUID) {
      Assertion.Require(recorderOfficeId, nameof(recorderOfficeId));

      var sql = "SELECT * FROM ( " +
                " SELECT tra.TransactionId, tra.TransactionUID, docType.ObjectName AS DocumentType, " +
                " transType.ObjectName AS TransactionType, tra.InternalControlNo, " +
                " assigned.ContactFullName AS AssignedBy, responsible.ContactFullName as Responsible, " +
                " tra.RequestedBy, tra.TransactionStatus, " +
                " MAX(tra.PresentationTime) AS PresentationTime " +

                " FROM LRSTransactions tra " +
                " INNER JOIN LRSLandRecords doc on tra.DocumentId = doc.LandRecordId " +
                " INNER JOIN LRSInstruments ins on doc.InstrumentId = ins.InstrumentId " +
                " INNER JOIN SimpleObjects docType on tra.DocumentTypeId = docType.ObjectId " +
                " INNER JOIN SimpleObjects transType on tra.TransactionTypeId = transType.ObjectId " +
                " INNER JOIN LRSTransactionTrack tt on tra.TransactionId = tt.TransactionId " +
                " INNER JOIN Contacts assigned on tt.AssignedById = assigned.ContactId " +
                " INNER JOIN Contacts responsible on tt.ResponsibleId = responsible.ContactId " +

                $" WHERE tra.TransactionStatus = 'S' AND tra.RecorderOfficeId = {recorderOfficeId} " +
                $" AND responsible.ContactUID = '{responsibleUID}'" +
                "  and tt.TrackId = " +
                " (select max (TrackId) from LRSTransactionTrack where TransactionId = tra.TransactionId) " +

                " GROUP BY tra.TransactionId, tra.TransactionUID, docType.ObjectName, " +
                "   transType.ObjectName, tra.InternalControlNo, assigned.ContactFullName, " +
                "   responsible.ContactFullName, tra.RequestedBy, tra.TransactionStatus " +

                ") AS GETTRANSACTIONS ORDER BY PresentationTime DESC";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<SignedDocumentEntry>(dataOperation);
    }


  } // class ESignDataService

} // namespace Empiria.Land.ESign.Data
