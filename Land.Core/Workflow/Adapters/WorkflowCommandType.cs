/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration                             *
*  Type     : WorkflowCommandType                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Strings enumeration that defines a transaction workflow command type.                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Workflow {

  /// <summary>Strings enumeration that defines a transaction workflow command type.</summary>
  public enum WorkflowCommandType {

    Undefined,

    AssignTo,

    PullToControlDesk,

    Receive,

    Reentry,

    ReturnToMe,

    SetNextStatus,

    Sign,

    Unsign

  }  // enum WorkflowCommandType

}  // namespace Empiria.Land.Workflow
