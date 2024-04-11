/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Certificates                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper                                  *
*  Type     : CertificateMapper                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Maps land certificate instances to thier DTOs.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.Registration;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Certificates.Adapters {

  static internal class CertificateMapper {

    static internal FixedList<CertificateDto> Map(FixedList<Certificate> list) {
      return list.Select(x => Map(x))
                 .ToFixedList();
    }


    static internal CertificateDto Map(Certificate certificate) {
      return new CertificateDto {
        UID = certificate.UID,
        CertificateID = certificate.CertificateID,
        Type = certificate.CertificateType.DisplayName,
        IssuedBy = certificate.IssuedBy.ShortName,
        AsText = certificate.AsText,
        IsClosed = certificate.IsClosed,
        IssueTime = certificate.IssueTime,
        TractPrelationStamp = certificate.TractPrelationStamp,
        OverRecordableSubject = !certificate.OnRecordableSubject.IsEmptyInstance,
        RecordableSubject = RecordableSubjectsMapper.Map(certificate.OnRecordableSubject),
        TransactionUID = certificate.Transaction.UID,
        ExternalTransactionNo = certificate.Transaction.ExternalTransactionNo,
        MediaLink = GetCertificateMediaLink(certificate),
        Status = certificate.Status.Name(),
        IssuingRecordingContext = MapRecordableSubjectRecordingContext(certificate.OnRecordableSubject),
        Actions = MapActions(certificate)
      };
    }

    #region Helpers

    static private MediaData GetCertificateMediaLink(Certificate certificate) {
      return new MediaData("text/html", "http://10.113.5.57/pages/recording-stamps/recording.stamp.aspx?uid=RP-ZS-38UB-92AP54-RH74XA");
    }


    static private CertificateActions MapActions(Certificate certificate) {
      return new CertificateActions {
        CanClose = certificate.CanChangeStatusTo(CertificateStatus.Closed),
        CanDelete = certificate.CanChangeStatusTo(CertificateStatus.Deleted),
        CanOpen = certificate.CanChangeStatusTo(CertificateStatus.Pending)
      };
    }

    static private RecordingContextDto MapRecordableSubjectRecordingContext(Resource recordableSubject) {
      var recordingAct = recordableSubject.Tract.LastRecordingAct;

      return new RecordingContextDto(recordingAct.LandRecord.GUID, recordingAct.UID);
    }

    #endregion Helpers

  }  // class CertificateMapper

}  // namespace Empiria.Land.Certificates.Adapters
