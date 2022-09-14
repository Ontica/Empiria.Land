/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Mapper class                            *
*  Type     : SubjectHistory                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains the recording history entries in wich a given subject was involved.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects
{

  /// <summary>Contains the recording entries in which a given subject was involved.</summary>
  internal class SubjectHistory {

    private readonly List<SubjectHistoryEntry> _entries = new List<SubjectHistoryEntry>(32);

    internal SubjectHistory(Resource subject) {
      Assertion.Require(subject, nameof(subject));

      Subject = subject;
    }


    public Resource Subject {
      get;
    }


    public FixedList<SubjectHistoryEntry> Entries {
      get {
        return _entries.ToFixedList();
      }
    }


    public EditionRules EditionRules {
      get {
        return new EditionRules {
          CanBeClosed = true,
          CanBeUpdated = true
        };
      }
    }


    #region Methods

    internal void AddEntry(SubjectHistoryEntry historyEntry) {
      Assertion.Require(historyEntry, nameof(historyEntry));

      _entries.Add(historyEntry);
    }

    #endregion Methods

  }  // class SubjectHistory

}  // namespace Empiria.Land.RecordableSubjects
