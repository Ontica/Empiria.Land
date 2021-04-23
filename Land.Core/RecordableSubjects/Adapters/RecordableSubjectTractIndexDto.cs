/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : RecordableSubjectTractIndexDto             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that returns the whole or partial tract index for a recordable subject.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.RecordableSubjects.Adapters {

  public class RecordableSubjectTractIndexDto {

    public RecordableSubjectDto RecordableSubject {
      get; internal set;
    }


    public FixedList<RecordableSubjectTractIndexEntryDto> TractIndex {
      get; internal set;
    } = new FixedList<RecordableSubjectTractIndexEntryDto>();


  }  // class RecordableSubjectTractIndexDto


  /// <summary>Holds an entry of the recordable subject tract.</summary>
  public class RecordableSubjectTractIndexEntryDto {

    internal RecordableSubjectTractIndexEntryDto() {
      // no-op
    }


    public string RecordingActUID {
      get; internal set;
    }


    public string RecordingActName {
      get; internal set;
    }


    public string Antecedent {
      get; internal set;
    }


    public DateTime PresentationTime {
      get; internal set;
    }


    public DateTime RecordingDate {
      get; internal set;
    }

  }  // class RecordableSubjectTractIndexEntryDto

}  // namespace Empiria.Land.RecordableSubjects.Adapters
