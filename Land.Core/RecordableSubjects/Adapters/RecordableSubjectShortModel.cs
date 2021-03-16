/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordableSubjectShortModel                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with a recordable subject short model.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Output DTO with a recordable subject short model.</summary>
  public class RecordableSubjectShortModel {

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string ElectronicID {
      get; internal set;
    }

    public string Kind {
      get; internal set;
    }

  }  // class RecordableSubjectShortModel

}  // namespace Empiria.Land.RecordableSubjects.Adapters
