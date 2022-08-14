/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Data Access Layer                       *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Data Services                           *
*  Type     : CertificatesData                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data read and write services for land certificates.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Certificates.Data {

  static internal class CertificatesData {

    static internal FixedList<Certificate> GetTransactionCertificates(LRSTransaction transaction) {
      var sql = "SELECT * FROM LRSCertificates " +
               $"WHERE TransactionId = {transaction.Id}";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Certificate>(operation);
    }


    static internal Certificate TryGetCertificateWithID(string certificateID) {
      var sql = "SELECT * FROM LRSCertificates " +
               $"WHERE CertificateNumber = '{certificateID}'";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetObject<Certificate>(operation);
    }


    /// <summary>Data read and write services for land certificates.</summary>
    static internal void WriteCertificate(Certificate o) {
      var op = DataOperation.Parse("writeLRSCertificate",
                  o.Id, o.UID, o.CertificateType.Id, o.CertificateID,
                  o.Transaction.Id, o.RecorderOffice.Id,
                  o.OnRecordableSubject.Id, o.OnRecording.Id, o.OnOwnerName,
                  o.Notes, o.ExtensionData.ToString(), o.AsText, o.Keywords,
                  o.IssueTime, o.IssuedBy.Id, (char) o.IssueMode,
                  o.PostedBy.Id, o.PostingTime, (char) o.Status,
                  o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


  }  // class CertificatesData

}  // namespace Empiria.Land.Certificates.Data
