/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : ResourceTract                                  Pattern  : Information holder                  *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Gets information about a resource historic recording acts tract.                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Land.Certification;
using Empiria.Land.Data;

namespace Empiria.Land.Registration {

  /// <summary>Gets information about a resource historic recording acts tract.</summary>
  public class ResourceTract {

    #region Constructors and parsers

    private ResourceTract(Resource resource) {
      this.Resource = resource;
    }

    static internal ResourceTract Parse(Resource resource) {
      Assertion.AssertObject(resource, "resource");

      return new ResourceTract(resource);
    }

    #endregion Constructors and parsers

    #region Properties

    public RecordingAct FirstRecordingAct {
      get {
        FixedList<RecordingAct> recordingActs = this.GetRecordingActs();
        if (recordingActs.Count != 0) {
          return recordingActs[0];
        } else {
          throw new LandRegistrationException(LandRegistrationException.Msg.PropertyDoesNotHaveAnyRecordingActs,
                                              this.Resource.UID);
        }
      }
    }

    public RecordingAct LastRecordingAct {
      get {
        FixedList<RecordingAct> domainActs = this.GetRecordingActs();
        if (domainActs.Count != 0) {
          return domainActs[domainActs.Count - 1];
        } else {
          return RecordingAct.Empty;
        }
      }
    }

    public Resource Resource {
      get;
      private set;
    }

    #endregion Properties

    #region Methods

    public FixedList<Certificate> GetEmittedCerificates() {
      return CertificatesData.ResourceEmittedCertificates(this.Resource);
    }


    public FixedList<RecordingAct> GetFullRecordingActs() {
      return ResourceTractData.GetResourceFullTractIndex(this.Resource);
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


    public Recording GetLastPhysicalRecording() {
      return this.FirstRecordingAct.PhysicalRecording;
    }


    public FixedList<RecordingAct> GetRecordingActs() {
      return ResourceTractData.GetResourceRecordingActList(this.Resource);
    }


    public FixedList<RecordingAct> GetRecordingActsUntil(RecordingAct breakAct,
                                                         bool includeBreakAct) {
      return ResourceTractData.GetResourceRecordingActListUntil(this.Resource,
                                                                breakAct, includeBreakAct);
    }

    public FixedList<RecordingAct> GetClosedRecordingActsUntil(RecordingDocument breakingDocument,
                                                               bool includeBreakingDocument) {
      var tract = ResourceTractData.GetResourceRecordingActList(this.Resource);

      if (includeBreakingDocument) {
        return tract.FindAll((x) => ((x.Document.IsClosed &&
                                      x.Document.PresentationTime < breakingDocument.PresentationTime)) ||
                                      x.Document.Equals(breakingDocument));
      } else {
        return tract.FindAll((x) => ((x.Document.IsClosed &&
                                      x.Document.PresentationTime < breakingDocument.PresentationTime)));
      }
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
      if (!(this.Resource is RealEstate)) {
        return tract[0];
      }

      /// Returns the last domain or structure act if founded, otherwise
      /// return the very first act of the real estate.
      var antecedent = tract.FindLast((x) => x.Document.PresentationTime <= presentationTime &&
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

        if (!(this.Resource is RealEstate)) {
          return RecordingAct.Empty;
        }

        /// If no antecedent, then look if the real estate is a new partition.
        /// If it is then return the antecedent of the partitioned or parent real estate.
        var parentRealEstate = ((RealEstate) this.Resource).IsPartitionOf;
        if (!parentRealEstate.IsEmptyInstance) {
          return parentRealEstate.Tract.GetRecordingAntecedent(beforeRecordingAct.Document.PresentationTime);
        } else {
          return RecordingAct.Empty;
        }
      }

      if (this.Resource is RealEstate) {
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


    public bool IsFirstRecordingAct(RecordingAct recordingAct) {
      if (recordingAct.IsAnnotation) {
        return false;
      }

      RecordingAct firstRecordingAct = this.FirstRecordingAct;
      if (firstRecordingAct != RecordingAct.Empty) {
        return firstRecordingAct.Equals(recordingAct);
      } else {
        return true;
      }
    }


    public bool IsLastRecordingAct(RecordingAct recordingAct) {
      if (recordingAct.IsAnnotation) {
        return false;
      }

      RecordingAct lastRecordingAct = this.LastRecordingAct;
      if (lastRecordingAct != RecordingAct.Empty) {
        return lastRecordingAct.Equals(recordingAct);
      } else {
        return true;
      }
    }

    internal RecordingAct TryGetLastActiveChainedAct(RecordingActType chainedRecordingActType,
                                                     RecordingDocument beforeThisDocument) {
      //var tract = this.Resource.Tract.GetRecordingActsUntil(this, false); /// From RecordingAct rule

      var tract = this.GetClosedRecordingActsUntil(beforeThisDocument, true);

      // Lookup for the last chainedRecordingActType occurrence, possibly including acts in beforeThisDocument
      RecordingAct lastChainedAct =
            tract.FindLast( (x) => (x.RecordingActType.Equals(chainedRecordingActType) &&
                                    x.WasAliveOn(beforeThisDocument.PresentationTime)));

      // If there aren't any chainedRecordingActType acts in the tract, then return null.
      if (lastChainedAct == null) {
        return null;
      }

      // Assert that the founded chained act was not used by any act recorded
      // after lastChainedAct AND before beforeThisDocument was recorded

      int startIndex = tract.IndexOf(lastChainedAct);

      // Get the index of the last closed recording act previous to beforeThisDocument
      RecordingAct lastActToSearch =
              tract.FindLast( (x) => x.Document.PresentationTime < beforeThisDocument.PresentationTime);

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

  }  // class ResourceTract

}  // namespace Empiria.Land.Registration
