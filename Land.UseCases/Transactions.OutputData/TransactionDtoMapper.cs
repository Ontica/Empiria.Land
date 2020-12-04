/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.UseCases.dll                  Pattern   : Mapper class                            *
*  Type     : TransactionListItemMapper                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods to map from LRSTransaction objects to TransactionListItem DTOs.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.UseCases {

  /// <summary>Contains methods to map from RecordedDocument to RecorderDocumentDTO objects.</summary>
  static internal class TransactionDtoMapper {

    static internal FixedList<TransactionDto> Map(FixedList<LRSTransaction> list) {
      return new FixedList<TransactionDto>(list.Select((x) => Map(x)));
    }


    static internal TransactionDto Map(LRSTransaction transaction) {
      var dto = new TransactionDto();

      dto.UID = transaction.UID;
      dto.TransactionID = transaction.UID;
      dto.Type = transaction.TransactionType.Name;
      dto.Subtype = transaction.DocumentType.Name;
      dto.RequestedBy = transaction.RequestedBy;
      dto.PresentationTime = transaction.PresentationTime;
      dto.Status = transaction.Workflow.CurrentStatus.ToString();
      dto.StatusName = transaction.Workflow.CurrentStatusName;

      return dto;
    }

  }  // class TransactionListItemMapper

}  // namespace Empiria.Land.Transactions.UseCases
