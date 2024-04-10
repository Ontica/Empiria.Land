/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transactions Workflow                      Component : Domain Layer                            *
*  Assembly : Empiria.Land.Transactions.dll              Pattern   : Aggregator class                        *
*  Type     : WorkflowTransition                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides specific rules for Empiria Land micro workflow engine.                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Transactions.Workflow {

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
