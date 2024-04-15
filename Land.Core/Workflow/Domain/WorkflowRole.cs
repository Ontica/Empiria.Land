/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration type                        *
*  Type     : WorkflowRole                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : User roles used for Empiria Land micro workflow execution.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Workflow {

  /// <summary>User roles used for Empiria Land micro workflow execution.</summary>
  internal enum WorkflowRole {

    ControlClerk,

    DeliveryClerk,

    Signer,

    Supervisor

  }  // enum WorkflowRole

}  // namespace Empiria.Land.Transactions.Workflow
