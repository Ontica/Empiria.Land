/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : SubjectHistoryEntry                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds information about a recordable subject history entry.                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.RecordableSubjects.Adapters;
using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects {

  /// <summary>Classifies a subject history entry.</summary>
  public enum SubjectHistoryEntryType {

    RecordingAct,

    Certificate

  }  // enum SubjectHistoryEntryType



  /// <summary>Holds information about a recordable subject history entry.</summary>
  internal class SubjectHistoryEntry {


    #region Constructors and parsers

    internal SubjectHistoryEntry(RecordingAct recordingAct, string name, string description) {
      Assertion.Require(recordingAct, nameof(recordingAct));
      Assertion.Require(name, nameof(name));

      this.RecordingAct = recordingAct;
      this.Name = name;
      this.Description = description;
    }

    #endregion Constructors and parsers

    #region Properties

    public SubjectHistoryEntryType EntryType {
      get {
        return SubjectHistoryEntryType.RecordingAct;
      }
    }


    public string Name {
      get;
    }


    public string Description {
      get;
    }


    public RecordingAct RecordingAct {
      get;
    }


    public string StatusName {
      get {
        return RecordingAct.StatusName;
      }
    }


    public ResourceShapshotData SubjectSnapshot {
      get {
        return RecordingAct.GetResourceSnapshotData();
      }
    }


    public SubjectEditionRules EditionRules {
      get {
        return GetEditionRules();
      }
    }


    #endregion Properties

    #region Helpers

    private SubjectEditionRules GetEditionRules() {
      bool isHistoric = RecordingAct.Document.IsHistoricDocument;
      bool isClosed = RecordingAct.Document.IsClosed;

      return new SubjectEditionRules {
        CanBeDeleted = isHistoric && !isClosed,
        CanBeClosed = isHistoric && !isClosed,
        CanBeOpened = isHistoric && isClosed,
        CanBeUpdated = isHistoric && !isClosed && RecordingAct.IsEditable,
      };
    }

    #endregion Helpers

  }  // class SubjectHistoryEntry

}  // namespace Empiria.Land.RecordableSubjects
