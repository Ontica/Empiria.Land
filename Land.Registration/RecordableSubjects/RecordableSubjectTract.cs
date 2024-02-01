/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Recording services                      Component : Domain Layer                          *
*  Assembly : Empiria.Land.Registration.dll                Pattern   : Service provider                      *
*  Type     : RecordableSubjectTract                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Gets information about a recordable subject's historic recording acts tract.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Certification;
using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Gets information about a recordable subject's historic recording acts tract.</summary>
  public class RecordableSubjectTract {

    #region Fields

    private readonly Resource _recordableSubject;

    #endregion Fields

    #region Constructors and parsers

    private RecordableSubjectTract(Resource recordableSubject) {
      _recordableSubject = recordableSubject;
    }


    static internal RecordableSubjectTract Parse(Resource recordableSubject) {
      Assertion.Require(recordableSubject, nameof(recordableSubject));

      return new RecordableSubjectTract(recordableSubject);
    }

    #endregion Constructors and parsers

    #region Properties

    public RecordingAct LastRecordingAct {
      get {
        FixedList<RecordingAct> recordingActs = this.GetRecordingActs();
        if (recordingActs.Count != 0) {
          return recordingActs[recordingActs.Count - 1];
        } else {
          return RecordingAct.Empty;
        }
      }
    }

    #endregion Properties

    #region Methods

    public FixedList<RecordingAct> GetClosedRecordingActsUntil(RecordingDocument breakingLandRecord,
                                                               bool includeBreakingLandRecord) {
      var tract = ResourceTractData.GetResourceRecordingActList(_recordableSubject);

      if (includeBreakingLandRecord) {
        return tract.FindAll((x) => ((x.LandRecord.IsClosed &&
                                      x.LandRecord.PresentationTime < breakingLandRecord.PresentationTime)) ||
                                      x.LandRecord.Equals(breakingLandRecord));
      } else {
        return tract.FindAll((x) => ((x.LandRecord.IsClosed &&
                                      x.LandRecord.PresentationTime < breakingLandRecord.PresentationTime)));
      }
    }


    public FixedList<FormerCertificate> GetEmittedCerificates() {
      return FormerCertificatesData.ResourceEmittedCertificates(_recordableSubject);
    }


    public FixedList<RecordingAct> GetFullRecordingActs() {
      return ResourceTractData.GetResourceFullTractIndex(_recordableSubject);
    }


    public FixedList<IResourceTractItem> GetFullRecordingActsWithCertificates() {
      var list = new List<IResourceTractItem>();

      var recordingActs = this.GetFullRecordingActs();
      var certificates = this.GetEmittedCerificates();

      list.AddRange(recordingActs);
      list.AddRange(certificates);

      list.Sort((x, y) => x.TractPrelationStamp.CompareTo(y.TractPrelationStamp));

      return list.ToFixedList();
    }


    public BookEntry GetLastDomainBookEntry() {
      var lastRecodingAct = this.GetRecordingActs()
                                .FindLast(x => !x.BookEntry.IsEmptyInstance &&
                                               (x.RecordingActType.IsDomainActType || x.RecordingActType.IsStructureActType));

      if (lastRecodingAct == null) {
        return BookEntry.Empty;
      }

      return lastRecodingAct.BookEntry;
    }


    public RecordingAct GetRecordingAct(string recordingActUID) {
      FixedList<RecordingAct> recordingActs = this.GetRecordingActs();

      RecordingAct recordingAct = recordingActs.Find(x => x.UID == recordingActUID);

      Assertion.Require(recordingAct,
                        $"The recording act with UID '{recordingActUID}' does not " +
                        $"belong to resource '{_recordableSubject.UID}' tract index");

      return recordingAct;
    }


    public FixedList<RecordingAct> GetRecordingActs() {
      return ResourceTractData.GetResourceRecordingActList(_recordableSubject);
    }


    public FixedList<RecordingAct> GetRecordingActsUntil(RecordingAct breakAct,
                                                         bool includeBreakAct) {
      return ResourceTractData.GetResourceRecordingActListUntil(_recordableSubject,
                                                                breakAct, includeBreakAct);
    }


    public RecordingAct GetRecordingAntecedent() {
      return this.GetRecordingAntecedent(RecordingAct.Empty, false);
    }


    public RecordingAct GetRecordingAntecedent(DateTime presentationTime) {
      FixedList<RecordingAct> tract = this.GetRecordingActs();

      if (tract.Count == 0) {
        return RecordingAct.Empty;
      }

      /// For no real estate, return always the first act that is the creational act.
      /// Resources different than real estates don't have domain or structure acts.
      if (!(_recordableSubject is RealEstate)) {
        return tract[0];
      }

      /// Returns the last domain or structure act if founded, otherwise
      /// return the very first act of the real estate.
      var antecedent = tract.FindLast((x) => x.LandRecord.PresentationTime <= presentationTime &&
                                      (x.RecordingActType.IsDomainActType || x.RecordingActType.IsStructureActType));

      if (antecedent != null) {
        return antecedent;
      } else {
        return tract[0];
      }
    }


    public RecordingAct GetRecordingAntecedent(RecordingAct beforeRecordingAct,
                                               bool returnAmendmentActs = false) {
      /// For amendment acts, this method returns the amendmentOf act
      /// when returnAmendmentActs flag is true
      if (returnAmendmentActs && beforeRecordingAct.RecordingActType.IsAmendmentActType) {
        return beforeRecordingAct.AmendmentOf;
      }

      var tract = this.GetRecordingActsUntil(beforeRecordingAct, false);

      /// Returns the empty recording act if there are not antecedents.
      if (tract.Count == 0) {

        if (!(_recordableSubject is RealEstate)) {
          return RecordingAct.Empty;
        }

        /// If no antecedent, then look if the real estate is a new partition.
        /// If it is then return the antecedent of the partitioned or parent real estate.
        var parentRealEstate = ((RealEstate) _recordableSubject).IsPartitionOf;

        if (!parentRealEstate.IsEmptyInstance) {
          return parentRealEstate.Tract.GetRecordingAntecedent(beforeRecordingAct.LandRecord.PresentationTime);
        } else {
          return RecordingAct.Empty;
        }
      }

      if (_recordableSubject is RealEstate) {
        /// Returns the last NO-CANCELED domain or structure act if founded, otherwise
        /// return the very first act of the real estate.

        return tract.FindLast((x) => (x.RecordingActType.IsDomainActType ||
                                     x.RecordingActType.IsStructureActType) && !x.IsCanceled) ?? tract[0];
      } else {
        /// For no real estate, return always the first act that is the creational act.
        /// Resources different than real estates don't have domain or structure acts.
        return tract[0];
      }
    }


    internal RecordingAct TryGetLastActiveChainedAct(RecordingActType chainedRecordingActType,
                                                     RecordingDocument beforeThisLandRecord) {

      var tract = this.GetClosedRecordingActsUntil(beforeThisLandRecord, true);

      // Lookup for the last chainedRecordingActType occurrence, possibly including acts in beforeThisLandRecord
      RecordingAct lastChainedAct =
            tract.FindLast( (x) => (x.RecordingActType.Equals(chainedRecordingActType) &&
                                    x.Validator.WasAliveOn(beforeThisLandRecord.PresentationTime)));

      // If there aren't any chainedRecordingActType acts in the tract, then return null.
      if (lastChainedAct == null) {
        return null;
      }

      // Assert that the founded chained act was not used by any act recorded
      // after lastChainedAct AND before beforeThisDocument was recorded

      int startIndex = tract.IndexOf(lastChainedAct);

      // Get the index of the last closed recording act previous to beforeThisLandRecord
      RecordingAct lastActToSearch =
              tract.FindLast( (x) => x.LandRecord.PresentationTime < beforeThisLandRecord.PresentationTime);

      int endIndex = lastActToSearch != null ? tract.IndexOf(lastActToSearch) : tract.Count - 1;

      for (int i = startIndex + 1; i <= endIndex; i++) {

        var rule = tract[startIndex].RecordingActType.RecordingRule;

        // If there are a recording act with the same chaining rule, then lastChainedAct was already used.
        if (rule.ChainedRecordingActType.Equals(chainedRecordingActType)) {
          return null;
        }
      }

      return lastChainedAct;
    }

    #endregion Methods

  }  // class RecordableSubjectTract

}  // namespace Empiria.Land.Registration
