/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : TransactionShortModelMapper                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains methods to map from LRSTransaction objects to TransactionShortModel DTOs.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Transactions.Adapters {

  /// <summary>Contains methods to map from LRSTransaction objects to TransactionShortModel DTOs.</summary>
  static internal class TransactionShortModelMapper {

    static internal FixedList<TransactionShortModel> Map(FixedList<LRSTransaction> list) {
      var mappedItems = list.Select((x) => Map(x));

      return new FixedList<TransactionShortModel>(mappedItems);
    }

    static internal TransactionShortModel Map(LRSTransaction transaction) {
      var dto = new TransactionShortModel();

      var currentTask = transaction.Workflow.GetCurrentTask();

      dto.UID = transaction.UID;
      dto.TransactionID = transaction.UID;
      dto.Type = transaction.TransactionType.Name;
      dto.Subtype = transaction.DocumentType.Name;
      dto.RequestedBy = transaction.RequestedBy;
      dto.PresentationTime = transaction.PresentationTime;
      dto.Stage = "InProgress";
      dto.Status = currentTask.CurrentStatus.ToString();
      dto.StatusName = currentTask.CurrentStatusName;
      dto.AssignedToUID = currentTask.Responsible.UID;
      dto.AssignedToName = currentTask.Responsible.Alias;

      return dto;
    }

  }  // class TransactionShortModelMapper

}  // namespace Empiria.Land.Transactions.Adapters
