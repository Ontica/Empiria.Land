/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Workflow Management                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Aggregator class                        *
*  Type     : WorkflowCommandBuilder                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Creates a workflow command that can be invoked by the workflow engine.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Workflow {

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
