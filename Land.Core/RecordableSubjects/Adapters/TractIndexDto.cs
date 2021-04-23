/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Interface adapters                      *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Data Transfer Object                    *
*  Type     : TractIndexDto                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO that returns the tract index of a real estate or other kind of recordable subjects. *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Output DTO that returns the tract index of a real estate
  /// or other kind of recordable subjects.</summary>
  public class TractIndexDto {

    public RecordableSubjectDto RecordableSubject {
      get; internal set;
    }


    public FixedList<TractIndexEntryDto> TractIndex {
      get; internal set;
    } = new FixedList<TractIndexEntryDto>();


  }  // class TractIndexDto


  /// <summary>Holds an entry of the recordable subject tract.</summary>
  public class TractIndexEntryDto {

    internal TractIndexEntryDto() {
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

  }  // class TractIndexEntryDto

}  // namespace Empiria.Land.RecordableSubjects.Adapters
