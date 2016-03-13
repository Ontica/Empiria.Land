/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : InformationAct                                 Pattern  : Empiria Object Type                 *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents an information recording act that are not limitations and can be applied to        *
*              properties, persons or neither.                                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents an information recording act that are not limitations and can be applied to
  /// properties, persons or neither.</summary>
  public class InformationAct : RecordingAct {

    #region Constructors and parsers

    private InformationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal InformationAct(RecordingActType recordingActType, RecordingDocument document,
                            Resource targetResource) : base(recordingActType, document) {
      Assertion.Assert(recordingActType.AppliesToResources,
                      "{0} doesn't apply to resources (real estate or associations).",
                      recordingActType.DisplayName);

      Assertion.AssertObject(targetResource, "targetResource");

      this.AttachResource(targetResource);
    }

    //internal InformationAct(RecordingActType recordingActType, RecordingDocument document,
    //                        RecordingDocument targetDocument) : base(recordingActType, document) {
    //  Assertion.Assert(recordingActType.AppliesTo == RecordingRuleApplication.Document,
    //                  "{0} acts doesn't apply to documents.", recordingActType.DisplayName);

    //  Assertion.AssertObject(targetDocument, "targetDocument");

    //  this.AttachDocument(targetDocument);
    //}

    //internal InformationAct(RecordingActType recordingActType, RecordingDocument document,
    //                        RecordingAct targetRecordingAct) : base(recordingActType, document) {
    //  Assertion.Assert(recordingActType.AppliesTo == RecordingRuleApplication.Document,
    //                  "{0} acts doesn't apply to recording acts.", recordingActType.DisplayName);

    //  Assertion.AssertObject(targetRecordingAct, "targetRecordingAct");

    //  this.AttachRecordingAct(targetRecordingAct);
    //}

    static public new InformationAct Parse(int id) {
      return BaseObject.ParseId<InformationAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    //private void AttachDocument(RecordingDocument targetDocument) {
    //  var target = new DocumentTarget(this, targetDocument);

    //  base.AddTractItem(target);
    //}

    //private void AttachRecordingAct(RecordingAct targetRecordingAct) {
    //  var target = new RecordingActTarget(this, targetRecordingAct);

    //  base.AddTractItem(target);
    //}

    private void AttachResource(Resource targetResource) {
      var target = new TractItem(this, targetResource);

      base.AddTractItem(target);
    }

    #endregion Private methods

  } // class InformationAct

} // namespace Empiria.Land.Registration
