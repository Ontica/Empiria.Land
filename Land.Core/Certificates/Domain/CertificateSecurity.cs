/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording services                      Component : Recording documents                   *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Separated entity                      *
*  Type     : CertificateSecurity                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains security methods used to protect cretificates integrity.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Contacts;
using Empiria.Security;

using Empiria.Land.Registration;

using Empiria.Land.Certificates.Data;

namespace Empiria.Land.Certificates {

  /// <summary>Contains security methods used to protect cretificates integrity.</summary>
  public class CertificateSecurity : IProtected {

    #region Constructors and parsers

    internal CertificateSecurity(Certificate certificate) {
      this.Certificate = certificate;
    }

    #endregion Constructors and parsers

    #region Public properties

    internal Certificate Certificate {
      get;
    }

    #endregion Public properties

    #region Public methods

    internal bool CanChangeStatusTo(CertificateStatus newStatus) {
      CertificateStatus currentStatus = Certificate.Status;

      if (currentStatus == CertificateStatus.Pending &&
         newStatus == CertificateStatus.Deleted) {
        return true;
      }
      if (currentStatus == CertificateStatus.Pending &&
          newStatus == CertificateStatus.Closed) {
        return true;
      }
      if (currentStatus == CertificateStatus.Closed &&
          newStatus == CertificateStatus.Pending) {
        return true;
      }

      return false;
    }


    public bool CanBeElectronicallySigned() {
      try {
        EnsureCanBeElectronicallySigned();

        return true;
      } catch {
        return false;
      }
    }


    public bool CanRevokeSign() {
      try {
        EnsureCanRevokeSign();

        return true;
      } catch {
        return false;
      }
    }

    internal void EnsureCanChangeStatusTo(CertificateStatus newStatus) {
      if (CanChangeStatusTo(newStatus)) {
        return;
      }
      Assertion.RequireFail(
        $"The status of the certificate with ID '{Certificate.CertificateID}' " +
        $"cannot be changed to {newStatus}.");
    }


    public void EnsureCanBeElectronicallySigned() {
      Assertion.Require(LandRecordSecurityData.ESIGN_ENABLED,
          "No es posible firmar electrónicamente este certificado debido a que el servicio " +
          "de firma electrónica no está habilitado." + CertificateDescriptionMessage());

      Assertion.Require(Certificate.IsClosed,
          "No se puede firmar un certificado que no ha sido cerrado." + CertificateDescriptionMessage());

      Assertion.Require(Certificate.SecurityData.IsUnsigned,
          "No se puede firmar un certificado que ya está firmado." + CertificateDescriptionMessage());

      Assertion.Require(Certificate.SecurityData.UsesESign,
          "No se puede firmar un certificado que no está marcado para firma electrónica." + CertificateDescriptionMessage());

      Assertion.Require(Certificate.SecurityData.SignedBy.Equals(ExecutionServer.CurrentContact) ||
                        Certificate.RecorderOffice.IsAttendantSigner(ExecutionServer.CurrentContact as Person),
          "El certificado está asignado para ser firmado " +
          $"por una persona distinta ({Certificate.SecurityData.SignedBy.FullName}) " +
          $"a la que está intentando firmarlo, y dicha persona tampoco se encuentra en " +
          $"la lista de firmantes auxiliares de la oficialía." + CertificateDescriptionMessage());
    }


    public void EnsureCanRevokeSign() {
      Assertion.Require(LandRecordSecurityData.ESIGN_ENABLED,
                        "No se puede revocar la firma de este certificado debido a que el servicio " +
                        "de firma electrónica no está habilitado." + CertificateDescriptionMessage());

      Assertion.Require(Certificate.SecurityData.UsesESign,
                       "Sólo se puede revocar la firma de un certificado " +
                       "marcado para firma electrónica." + CertificateDescriptionMessage());

      Assertion.Require(Certificate.IsClosed,
                        "No se puede revocar la firma de un certificado " +
                        "que no está cerrado." + CertificateDescriptionMessage());

      Assertion.Require(Certificate.SecurityData.IsSigned,
                        "No se puede revocar la firma de un certificado " +
                        "que no ha sido firmado." + CertificateDescriptionMessage());


      Assertion.Require(Certificate.SecurityData.SignedBy.Equals(ExecutionServer.CurrentContact),
                       "Únicamente la misma persona que firmó el certificado puede " +
                       "revocar la firma electrónica." + CertificateDescriptionMessage());
    }


    public void ElectronicSign(LandESignData signData) {
      Assertion.Require(signData, nameof(signData));

      EnsureCanBeElectronicallySigned();

      Certificate.SecurityData.SetElectronicSignData(signData);

      CertificatesData.SaveSecurityData(this.Certificate);
    }


    public void PrepareForElectronicSign() {

      EnsureCanBeElectronicallySigned();

      Certificate.SecurityData.PrepareForElectronicSign(Certificate);

      CertificatesData.SaveSecurityData(this.Certificate);
    }


    public void RemoveSign() {

      EnsureCanRemoveElectronicSign();

      this.Certificate.SecurityData.RemoveSignData();

      CertificatesData.SaveSecurityData(this.Certificate);
    }


    public void RevokeSign() {

      EnsureCanRevokeSign();

      this.Certificate.SecurityData.RevokeSignData();

      CertificatesData.SaveSecurityData(this.Certificate);
    }


    public void SetElectronicSignerData(Person person) {

      EnsureCanSetElectronicSigner();

      this.Certificate.SecurityData.SetElectronicSignerData(person);

      CertificatesData.SaveSecurityData(this.Certificate);
    }

    #endregion Public methods

    #region Integrity methods

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }


    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      var cer = this.Certificate;
      if (version == 1) {
        return new object[] {
          1, "Id", cer.Id,
          "Type", cer.CertificateType.Id,
          "CertificateID", cer.CertificateID,
          "Transaction", cer.Transaction.Id,
          "RecorderOffice", cer.RecorderOffice.Id,
          "IssuedBy", cer.IssuedBy.Id,
          "IssueTime", cer.IssueTime,
          "OnLandRecord", cer.OnLandRecord.Id,
          "OnPersonName", cer.OnPersonName,
          "OnRealEstateDescription", cer.OnRealEstateDescription,
          "OnRecordableSubject", cer.OnRecordableSubject.Id,
          "CertificateText", cer.AsText,
          "SignedBy", cer.SecurityData.SignedBy.Id,
          "SignedTime", cer.SecurityData.SignedTime,
          "SignStatus", (char) cer.SecurityData.SignStatus,
          "SignType", (char) cer.SecurityData.SignType,
          "SecurityData", cer.SecurityData.ExtData.ToString(),
          "PostedBy", cer.PostedBy.Id,
          "PostingTime", cer.PostingTime,
          "Status", (char) cer.Status,
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }


    private IntegrityValidator _validator = null;
    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }

    #endregion Integrity methods

    #region Helpers

    private void EnsureCanRemoveElectronicSign() {
      Assertion.Require(Certificate.SecurityData.IsUnsigned,
          "Este certificado ya fue firmado electrónicamente. " +
          "Para poder abrirlo se necesita solicitar que se revoque la firma electrónica.");
    }


    private void EnsureCanSetElectronicSigner() {
      Assertion.Require(LandRecordSecurityData.ESIGN_ENABLED,
                "No es posible preparar este certificado para firma electrónica, " +
                "debido a que el servicio de firma electrónica no está habilitado." + CertificateDescriptionMessage());

      Assertion.Require(Certificate.IsClosed,
                "No se puede enviar a firma un certificado que no está cerrado." + CertificateDescriptionMessage());

      Assertion.Require(Certificate.SecurityData.IsUnsigned,
                "No se puede enviar a firma un certificado que ya está firmado." + CertificateDescriptionMessage());

    }


    private string CertificateDescriptionMessage() {
      return $" Trámite {Certificate.Transaction.UID}. Certificado: {Certificate.CertificateID}.";
    }


    #endregion Helpers

  } // class CertificateSecurity

} // namespace Empiria.Land.Certificates
