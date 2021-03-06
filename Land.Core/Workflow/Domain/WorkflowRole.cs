﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Enumeration type                        *
*  Type     : WorkflowRole                               License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : User roles used for Empiria Land micro workflow execution.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Workflow {

  /// <summary>User roles used for Empiria Land micro workflow execution.</summary>
  internal enum WorkflowRole {

    Certificator,

    ControlClerk,

    DeliveryClerk,

    Digitizer,

    HistoricDigitizer,

    FilingQualifier,

    LegalAdvisor,

    ReceptionClerk,

    Registrar,

    Signer,

    Supervisor,

    User

  }  // enum WorkflowRole

}  // namespace Empiria.Land.Workflow
