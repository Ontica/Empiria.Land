/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DomainAct                                      Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a domain traslative recording act.                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents a domain traslative recording act.</summary>
  public class DomainAct : RecordingAct {

    #region Constructors and parsers

    private DomainAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal DomainAct(RecordingActType recordingActType,
                       RecordingDocument document, RealEstate property,
                       decimal percentage = decimal.One) : base(recordingActType, document) {
      this.AttachRealEstate(property, percentage);
    }

    internal DomainAct(RecordingActType recordingActType, RecordingDocument document,
                       RealEstate property, BookEntry bookEntry,
                       decimal percentage = decimal.One) : base(recordingActType, document, bookEntry) {
      this.AttachRealEstate(property, percentage);
    }

    static public new DomainAct Parse(int id) {
      return BaseObject.ParseId<DomainAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    private void AttachRealEstate(RealEstate property, decimal percentage) {
      Assertion.Require(property, "property");

      var tract = property.Tract.GetRecordingActs();

      this.AssertNoLimitationActs(tract);

      if (tract.Count != 0) {     // This is not the first act of the real estate
        base.SetResource(property, ResourceRole.Informative, percentage: percentage);
        return;
      }

      if (property.IsPartition) {
        base.SetResource(property, ResourceRole.PartitionOf, property.IsPartitionOf, percentage);
      } else {
        base.SetResource(property, ResourceRole.Created, percentage: percentage);
      }
    }


    private void AssertNoLimitationActs(FixedList<RecordingAct> tract) {
      //var limitationActs = tract.FindAll((x) => x.RecordingActType.IsLimitationActType);

      //if (limitationActs.Count > 0) {
      //  Assertion.AssertFail("This property has limitation acts, so I can't append a new domain act.");
      //}
    }

    #endregion Private methods

  } // class DomainAct

} // namespace Empiria.Land.Registration
