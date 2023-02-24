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

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Maps land esign instances to thier DTOs.</summary>
  static internal class ESignEngineMapper {

    #region Public methods


    static internal ESignDTO Map(FixedList<SignRequestEntry> signRequests) {
      return new ESignDTO {
        SignRequests = MapEntries(signRequests)
      };
    }


    #endregion Public methods

    #region Private methods


    static private FixedList<SignRequestDto> MapEntries(FixedList<SignRequestEntry> signRequests) {
      var requests = signRequests.Select((x) => MapEntry((SignRequestEntry) x));

      return new FixedList<SignRequestDto>(requests);
    }


    static private SignRequestDto MapEntry(SignRequestEntry x) {
      var dto = new SignRequestDto();
      dto.UID = x.UID;
      dto.RequestedBy = x.RequestedBy;
      dto.RequestedTime = x.RequestedTime;
      dto.SignStatus = x.SignStatus;
      dto.SignatureKind = x.SignatureKind;
      dto.DigitalSignature = x.DigitalSignature;
      dto.Document = x.Document;
      dto.Filing = x.Filing;
      return dto;
    }


    #endregion Private methods

  } // class ESignEngineMapper

} // namespace Empiria.Land.ESign.Adapters
