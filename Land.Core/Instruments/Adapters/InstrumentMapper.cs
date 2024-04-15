/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : InstrumentMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map legal instruments to InstrumentDto objects.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Media;
using Empiria.Land.Media.Adapters;

using Empiria.Land.Transactions;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Methods to map legal instruments to InstrumentDto objects.</summary>
  static public class InstrumentMapper {

    static internal InstrumentDto Map(Instrument instrument) {
      var issuerDto = IssuerMapper.Map(instrument.Issuer);

      FixedList<LandMediaPosting> mediaPostings = LandMediaReadServices.InstrumentFiles(instrument);

      FixedList<LandMediaFileDto> mediaFilesDtos = LandMediaFileMapper.Map(mediaPostings);

      var dto = new InstrumentDto {
        UID = instrument.UID,
        Type = instrument.InstrumentType.ToInstrumentTypeEnum(),
        TypeName = instrument.InstrumentType.DisplayName,
        Kind = instrument.Kind,
        ControlID = instrument.ControlID,
        Issuer = issuerDto,
        IssueDate = instrument.IssueDate,
        Summary = instrument.Summary,
        AsText = instrument.AsText,
        InstrumentNo = instrument.InstrumentNo,
        BinderNo = instrument.BinderNo,
        Folio = instrument.Folio,
        EndFolio = instrument.EndFolio,
        SheetsCount = instrument.SheetsCount,
        Media = mediaFilesDtos
      };

      return dto;
    }


    static public InstrumentDto Map(Instrument instrument, LRSTransaction transaction) {
      if (!instrument.IsNew && !instrument.IsEmptyInstance) {
        return Map(instrument);
      }

      FixedList<LandMediaPosting> mediaPostings = LandMediaReadServices.TransactionFiles(transaction);

      FixedList<LandMediaFileDto> mediaFilesDtos = LandMediaFileMapper.Map(mediaPostings);

      InstrumentDto dto = Map(instrument);

      dto.Media = mediaFilesDtos;

      return dto;
    }

  }  // class InstrumentMapper

}  // namespace Empiria.Land.Instruments.Adapters
