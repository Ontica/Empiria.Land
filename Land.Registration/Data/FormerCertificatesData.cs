/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  System   : Empiria Land                                 Module  : Certificate Issuing Services            *
*  Assembly : Empiria.Land.Registration.dll                Pattern : Data Services                           *
*  Type     : CertificatesData                             License : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : FormerCertificatesData Provides database read and write methods for land certificates.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;
using Empiria.Contacts;

using Empiria.Land.Registration;
using Empiria.Land.Certification;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Data {

  /// <summary>Provides database read and write methods for land certificates.</summary>
  static internal class FormerCertificatesData {

    #region Public methods

    static internal FixedList<FormerCertificate> GetTransactionIssuedCertificates(LRSTransaction transaction) {
      if (transaction.IsEmptyInstance) {
        return new FixedList<FormerCertificate>();
      }

      var op = DataOperation.Parse("qryLRSCertificatesByTransaction", transaction.Id);

      return DataReader.GetFixedList<FormerCertificate>(op, true);
    }


    static internal bool IsSigned(FormerCertificate certificate) {
      var sql = $"SELECT * FROM vwLRSDocumentSign WHERE DocumentNo = '{certificate.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      var dataRow = DataReader.GetDataRow(DataOperation.Parse(sql));

      return dataRow != null;
    }


    static internal string GetDigitalSignature(FormerCertificate certificate) {
      var sql = $"SELECT DigitalSign FROM vwLRSDocumentSign WHERE DocumentNo = '{certificate.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      var sign = DataReader.GetScalar<string>(DataOperation.Parse(sql),
                                              "NO TIENE FIRMA ELECTRÓNICA.");
      return sign;
    }


    static internal Person GetDigitalSignatureSignedBy(FormerCertificate certificate) {
      var sql = $"SELECT RequestedToId FROM vwLRSDocumentSign WHERE DocumentNo = '{certificate.UID}' " +
                $"AND SignStatus = 'S' AND DigitalSign <> ''";

      var signedById = DataReader.GetScalar<int>(DataOperation.Parse(sql), -1);

      return Person.Parse(signedById);
    }


    static internal FixedList<FormerCertificate> ResourceEmittedCertificates(Resource resource) {
      if (resource.IsEmptyInstance) {
        return new FixedList<FormerCertificate>();
      }

      var op = DataOperation.Parse("qryLRSResourceEmittedCertificates", resource.Id);

      return DataReader.GetFixedList<FormerCertificate>(op, true);
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


    #endregion Public methods

  } // class FormerCertificatesData

} // namespace Empiria.Land.Data
