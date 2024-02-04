/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : LimitationAct                                  Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a property limitation or property assessment or mortgage act.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a property limitation or property assessment or mortgage act.</summary>
  public class LimitationAct : RecordingAct {

    #region Constructors and parsers

    private LimitationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }


    internal LimitationAct(RecordingActType recordingActType,
                           LandRecord landRecord, RealEstate realEstate,
                           decimal percentage = decimal.One) : base(recordingActType, landRecord) {
      Assertion.Require(recordingActType.AppliesTo == RecordingRuleApplication.RealEstate,
                       $"{recordingActType.DisplayName} doesn't apply to real estates.");

      Assertion.Require(realEstate, nameof(realEstate));

      this.SetRealEstate(realEstate, percentage);
    }


    internal LimitationAct(RecordingActType recordingActType,
                           LandRecord landRecord, RealEstate realEstate,
                           BookEntry bookEntry,
                           decimal percentage = decimal.One) : base(recordingActType, landRecord, bookEntry) {
      Assertion.Require(recordingActType.AppliesTo == RecordingRuleApplication.RealEstate,
                       $"{recordingActType.DisplayName} doesn't apply to real estates.");

      Assertion.Require(realEstate, nameof(realEstate));

      this.SetRealEstate(realEstate, percentage);
    }


    static public new LimitationAct Parse(int id) {
      return BaseObject.ParseId<LimitationAct>(id);
    }

    #endregion Constructors and parsers


    private void SetRealEstate(RealEstate realEstate, decimal percentage) {
      var tract = realEstate.Tract.GetRecordingActs();

      if (tract.Count != 0) {     // This is not the first act of the real estate
        base.SetResource(realEstate, ResourceRole.Informative);
        return;
      }

      if (realEstate.IsPartition) {
        base.SetResource(realEstate, ResourceRole.PartitionOf, realEstate.IsPartitionOf, percentage: percentage);
      } else {
        base.SetResource(realEstate, ResourceRole.Created, percentage: percentage);
      }
    }

  } // class LimitationAct

} // namespace Empiria.Land.Registration
