﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Data Access Layer                       *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Services                           *
*  Type     : CertificatesData                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data read and write services for land certificates.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

using Empiria.Land.Registration;
using Empiria.Land.Transactions;

namespace Empiria.Land.Certificates.Data {

  static internal class CertificatesData {

    static internal FixedList<Certificate> GetRecordableSubjectIssuedCertificates(Resource recordableSubject) {
      var sql = "SELECT * FROM LRSCertificates " +
               $"WHERE PropertyId = {recordableSubject.Id} AND CertificateStatus = 'C' " +
                "ORDER BY CertificateId";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Certificate>(op);
    }


    static internal FixedList<Certificate> GetTransactionCertificates(LRSTransaction transaction) {
      var sql = "SELECT * FROM LRSCertificates " +
               $"WHERE TransactionId = {transaction.Id} AND CertificateStatus <> 'X' " +
                "ORDER BY CertificateId";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Certificate>(op);
    }


    static internal Certificate TryGetCertificateWithID(string certificateID) {
      var sql = "SELECT * FROM LRSCertificates " +
               $"WHERE CertificateUID = '{certificateID}'";

      var op = DataOperation.Parse(sql);

      return DataReader.GetObject<Certificate>(op, null);
    }


    /// <summary>Data read and write services for land certificates.</summary>
    static internal void WriteCertificate(Certificate o) {
      var op = DataOperation.Parse("writeLRSCertificate",
                  o.Id, o.UID, o.CertificateType.Id, o.CertificateID,
                  o.Transaction.Id, o.RecorderOffice.Id,
                  o.OnRecordableSubject.Id, o.OnLandRecord.Id,
                  o.Notes, o.ExtensionData.ToString(), o.AsText,
                  (char) o.SecurityData.SignStatus, (char) o.SecurityData.SignType,
                  o.SecurityData.SignedBy.Id, o.SecurityData.SignedTime, o.SecurityData.ExtData.ToString(),
                  o.Keywords, o.IssueTime, o.IssuedBy.Id, (char) o.IssueMode,
                  o.PostedBy.Id, o.PostingTime, (char) o.Status,
                  o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

  }  // class CertificatesData

}  // namespace Empiria.Land.Certificates.Data
