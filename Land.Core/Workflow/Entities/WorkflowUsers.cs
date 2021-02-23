/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Aggregator class                        *
*  Type     : WorkflowCommandBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Creates a workflow command that can be invoked by the workflow engine.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Security;

using Empiria.Land.Registration.Transactions;

namespace Empiria.Land.Workflow {

  static internal class WorkflowUsers {


    static internal bool CanReceiveFor(Contact user, LRSTransactionStatus nextStatus) {
      return nextStatus != LRSTransactionStatus.EndPoint;
    }


    static internal bool IsInRole(Contact user, WorkflowRole role) {
      return EmpiriaUser.IsInRole(user, $"Land.{role}");
    }


  }  // class WorkflowUsers

}  // namespace Empiria.Land.Workflow
