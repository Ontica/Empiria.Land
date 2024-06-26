﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                        Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Structurer                              *
*  Type     : SubjectHistoryBuilder                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Builds the SubjectHistory for a given subject.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Linq;

using Empiria.Land.Registration;

namespace Empiria.Land.RecordableSubjects {

  /// <summary>Builds the SubjectHistory for a given subject.</summary>
  internal class SubjectHistoryBuilder {

    private readonly Resource _subject;
    private readonly FixedList<RecordingAct> _recordingActs;

    public SubjectHistoryBuilder(Resource subject, RecordingAct recordingAct) {
      Assertion.Require(subject, nameof(subject));
      Assertion.Require(recordingAct, nameof(recordingAct));

      _subject = subject;
      _recordingActs = new[] { recordingAct }.ToFixedList();
    }

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
      bool isPartition = recordingAct.IsAppliedOverNewPartition;

      if (recordingAct.Kind.Length != 0) {
        return recordingAct.Kind + (isPartition ? " de una fracción" : String.Empty);
      }

      return recordingAct.DisplayName + (isPartition ? " de una fracción" : String.Empty);
    }


    private string GetDescription(RecordingAct recordingAct) {
      if (!(_subject is RealEstate)) {
        return string.Empty;
      }

      string temp = BuildRecordingActText(recordingAct);


      if (!recordingAct.ResourceRole.IsCreationalRole()) {
        return temp;
      }

      var realEstate = (RealEstate) recordingAct.Resource;

      if (recordingAct.ResourceRole == ResourceRole.Created) {
        return temp + ". Acto inicial en la historia.";

      } else if (realEstate.Equals(_subject)) {
        return temp + $". Fracción identificada como {PartitionText(realEstate)} del predio " +
               $"{recordingAct.RelatedResource.UID}, con una superficie de {realEstate.LotSize}.";

      } else {
        return temp + $". Subdividido en {PartitionText(realEstate)} con folio real " +
               $"{realEstate.UID}. Superficie: {realEstate.LotSize}.";

      }
    }


    private string BuildRecordingActText(RecordingAct recordingAct) {
      string temp = "{PARTIES} {AMOUNT}";

      temp = temp.Replace("{PARTIES}", BuildRecordingActParties(recordingAct));

      if (recordingAct.OperationAmount != 0) {
        temp = temp.Replace("{AMOUNT}", $"Monto {recordingAct.OperationCurrency.Format(recordingAct.OperationAmount)}");

      } else if (recordingAct.RecordingActType.RecordingRule.EditOperationAmount &&
                 recordingAct.OperationAmount == 0) {
        temp = temp.Replace("{AMOUNT}", "Sin monto de operación.");

      } else {
        temp = temp.Replace("{AMOUNT}", string.Empty);
      }

      return temp;
    }


    private string BuildRecordingActParties(RecordingAct recordingAct) {

      var primaries = recordingAct.Parties.PrimaryParties;
      var secondaries = recordingAct.Parties.SecondaryParties;

      if (primaries.Count == 0 && recordingAct.RecordingActType.RecordingRule.AllowNoParties) {
        return string.Empty;
      }

      if (primaries.Count == 0) {
        return "Sin personas registradas";
      }

      if (recordingAct.RecordingActType.IsDomainActType) {
        return EmpiriaString.ToString(primaries.Select(x => x.Party.FullName + ",")
                                               .Distinct()
                                               .ToFixedList());

      } else if (secondaries.Count != 0) {
        return secondaries[0].Party.FullName + ",";

      } else {
        return primaries[0].Party.FullName + ",";
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
