/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : WorkflowCommandType                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumeration that defines a transaction workflow command type.                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Workflow.Adapters {

  /// <summary>Enumeration that defines a transaction workflow command type.</summary>
  public enum WorkflowCommandType {

    Undefined,

    AssignTo,

    Finish,

    PullToControlDesk,

    Take,

    Reentry,

    ReturnToMe,

    SetNextStatus,

    Sign,

    Unarchive,

    Unsign,

  }  // enum WorkflowCommandType

}  // namespace Empiria.Land.Transactions.Workflow.Adapters
