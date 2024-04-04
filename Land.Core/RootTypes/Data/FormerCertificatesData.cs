/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificate emission services                Component : Data services                         *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Data services provider                *
*  Type     : FormerCertificatesData                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Provides database read and write methods for land certificates.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.Land.Registration;
using Empiria.Land.Certification;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for land certificates.</summary>
  static internal class FormerCertificatesData {

    #region Methods

    static internal FixedList<FormerCertificate> GetTransactionIssuedCertificates(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new FixedList<FormerCertificate>();
      }

      var op = DataOperation.Parse("qryLRSCertificatesByTransaction", transaction.Id);

      return DataReader.GetFixedList<FormerCertificate>(op);
    }


    static internal FixedList<FormerCertificate> ResourceEmittedCertificates(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<FormerCertificate>();
      }

      var op = DataOperation.Parse("qryLRSResourceEmittedCertificates", resource.Id);

      return DataReader.GetFixedList<FormerCertificate>(op);
    }


    static internal void WriteCertificate(FormerCertificate o) {
      var op = DataOperation.Parse("writeLRSCertificate",
                          o.Id, o.CertificateType.Id, o.GUID, o.UID,
                          o.Transaction.Id, o.RecorderOffice.Id, o.Property.Id, o.OwnerName,
                          o.UserNotes, o.ExtensionData.ToJson(), o.AsText, o.Keywords,
                          o.IssueTime, o.IssuedBy.Id, (char) o.IssueMode,
                          o.PostedBy.Id, o.PostingTime, (char) o.Status,
                          o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }


    #endregion Methods

  } // class FormerCertificatesData

} // namespace Empiria.Land.Data
