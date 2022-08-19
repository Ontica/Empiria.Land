/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Certificates Issuing                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Certificates.dll              Pattern   : Mapper                                  *
*  Type     : CertificateMapper                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Maps land certificate instances to thier DTOs.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DataTypes;

using Empiria.Land.Registration;

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Certificates {

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
        RecordableSubject = RecordableSubjectsMapper.Map(certificate.OnRecordableSubject),
        IssuingRecordingContext = MapRecordableSubjectRecordingContext(certificate.OnRecordableSubject),
        MediaLink = GetCertificateMediaLink(certificate),
        Status = certificate.Status.Name()
      };
    }

    #region Helpers

    static private MediaData GetCertificateMediaLink(Certificate certificate) {
      return new MediaData("text/html", "http://10.113.5.57/pages/recording-stamps/recording.stamp.aspx?uid=RP-ZS-38UB-92AP54-RH74XA");
    }

    static private RecordingContextDto MapRecordableSubjectRecordingContext(Resource recordableSubject) {
      var recordingAct = recordableSubject.Tract.LastRecordingAct;

      return new RecordingContextDto(recordingAct.Document.UID, recordingAct.UID);
    }

    #endregion Helpers

  }  // class CertificateMapper

}  // namespace Empiria.Land.Certificates
