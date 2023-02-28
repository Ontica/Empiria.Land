/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : ESign Services                             Component : Interface adapter                       *
*  Assembly : Empiria.Land.ESign.dll                     Pattern   : Mapper                                  *
*  Type     : ESignEngineMapper                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Maps land esign instances to thier DTOs.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Land.ESign.Domain;
using Empiria.OnePoint.ESign.Adapters;

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Maps land esign instances to thier DTOs.</summary>
  static internal class ESignEngineMapper {

    #region Public methods


    static internal ESignDTO Map(FixedList<OPSignDocumentEntryDto> signedDocumentsDto) {
      return new ESignDTO {
        SignRequests = MapEntries(signedDocumentsDto)
      };
    }


    #endregion Public methods

    #region Private methods


    static private FixedList<SignRequestDto> MapEntries(
                    FixedList<OPSignDocumentEntryDto> signedDocumentsDto) {

      var requests = signedDocumentsDto.Select((x) => MapEntry((OPSignDocumentEntryDto) x));

      return new FixedList<SignRequestDto>(requests);
    }


    static private SignRequestDto MapEntry(OPSignDocumentEntryDto x) {
      var dto = new SignRequestDto();
      
      return dto;
    }


    #endregion Private methods

  } // class ESignEngineMapper

} // namespace Empiria.Land.ESign.Adapters
