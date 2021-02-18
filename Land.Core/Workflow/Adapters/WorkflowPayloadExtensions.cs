/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Type Extension methods                  *
*  Type     : WorkflowPayloadExtensions                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Isolates WorkflowPayload adapter methods.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;

namespace Empiria.Land.Workflow.Adapters {

  /// <summary>Isolates WorkflowPayload adapter methods.</summary>
  static internal class WorkflowPayloadExtensions {

    static internal Contact AssignTo(this WorkflowPayload payload) {
      if (!string.IsNullOrWhiteSpace(payload.AssignToUID)) {
        return Contact.Parse(payload.AssignToUID);
      }
      return Contact.Empty;
    }

  }  // class WorkflowPayloadExtensions

} // namespace Empiria.Land.Workflow.Adapters
