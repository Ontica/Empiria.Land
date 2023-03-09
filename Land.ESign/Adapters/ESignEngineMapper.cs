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


    static internal FixedList<SignDocumentDto> Mapper(FixedList<SignedDocumentEntry> signedDocumentsDto) {

      FixedList<SignDocumentDto> mapEntries = MapToSignDocument(signedDocumentsDto);

      return new FixedList<SignDocumentDto>(mapEntries);
    }


    #endregion Public methods

    #region Private methods

    
    static private SignDocumentDto MapDocument(SignedDocumentEntry x) {
      var dto = new SignDocumentDto();

      dto.TransactionUID = x.TransactionUID;
      dto.DocumentType = x.DocumentType;
      dto.TransactionType = x.TransactionType;
      dto.InternalControlNo = x.InternalControlNo;
      dto.AssignedBy = x.AssignedBy;
      dto.RequestedBy = x.RequestedBy;
      dto.TransactionStatus = x.TransactionStatus;
      dto.PresentationTime = x.PresentationTime;

      return dto;
    }


    static private FixedList<SignDocumentDto> MapToSignDocument(
                    FixedList<SignedDocumentEntry> signedDocumentsDto) {

      var requests = signedDocumentsDto.Select((x) => MapDocument(x));

      return new FixedList<SignDocumentDto>(requests);
    }


    #endregion Private methods

  } // class ESignEngineMapper

} // namespace Empiria.Land.ESign.Adapters
