/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Mapper class                            *
*  Type     : InstrumentMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map legal instruments to InstrumentDto objects.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Land.Registration;

namespace Empiria.Land.Instruments.UseCases {

  /// <summary>Methods to map legal instruments to InstrumentDto objects.</summary>
  static internal class InstrumentMapper {


    static internal FixedList<InstrumentDto> Map(FixedList<RecordingDocument> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<InstrumentDto>(mappedItems);
    }


    static internal InstrumentDto Map(RecordingDocument document) {
      var dto = new InstrumentDto();

      dto.UID = document.UID;
      dto.Type = document.DocumentType.Name;
      dto.Subtype = document.Subtype.Name;
      dto.Summary = document.Notes;

      return dto;
    }

  }

}
