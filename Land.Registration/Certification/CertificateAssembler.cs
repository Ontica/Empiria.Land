/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : CertificateAssembler (partial)                 Pattern  : Nested Assembler Type               *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Private class that assembles a Certificate instance from a CertificateDTO.                    *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

using Empiria.Security;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Certification {

  public partial class Certificate : BaseObject, IProtected {

    #region Certificate public methods related with CertificateDTO

    /// <summary>Creates a certificate with the data contained in a CertificateDTO.</summary>
    static public Certificate Create(CertificateDTO data) {
      Assertion.AssertObject(data, "data");

      var assembler = new CertificateAssembler(data);

      var newCertificate = assembler.CreateCertificate();
      newCertificate.Save();

      return newCertificate;
    }

    /// <summary>Updates a certificate with the data contained in a CertificateDTO.</summary>
    public void Update(CertificateDTO data) {
      Assertion.AssertObject(data, "data");

      Assertion.Assert(this.Status == CertificateStatus.Pending,
                       "This certificate can't be updated. It's not in pending status.");

      var assembler = new CertificateAssembler(data);

      assembler.UpdateCertificate(this);
      this.Save();
    }

    #endregion Certificate public methods related with CertificateDTO

    #region Inner Class: CertificateAssembler

    /// <summary>Private class that assembles a Certificate instance from a CertificateDTO.</summary>
    private class CertificateAssembler {

      private CertificateDTO data = new CertificateDTO();

      internal CertificateAssembler(CertificateDTO data) {
        this.data = data;
      }

      internal Certificate CreateCertificate() {
        var certificate = new Certificate(this.GetCertificateType());
        this.LoadData(certificate);

        return certificate;
      }

      internal void UpdateCertificate(Certificate certificate) {
        AssertImmutableData(certificate);
        LoadData(certificate);
      }

      #region Private methods

      private void AssertImmutableData(Certificate certificate) {
        Validate.IsTrue(certificate.Transaction.Equals(this.GetTransaction()),
                        "Certificate Transaction was changed between calls. {0}  {1}",
                        certificate.Transaction.Id, this.GetTransaction().Id);
      }

      private CertificateType GetCertificateType() {
        Validate.NotNull(data.CertificateTypeUID, "Certificate type can't be null.");

        return CertificateType.Parse(data.CertificateTypeUID);
      }

      private RealEstate GetProperty() {
        if (data.PropertyUID.Length == 0) {
          return RealEstate.Empty;
        }
        var property = RealEstate.TryParseWithUID(data.PropertyUID);
        Validate.NotNull(property,
                        "Property with unique ID '{0}' was not found.", data.PropertyUID);
        return property;
      }

      private RecorderOffice GetRecorderOffice() {
        return RecorderOffice.Parse(data.RecorderOfficeId);
      }

      private LRSTransaction GetTransaction() {
        var transaction = LRSTransaction.TryParse(data.TransactionUID);
        Validate.NotNull(transaction,
                         "Transaction with unique ID '{0}' was not found.", data.TransactionUID);

        return transaction;
      }

      private void LoadData(Certificate certificate) {
        certificate.Transaction = this.GetTransaction();
        certificate.RecorderOffice = this.GetRecorderOffice();
        certificate.Property = this.GetProperty();
        certificate.OwnerName = data.ToOwnerName;
        certificate.UserNotes = data.UserNotes;

        certificate.ExtensionData = new CertificateExtData(this.data);
      }

      #endregion Private methods

    }  // inner class CertificateAssembler

    #endregion Inner Class: CertificateAssembler

  } // partial class Certificate

} // namespace Empiria.Land.Certification
