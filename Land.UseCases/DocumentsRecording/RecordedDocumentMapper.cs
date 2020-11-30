﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Documents Recording                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Mapper class                            *
*  Type     : RecordedDocumentMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods to map from RecordedDocument to RecorderDocumentDTO objects.                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Land.Registration;

namespace Empiria.Land.Recording.UseCases {

  /// <summary>Contains methods to map from RecordedDocument to RecorderDocumentDTO objects.</summary>
  static internal class RecordedDocumentMapper {


    static internal FixedList<RecordedDocumentDto> Map(FixedList<RecordingDocument> source) {
      return new FixedList<RecordedDocumentDto>(source.Select((x) => Map(x)));
    }


    static internal RecordedDocumentDto Map(RecordingDocument document) {
      var dto = new RecordedDocumentDto();

      dto.UID = document.UID;
      dto.Type = document.DocumentType.Name;
      dto.Subtype = document.Subtype.Name;
      dto.Summary = document.Notes;

      return dto;
    }


  }  // class RecordedDocumentMapper

}  // namespace Empiria.Land.Recording.UseCases