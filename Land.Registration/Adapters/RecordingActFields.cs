/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holders                      *
*  Type     : RecordingActFields                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for a recording act with it associated subject and its participants.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.Registration.Adapters {

  /// <summary>Input data structure used to update recording acts.</summary>
  public class RecordingActFields {

    public string TypeUID {
      get; set;
    } = string.Empty;


    public string Kind {
      get; set;
    } = string.Empty;


    public string Description {
      get; set;
    } = string.Empty;


    public decimal OperationAmount {
      get; set;
    } = -1m;


    public string CurrencyUID {
      get; set;
    } = string.Empty;


    public RecordableObjectStatus Status {
      get; set;
    } = RecordableObjectStatus.Incomplete;


  }  // class RecordingActFields

}  // namespace Empiria.Land.Registration.Adapters
