/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : WorkflowTransition                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a workflow transition from a given task to the next one.                             *
*             It's an edge of a directed graph.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>Describes a workflow transition from a given task to the next one.
  /// It's an edge of a directed graph.</summary>
  internal class WorkflowTransition {

    public string Condition {
      get;
      private set;
    }


    public string Next {
      get;
      private set;
    }

  }  // class WorkflowTransition

}  // namespace Empiria.Land.Transactions.Workflow
