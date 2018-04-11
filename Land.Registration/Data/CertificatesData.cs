/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  System   : Empiria Land                                 Module  : Certificate Issuing Services            *
*  Assembly : Empiria.Land.Registration.dll                Pattern : Data Services                           *
*  Type     : CertificatesData                             License : Please read LICENSE.txt file            *
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
  static internal class CertificatesData {

    #region Public methods

    static internal string BuildCertificateUID() {
      while (true) {
        string newCertificateUID = UIDGenerators.CreateCertificateUID();

        var checkIfExistCertificate = Certificate.TryParse(newCertificateUID);

        if (checkIfExistCertificate == null) {
          return newCertificateUID;
        }
      }
    }

    static internal FixedList<Certificate> GetTransactionIssuedCertificates(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new FixedList<Certificate>();
      }

      var op = DataOperation.Parse("qryLRSCertificatesByTransaction", transaction.Id);

      return DataReader.GetList(op, (x) => BaseObject.ParseList<Certificate>(x)).ToFixedList();
    }

    static internal FixedList<Certificate> ResourceEmittedCertificates(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<Certificate>();
      }

      var op = DataOperation.Parse("qryLRSResourceEmittedCertificates", resource.Id);

      return DataReader.GetList(op, (x) => BaseObject.ParseList<Certificate>(x)).ToFixedList();
    }

    static internal void WriteCertificate(Certificate o) {
      var op = DataOperation.Parse("writeLRSCertificate",
                          o.Id, o.CertificateType.Id, o.UID,
                          o.Transaction.Id, o.RecorderOffice.Id, o.Property.Id, o.OwnerName,
                          o.UserNotes, o.ExtensionData.ToJson(), o.AsText, o.Keywords,
                          o.IssueTime, o.IssuedBy.Id, o.SignedBy.Id, (char) o.IssueMode,
                          o.PostedBy.Id, o.PostingTime, (char) o.Status, o.Integrity.GetUpdatedHashCode());

      DataWriter.Execute(op);
    }

    #endregion Public methods

  } // class CertificatesData

} // namespace Empiria.Land.Data
