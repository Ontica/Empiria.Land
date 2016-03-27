/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DomainAct                                      Pattern  : Empiria Object Type                 *
*  Version   : 2.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a domain traslative recording act.                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents a domain traslative recording act.</summary>
  public class DomainAct : RecordingAct {

    #region Constructors and parsers

    private DomainAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal DomainAct(RecordingActType recordingActType, RecordingDocument document,
                       RealEstate property, decimal percentage = 1.0m) : base(recordingActType, document) {
      Assertion.AssertObject(property, "property");

      this.AttachResource(property, percentage);
    }

    internal DomainAct(RecordingActType recordingActType, RecordingDocument document,
                       RealEstate property, Recording physicalRecording, decimal percentage = 1.0m)
                       : base(recordingActType, document, physicalRecording) {
      Assertion.AssertObject(property, "property");

      this.AttachResource(property, percentage);
    }

    static public new DomainAct Parse(int id) {
      return BaseObject.ParseId<DomainAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    private void AttachResource(RealEstate resource, decimal percentage) {
      var tractItem = new TractItem(this, resource,
                                    recordingActPercentage:percentage);

      base.AddTractItem(tractItem);
    }

    #endregion Private methods

  } // class DomainAct

} // namespace Empiria.Land.Registration
