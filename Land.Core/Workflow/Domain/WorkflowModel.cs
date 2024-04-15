/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : WorkflowModel                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides specific rules for Empiria Land micro workflow engine.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.Workflow {

  internal class WorkflowModel {


    public static WorkflowModel Empty => new WorkflowModel();

    internal FixedList<WorkflowTransition> TransitionsFrom(string node) {
      throw new NotImplementedException();
    }

  }  // class WorkflowModel

}  // namespace Empiria.Land.Transactions.Workflow
