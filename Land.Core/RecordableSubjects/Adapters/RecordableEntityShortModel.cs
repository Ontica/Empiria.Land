/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects Management             Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordableEntityShortModel                 License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with a recordable entity short model.                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Output DTO with a recordable resource short model.</summary>
  public class RecordableEntityShortModel {

    public string UID {
      get; internal set;
    }

    public string Type {
      get; internal set;
    }

    public string Subtype {
      get; internal set;
    }

    public string RecordableID {
      get; internal set;
    }

    public string MediaUri {
      get; internal set;
    }

  }  // class RecordableEntityShortModel

}  // namespace Empiria.Land.RecordableSubjects.Adapters
