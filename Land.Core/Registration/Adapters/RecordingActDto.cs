/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Registration Services                      Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordingActDto                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with recording act data.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Registration.Adapters {

  public class RecordingActDto {

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public RecordableSubjectDto RecordableSubject {
      get; internal set;
    }

    public string Antecedent {
      get; internal set;
    }

  }  // class RecordingActDto

} // namespace Empiria.Land.Registration.Adapters
