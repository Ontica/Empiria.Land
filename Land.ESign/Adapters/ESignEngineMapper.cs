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
using Empiria.OnePoint.ESign;

namespace Empiria.Land.ESign.Adapters {

  /// <summary>Maps land esign instances to thier DTOs.</summary>
  static internal class ESignEngineMapper {

    #region Public methods


    static internal FixedList<SignDocumentDto> Map(FixedList<SignedDocumentEntry> signedDocumentsDto) {

      FixedList<SignDocumentDto> mapEntries = MapToSignDocument(signedDocumentsDto);

      return new FixedList<SignDocumentDto>(mapEntries);
    }


    static internal FixedList<SignDocumentRequestDto> Map(FixedList<SignRequestDTO> signRequest) {

      FixedList<SignDocumentRequestDto> mapDocuments = MapToESignDto(signRequest);

      return new FixedList<SignDocumentRequestDto>(mapDocuments);

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


    private static FixedList<SignDocumentRequestDto> MapToESignDto(FixedList<SignRequestDTO> signRequest) {
      
      var requests = signRequest.Select((x) => MapToSignDocumentDto(x));

      return new FixedList<SignDocumentRequestDto>(requests);
    }


    static private SignDocumentRequestDto MapToSignDocumentDto(SignRequestDTO x) {
      var dto = new SignDocumentRequestDto();

      dto.UID = x.uid;
      dto.RequestedBy = x.requestedBy;
      dto.RequestedTime = x.requestedTime;
      dto.SignStatus = x.signStatus;
      dto.SignatureKind = x.signatureKind;
      dto.DigitalSignature = x.digitalSignature;
      dto.Document = MapToDocument(x.document);
      dto.Filing = MapToDocumentFiling(x.filing);

        return dto;
    }


    static private DocumentType MapToDocument(SignableDocumentDTO document) {
      var doc = new DocumentType();

      doc.UID = document.uid;
      doc.Type = document.type;
      doc.DocumentNumber = document.documentNo;
      doc.Description = document.description;
      doc.Uri = document.uri;

      return doc;
    }


    static private DocumentFiling MapToDocumentFiling(SignRequestFilingDTO filing) {
      var doc = new DocumentFiling();

      doc.FilingNo = filing.filingNo;
      doc.FilingTime = filing.filingTime;
      doc.FiledBy = filing.filedBy;
      doc.PostedBy = filing.postedBy;

      return doc;
    }


    #endregion Private methods

  } // class ESignEngineMapper

} // namespace Empiria.Land.ESign.Adapters
