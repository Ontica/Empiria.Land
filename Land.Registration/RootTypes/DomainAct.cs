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
                       Property property) : base(recordingActType, document) {
      this.AttachProperty(property);
    }

    internal DomainAct(RecordingActType recordingActType, RecordingDocument document,
                       Property property, Recording physicalRecording)
                       : base(recordingActType, document, physicalRecording) {
      this.AttachProperty(property);
    }

    internal DomainAct(RecordingActType recordingActType, RecordingDocument document,
                       Property property, Recording physicalRecording,
                       ResourceRole role, decimal percentage = 1.0m)
                       : base(recordingActType, document, physicalRecording) {
      this.AttachProperty(property, role, percentage);
    }

    static public new DomainAct Parse(int id) {
      return BaseObject.ParseId<DomainAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    private void AttachProperty(Property property) {
      Assertion.AssertObject(property, "property");

      var target = new ResourceTarget(this, property, ResourceRole.Informative);

      base.AttachTarget(target);
    }

    private void AttachProperty(Property property, ResourceRole role, decimal percentage) {
      Assertion.AssertObject(property, "property");

      var target = new ResourceTarget(this, property, role, percentage);

      base.AttachTarget(target);
    }

    #endregion Private methods

  } // class DomainAct

} // namespace Empiria.Land.Registration
