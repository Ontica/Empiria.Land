/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Management                     Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Command payload                         *
*  Type     : WorkflowCommand                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Defines a transaction workflow command.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using Empiria.Contacts;

namespace Empiria.Land.Transactions.Adapters {

  public class WorkflowCommand {

    public WorkflowCommandType Type {
      get; set;
    } = WorkflowCommandType.Undefined;


    public WorkflowPayload Payload {
      get; set;
    } = new WorkflowPayload();

  }  // class WorkflowCommand



  public enum WorkflowCommandType {

    Undefined,

    SetNextStatus,

    Take,

    ReturnToMe,

    PullToControlDesk,

    Reentry

  }  // enum WorkflowCommandType


  public class WorkflowPayload {

    public string[] TransactionUID {
      get; set;
    } = new string[0];


    public TransactionStatus NextStatus {
      get; set;
    } = TransactionStatus.Undefined;


    public string Note {
      get; set;
    } = string.Empty;


    public string AssignToUID {
      get; set;
    } = string.Empty;


    internal Contact AssignTo {
      get {
        if (!string.IsNullOrWhiteSpace(this.AssignToUID)) {
          return Contact.Parse(this.AssignToUID);
        }
        return Contact.Empty;
      }
    }

  }  // class WorkflowPayload

}  // namespace Empiria.Land.Transactions.Adapters
