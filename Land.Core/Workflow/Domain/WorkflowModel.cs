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

  internal class WorkflowModel : GeneralObject {

    static public WorkflowModel Parse(int id) {
      return BaseObject.ParseId<WorkflowModel>(id);
    }


    public bool AllowAutoTake {
      get {
        return base.ExtendedDataField.Get("allowAutoTake", false);
      }
    }


    public FixedList<WorkflowModelRule> Rules {
      get {
        return base.ExtendedDataField.GetFixedList<WorkflowModelRule>("rules");
      }
    }

  }  // class WorkflowModel

}  // namespace Empiria.Land.Transactions.Workflow
