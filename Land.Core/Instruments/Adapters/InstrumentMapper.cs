﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Legal Instruments                          Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : InstrumentMapper                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods to map legal instruments to InstrumentDto objects.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Instruments.Adapters {

  /// <summary>Methods to map legal instruments to InstrumentDto objects.</summary>
  static internal class InstrumentMapper {

    static internal FixedList<InstrumentDto> Map(FixedList<Instrument> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<InstrumentDto>(mappedItems);
    }


    static internal InstrumentDto Map(Instrument instrument) {
      var issuerDto = IssuerMapper.Map(instrument.Issuer);

      var mediaFiles = LandMediaFileMapper.Map(instrument.GetMediaFileSet());

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
        Media = mediaFiles,
        Status = instrument.Status.ToString()
      };

      return dto;
    }

    internal static object Map(object instrument) {
      throw new NotImplementedException();
    }
  }  // class InstrumentMapper

}  // namespace Empiria.Land.Instruments.Adapters
