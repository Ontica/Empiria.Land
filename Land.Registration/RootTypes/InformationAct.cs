/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : InformationAct                                 Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents an information recording act that are not limitations and can be applied to        *
*              properties, persons or neither.                                                               *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
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
                            Resource resource) : base(recordingActType, document) {
      recordingActType.AssertIsApplicableResource(resource);

      this.AttachResource(resource);
    }

    internal InformationAct(RecordingActType recordingActType, RecordingDocument document,
                            Resource resource, Recording physicalRecording)
                                      : base(recordingActType, document, physicalRecording) {
      recordingActType.AssertIsApplicableResource(resource);
      this.AttachResource(resource);
    }

    static public new InformationAct Parse(int id) {
      return BaseObject.ParseId<InformationAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    private void AttachResource(Resource targetResource) {
      var target = new TractItem(this, targetResource);

      base.AddTractItem(target);
    }

    #endregion Private methods

  } // class InformationAct

} // namespace Empiria.Land.Registration
