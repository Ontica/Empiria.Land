/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Instruments.dll               Pattern   : Mapper class                            *
*  Type     : InstrumentMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map legal instruments to InstrumentDto objects.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Instruments.UseCases {

  /// <summary>Methods to map legal instruments to InstrumentDto objects.</summary>
  static internal class InstrumentMapper {

    static internal FixedList<InstrumentDto> Map(FixedList<Instrument> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<InstrumentDto>(mappedItems);
    }


    static internal InstrumentDto Map(Instrument instrument) {
      var issuerDto = IssuerMapper.Map(instrument.Issuer);
      var mediaDto = IssuerMapper.Map(instrument.Issuer);

      var dto = new InstrumentDto {
        UID = instrument.UID,
        Type = instrument.InstrumentType.Name,
        TypeName = instrument.InstrumentType.DisplayName,
        CategoryUID = instrument.Category.UID,
        CategoryName = instrument.Category.Name,
        IssueDate = instrument.IssueDate,
        Issuer = issuerDto,
        SheetsCount = instrument.SheetsCount,
        DocumentNo = instrument.DocumentNo,
        CaseNo = instrument.CaseNo,
        Media = mediaDto
      };

      return dto;
    }

  }  // class InstrumentMapper

}  // namespace Empiria.Land.Instruments.UseCases
