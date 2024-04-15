/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : WorkflowTaskMapper                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Mapping methods from WorkflowTask instances to WorkflowTaskDto models.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.Workflow.Adapters {

  /// <summary>Mapping methods from WorkflowTask instances to WorkflowTaskDto models.</summary>
  static internal class WorkflowTaskMapper {

    static internal FixedList<WorkflowTaskDto> Map(LRSWorkflowTaskList list) {
      return new FixedList<WorkflowTaskDto>(list.Select((x) => Map(x)));
    }


    static internal WorkflowTaskDto Map(LRSWorkflowTask task) {
      return new WorkflowTaskDto {
        TaskName = task.CurrentStatusName,
        AssigneeName = task.Responsible.ShortName,
        CheckInTime = task.CheckInTime,
        EndProcessTime = task.EndProcessTime,
        CheckOutTime = task.CheckOutTime,
        ElapsedTime = task.ElapsedTime.ToString(@"dd\:hh\:mm") + ":00",
        NextTask = task.NextStatus,
        NextTaskName = task.NextStatusName,
        NextAssigneeUID = task.NextContact.UID,
        NextAssigneeName = task.NextContact.ShortName,
        Notes = task.Notes,
        StatusName = task.StatusName
      };
    }

  }  // class WorkflowTaskMapper

}  // namespace Empiria.Land.Transactions.Workflow.Adapters
