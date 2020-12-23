/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : TransactionListItemDtoMapper               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods to map from LRSTransaction objects to TransactionListItem DTOs.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Contains methods to map from LRSTransaction objects to TransactionListItem DTOs.</summary>
  static internal class TransactionListItemDtoMapper {

    static internal FixedList<TransactionListItemDto> Map(FixedList<LRSTransaction> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<TransactionListItemDto>(mappedItems);
    }

    static internal TransactionListItemDto Map(LRSTransaction transaction) {
      var dto = new TransactionListItemDto();

      dto.UID = transaction.UID;
      dto.TransactionID = transaction.UID;
      dto.Type = transaction.TransactionType.Name;
      dto.Subtype = transaction.DocumentType.Name;
      dto.RequestedBy = transaction.RequestedBy;
      dto.PresentationTime = transaction.PresentationTime;
      dto.Stage = "InProgress";
      dto.Status = transaction.Workflow.CurrentStatus.ToString();
      dto.StatusName = transaction.Workflow.CurrentStatusName;

      return dto;
    }

  }  // class TransactionListItemDtoMapper

}  // namespace Empiria.Land.Transactions.Adapters
