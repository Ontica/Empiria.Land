/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : SubjectHistoryDto                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with the history of a real property and other kinds of recordable subjects.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

using Empiria.Land.Registration.Adapters;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Output DTO with the history of a real property and other
  /// kinds of recordable subjects.</summary>
  public class SubjectHistoryDto {

    public RecordableSubjectDto RecordableSubject {
      get; internal set;
    }


    public FixedList<SubjectHistoryEntryDto> Entries {
      get; internal set;
    } = new FixedList<SubjectHistoryEntryDto>();


    public FixedList<SubjectHistoryEntryDto> Structure {
      get; internal set;
    } = new FixedList<SubjectHistoryEntryDto>();


    public EditionRules Actions {
      get; internal set;
    } = new EditionRules();

  }  // class SubjectHistoryDto



  /// <summary>Holds a recordable subject history entry.</summary>
  public class SubjectHistoryEntryDto {

    internal SubjectHistoryEntryDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public SubjectHistoryEntryType EntryType {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string Description {
      get; internal set;
    }


    public string Status {
      get; internal set;
    }



    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public SubjectHistoryEntryDto AmendedAct {
      get; internal set;
    }


    public RecordingDataDto RecordingData {
      get; internal set;
    }


    public RecordableSubjectDto SubjectSnapshot {
      get; internal set;
    }


    public EditionRules Actions {
      get; internal set;
    } = new EditionRules();


  }  // class SubjectHistoryEntryDto

}  // namespace Empiria.Land.RecordableSubjects.Adapters
