/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects Management             Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Input Data Holder                       *
*  Type     : RecordingAntecedentFields                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data to target a recording antecedent.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Holds data to target a recording antecedent.</summary>
  public class RecordingAntecedentFields {

    public string RecordingBookUID {
      get; set;
    }

    public string RecordingNo {
      get; set;
    }

    public string Notes {
      get; set;
    }

  }  // class RecordingAntecedentFields

}  // namespace Empiria.Land.RecordableSubjects.Adapters
