/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                            Component : Interface adapters                      *
*  Assembly : Empiria.Land.SearchServices.dll            Pattern   : Mapper class                            *
*  Type     : RecordMapper                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map land electronic records.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.SearchServices {

  /// <summary>Methods used to map land electronic records.</summary>
  static internal class RecordMapper {

    static internal RecordDto Map(RecordingDocument document) {
      PhysicalRecording bookEntry = document.TryGetHistoricRecording();

      return new RecordDto {
        UID = document.GUID,
        ElectronicID = document.UID,
        RecorderOffice = document.RecorderOffice.Alias,
        InstrumentType = document.DocumentType.DisplayName,
        PresentationTime = document.PresentationTime,
        RecordingTime = document.AuthorizationTime,
        RecordedBy = document.PostedBy.Alias,
        SignedBy = document.AuthorizedBy.Alias,
        BookEntry = bookEntry != null ? bookEntry.AsText : String.Empty,
        TransactionID = document.TransactionID,
      };
    }

  }  // class RecordMapper

}  // namespace Empiria.Land.SearchServices
