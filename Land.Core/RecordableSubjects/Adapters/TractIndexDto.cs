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

using Newtonsoft.Json;

using Empiria.DataTypes;

namespace Empiria.Land.RecordableSubjects.Adapters {

  /// <summary>Output DTO that returns the tract index of a real estate
  /// or other kind of recordable subjects.</summary>
  public class TractIndexDto {

    public RecordableSubjectDto RecordableSubject {
      get; internal set;
    }


    public FixedList<TractIndexEntryDto> Entries {
      get; internal set;
    } = new FixedList<TractIndexEntryDto>();


    public FixedList<TractIndexEntryDto> Structure {
      get; internal set;
    } = new FixedList<TractIndexEntryDto>();


    public TractIndexActions Actions {
      get; internal set;
    } = new TractIndexActions();

  }


  public class TractIndexActions {

    internal TractIndexActions() {
      // no-op
    }

    public bool CanBeClosed {
      get; internal set;
    }


    public bool CanBeOpened {
      get; internal set;
    }


    public bool CanBeUpdated {
      get; internal set;
    }

  }


  /// <summary>Holds an entry of the recordable subject tract.</summary>
  public class TractIndexEntryDto {

    internal TractIndexEntryDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string EntryType {
      get; internal set;
    }


    public string Description {
      get; internal set;
    }


    public DateTime RequestedTime {
      get; internal set;
    }


    public DateTime IssueTime {
      get; internal set;
    }


    public string Status {
      get; internal set;
    }


    public TransactionInfoDto Transaction {
      get; internal set;
    }


    public OfficialDocumentDto OfficialDocument {
      get; internal set;
    }


    public RecordableSubjectChangesDto SubjectChanges {
      get; internal set;
    }


    public TrantIndexEntryActions Actions {
      get; internal set;
    } = new TrantIndexEntryActions();


  }  // class TractIndexEntryDto


  public class TrantIndexEntryActions {

    internal TrantIndexEntryActions() {
      // no-op
    }

    public bool CanBeClosed {
      get; internal set;
    }

    public bool CanBeDeleted {
      get; internal set;
    }

    public bool CanBeOpened {
      get; internal set;
    }

    public bool CanBeUpdated {
      get; internal set;
    }

  }


  public class TransactionInfoDto {

    internal TransactionInfoDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string TransactionID {
      get; internal set;
    }


    public string RequestedBy {
      get; internal set;
    }


    public NamedEntityDto Agency {
      get; internal set;
    }


    public NamedEntityDto FilingOffice {
      get; internal set;
    }


    public DateTime PresentationTime {
      get; internal set;
    }


    public DateTime CompletedTime {
      get; internal set;
    }


    public string Status {
      get; internal set;
    }

  }


  public class OfficialDocumentDto {

    internal OfficialDocumentDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Type {
      get; internal set;
    }


    public string DocumentID {
      get; internal set;
    }


    public string Description {
      get; internal set;
    }


    public NamedEntityDto Office {
      get; internal set;
    }


    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public BookEntryIdentifiersDto BookEntry {
      get;
      internal set;
    }


    public DateTime IssueTime {
      get; internal set;
    }


    public string ElaboratedBy {
      get; internal set;
    }


    public string AuthorizedBy {
      get; internal set;
    }


    public string Status {
      get; internal set;
    }


    public MediaData Media {
      get; internal set;
    }

  }



  public class BookEntryIdentifiersDto {

    internal BookEntryIdentifiersDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string RecordingBookUID {
      get; internal set;
    }


    public string InstrumentRecordingUID {
      get; internal set;
    }

  }  // BookEntryIdentifiersDto


  public class RecordableSubjectChangesDto {

    internal RecordableSubjectChangesDto() {
      // no-op
    }

    public string Summary {
      get; internal set;
    }


    public RecordableSubjectDto Snapshot {
      get; internal set;
    }


    public FixedList<StructureChangeDto> StructureChanges {
      get; internal set;
    } = new FixedList<StructureChangeDto>();


  }



  public class StructureChangeDto {

    internal StructureChangeDto() {
      // no-op
    }

    public string Operation {
      get; internal set;
    }


    public string OperationType {
      get; internal set;
    }


    public RecordableSubjectDto RelatedSubject {
      get; internal set;
    }

  }


}  // namespace Empiria.Land.RecordableSubjects.Adapters
