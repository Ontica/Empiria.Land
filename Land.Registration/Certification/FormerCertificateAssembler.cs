﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Certification Services              *
*  Namespace : Empiria.Land.Certification                     Assembly : Empiria.Land.Certification.dll      *
*  Type      : FormerCertificateAssembler (partial)           Pattern  : Nested Assembler Type               *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : FormerCertificateAssembler assembles a Certificate instance from a CertificateDTO.            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Security;

using Empiria.Land.Registration;
using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Certification {

  /// <summary>FormerCertificateAssembler assembles a Certificate instance from a CertificateDTO.</summary>
  public partial class FormerCertificate : BaseObject, IProtected {

    #region Certificate public methods related with CertificateDTO

    static public FormerCertificate AutoCreate(LRSTransaction transaction,
                                         string certificateType,
                                         RealEstate property, string ownerName) {

      FormerCertificateDTO data = new FormerCertificateDTO();

      switch (certificateType) {
        case "gravamen":
          if (property.HasHardLimitationActs) {
            data.CertificateTypeUID = "ObjectType.LandCertificate.Gravamen";
            data.MarginalNotes = AutoFillMarginalNotes(property.GetHardLimitationActs());
            ownerName = property.CurrentOwners;
          } else {
            data.CertificateTypeUID = "ObjectType.LandCertificate.LibertadGravamen";
            ownerName = property.CurrentOwners;
          }
          break;
        case "inscripción":
          data.CertificateTypeUID = "ObjectType.LandCertificate.Inscripción";
          ownerName = property.CurrentOwners;
          break;
        case "no-propiedad":
          data.CertificateTypeUID = "ObjectType.LandCertificate.NoPropiedad";
          break;
      }

      data.TransactionUID = transaction.UID;

      data.RecorderOfficeId = 101;
      data.SeekForName = ownerName;
      data.FromOwnerName = ownerName;
      data.ToOwnerName = ownerName;
      data.StartingYear = 1976;
      data.PropertyUID = property.UID;
      data.PropertyMetesAndBounds = property.MetesAndBounds;
      data.PropertyLocation = $"{property.Description} en el municipio de {property.Municipality.FullName}";
      data.PropertyCommonName = GetPropertyCommonName(property);

      data.Operation = property.LastDomainAct.DisplayName.ToUpperInvariant();
      data.OperationDate = property.LastDomainAct.LandRecord.AuthorizationTime;

      var assembler = new CertificateAssembler(data);
      var newCertificate = assembler.CreateCertificate();

      newCertificate.OwnerName = ownerName;
      newCertificate.Save();
      newCertificate.Close();

      return newCertificate;
    }


    private static string GetPropertyCommonName(RealEstate realEstate) {
      if (realEstate.Kind.Length == 0) {
        return "Información disponible únicamente en documentos físicos.";
      }

      return $"predio denominado {realEstate.Description}, de tipo {realEstate.Kind}";
    }


    private static string AutoFillMarginalNotes(FixedList<RecordingAct> recordingActs) {
      const string template = "{0} registrado el día {1} bajo el número de documento electrónico {2}";

      string notes = String.Empty;

      foreach (var act in recordingActs) {
        string temp = String.Format(template, act.DisplayName,
                                    EmpiriaSpeech.SpeechDate(act.LandRecord.AuthorizationTime),
                                    act.LandRecord.UID);
        if (act.RecordingActType.RecordingRule.IsHardLimitation) {
          var p = act.Parties.SecondaryParties;
          if (p.Count > 0) {
            temp += " a favor de " + p[0].Party.FullName;
          }
        }
        notes += temp + ".\n";
      }
      return notes;
    }


    /// <summary>Creates a certificate with the data contained in a CertificateDTO.</summary>
    static public FormerCertificate Create(FormerCertificateDTO data) {
      Assertion.Require(data, "data");

      var assembler = new CertificateAssembler(data);

      var newCertificate = assembler.CreateCertificate();
      newCertificate.Save();

      return newCertificate;
    }

    /// <summary>Updates a certificate with the data contained in a CertificateDTO.</summary>
    public void Update(FormerCertificateDTO data) {
      Assertion.Require(data, "data");

      Assertion.Require(this.Status == FormerCertificateStatus.Pending,
                       "This certificate can't be updated. It's not in pending status.");

      var assembler = new CertificateAssembler(data);

      assembler.UpdateCertificate(this);
      this.Save();
    }

    #endregion Certificate public methods related with CertificateDTO

    #region Inner Class: CertificateAssembler

    /// <summary>Private class that assembles a Certificate instance from a CertificateDTO.</summary>
    private class CertificateAssembler {

      private FormerCertificateDTO data = new FormerCertificateDTO();

      internal CertificateAssembler(FormerCertificateDTO data) {
        this.data = data;
      }

      internal FormerCertificate CreateCertificate() {
        var certificate = new FormerCertificate(this.GetCertificateType());
        this.LoadData(certificate);

        return certificate;
      }

      internal void UpdateCertificate(FormerCertificate certificate) {
        AssertImmutableData(certificate);
        LoadData(certificate);
      }

      #region Private methods

      private void AssertImmutableData(FormerCertificate certificate) {
        Validate.IsTrue(certificate.Transaction.Equals(this.GetTransaction()),
                        $"Certificate Transaction was changed between calls. " +
                        $"{certificate.Transaction.Id}  {this.GetTransaction().Id}");
      }

      private FormerCertificateType GetCertificateType() {
        Validate.NotNull(data.CertificateTypeUID, "Certificate type can't be null.");

        return FormerCertificateType.Parse(data.CertificateTypeUID);
      }

      private RealEstate GetProperty() {
        if (data.PropertyUID.Length == 0) {
          return RealEstate.Empty;
        }

        var property = RealEstate.TryParseWithUID(data.PropertyUID);

        Validate.NotNull(property,
                        $"Property with unique ID '{data.PropertyUID}' was not found.");

        return property;
      }

      private RecorderOffice GetRecorderOffice() {
        return RecorderOffice.Parse(data.RecorderOfficeId);
      }

      private LRSTransaction GetTransaction() {
        var transaction = LRSTransaction.TryParse(data.TransactionUID);
        Validate.NotNull(transaction,
                         $"Transaction with unique ID '{data.TransactionUID}' was not found.");

        return transaction;
      }

      private void LoadData(FormerCertificate certificate) {
        certificate.Transaction = this.GetTransaction();
        certificate.RecorderOffice = this.GetRecorderOffice();
        certificate.Property = this.GetProperty();
        certificate.OwnerName = data.ToOwnerName;
        certificate.UserNotes = data.UserNotes;

        certificate.ExtensionData = new FormerCertificateExtData(this.data);
      }

      #endregion Private methods

    }  // inner class CertificateAssembler

    #endregion Inner Class: CertificateAssembler

  } // partial class FormerCertificate

} // namespace Empiria.Land.Certification
