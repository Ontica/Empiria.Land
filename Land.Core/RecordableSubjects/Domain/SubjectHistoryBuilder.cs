/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Structurer                              *
*  Type     : SubjectHistoryBuilder                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds the SubjectHistory for a given subject.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects {

  /// <summary>Builds the SubjectHistory for a given subject.</summary>
  internal class SubjectHistoryBuilder {

    private readonly Resource _subject;
    private readonly FixedList<RecordingAct> _recordingActs;

    public SubjectHistoryBuilder(Resource subject, FixedList<RecordingAct> recordingActs) {
      Assertion.Require(subject, nameof(subject));
      Assertion.Require(recordingActs, nameof(recordingActs));

      _subject = subject;
      _recordingActs = recordingActs;
    }


    public SubjectHistory Build() {
      var history = new SubjectHistory(_subject);

      foreach (var recordingAct in _recordingActs) {
        SubjectHistoryEntry entry = BuildSubjectHistoryEntry(recordingAct);

        history.AddEntry(entry);
      }

      return history;
    }


    private SubjectHistoryEntry BuildSubjectHistoryEntry(RecordingAct recordingAct) {
      string name = GetName(recordingAct);
      string description = GetDescription(recordingAct);

      return new SubjectHistoryEntry(recordingAct, name, description);
    }


    private string GetName(RecordingAct recordingAct) {
      bool isPartition = recordingAct.AppliedOverNewPartition;

      if (recordingAct.Kind.Length != 0) {
        return recordingAct.Kind + (isPartition ? " de una fracción" : String.Empty);
      }

      return recordingAct.DisplayName + (isPartition ? " de una fracción" : String.Empty);
    }


    private string GetDescription(RecordingAct recordingAct) {
      if (!(_subject is RealEstate)) {
        return string.Empty;
      }

      if (!Resource.IsCreationalRole(recordingAct.ResourceRole)) {
        return string.Empty;
      }

      var realEstate = (RealEstate) recordingAct.Resource;

      if (recordingAct.ResourceRole == ResourceRole.Created) {
        return "Predio inscrito por primera vez (no es fusión ni se subdividió de otro).";

      } else if (realEstate.Equals(_subject)) {
        return $"Fracción identificada como {PartitionText(realEstate)} del predio " +
               $"{recordingAct.RelatedResource.UID}, con una superficie de {realEstate.LotSize}.";

      } else {
        return $"Subdividido en {PartitionText(realEstate)} con folio real " +
               $"{realEstate.UID}. Superficie: {realEstate.LotSize}.";

      }
    }


    static private string PartitionText(RealEstate newPartition) {
      if (newPartition.Kind.Length == 0 && newPartition.PartitionNo.Length == 0) {
        return $"Fracción sin identificar";

      } else if (newPartition.Kind.Length != 0 && newPartition.PartitionNo.Length == 0) {
        return $"{newPartition.Kind} sin identificar";

      } else if (newPartition.Kind.Length == 0 && newPartition.PartitionNo.Length != 0) {
        return $"Fracción {newPartition.PartitionNo}";

      } else {
        return $"{newPartition.Kind} {newPartition.PartitionNo}";
      }
    }

  }  // class SubjectHistoryBuilder

}  // namespace Empiria.Land.RecordableSubjects
