/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Transaction Workflow                       Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : ApplicableCommandDto                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that with applicable commands for a given set of transactions.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Transactions.Workflow.Adapters {

  /// <summary>Output DTO that with applicable command for a given set of transactions.</summary>
  public class ApplicableCommandDto {

    public WorkflowCommandType Type {
      get; internal set;
    } = WorkflowCommandType.Undefined;


    public string Name {
      get; internal set;
    } = string.Empty;


    public NextStateDto[] NextStatus {
      get; internal set;
    } = new NextStateDto[0];


    public NamedEntityDto[] NextUsers {
      get; internal set;
    } = new NamedEntityDto[0];


  }  // class ApplicableCommandDto



  /// <summary>Next state data transfer object.</summary>
  public class NextStateDto {

    public string Type {
      get; internal set;
    } = string.Empty;


    public string Name {
      get; internal set;
    } = string.Empty;


    public NamedEntityDto[] Users {
      get; internal set;
    } = new NamedEntityDto[0];

  }  // class NextStateDto


}  //namespace Empiria.Land.Transactions.Workflow.Adapters
