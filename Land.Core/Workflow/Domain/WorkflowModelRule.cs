/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : WorkflowModelRule                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about an specific rule for Empiria Land micro workflow engine.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Json;

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>Holds information about an specific rule for Empiria Land micro workflow engine.</summary>
  internal class WorkflowModelRule {

    static internal WorkflowModelRule Parse(JsonObject json) {
      return new WorkflowModelRule {
        From = json.Get<TransactionStatus>("from"),
        To = json.GetFixedList<TransactionStatus>("to"),
        If = json.Get("if", string.Empty),
        IfNot = json.Get("ifNot", string.Empty)
      };
    }

    public TransactionStatus From {
      get; private set;
    }

    public FixedList<TransactionStatus> To {
      get; private set;
    }

    public string If {
      get; private set;
    }

    public string IfNot {
      get; private set;
    }

  }  // class WorkflowModelRule

}  // namespace Empiria.Land.Transactions.Workflow
