/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : LimitationAct                                  Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a property limitation or property assessment or mortgage act.                      *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System.Data;

namespace Empiria.Land.Registration {

  /// <summary>Represents a property limitation or property assessment or mortgage act.</summary>
  public class LimitationAct : RecordingAct {

    #region Constructors and parsers

    private LimitationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal LimitationAct(RecordingActType recordingActType,
                           RecordingDocument document, RealEstate property,
                           decimal percentage = decimal.One) : base(recordingActType, document) {
      Assertion.Assert(recordingActType.AppliesTo == RecordingRuleApplication.RealEstate,
                       "{0} doesn't apply to properties (real estate).", recordingActType.DisplayName);

      Assertion.AssertObject(property, "property");

      this.AttachProperty(property, percentage);
    }

    static public new LimitationAct Parse(int id) {
      return BaseObject.ParseId<LimitationAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    private void AttachProperty(RealEstate property, decimal percentage) {
      var tractItem = new TractItem(this, property, recordingActPercentage: percentage);

      base.AddTractItem(tractItem);
    }

    #endregion Private methods

  } // class LimitationAct

} // namespace Empiria.Land.Registration
