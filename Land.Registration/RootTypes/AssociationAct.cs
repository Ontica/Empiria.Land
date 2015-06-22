/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : AssociationAct                                 Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 25/Jun/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a recording act that applies to associations or nonprofit organizations.           *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents a recording act that applies to associations or nonprofit organizations.</summary>
  public class AssociationAct : RecordingAct {

    #region Constructors and parsers

    private AssociationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal AssociationAct(RecordingActType recordingActType, RecordingDocument document,
                            Association association) : base(recordingActType, document) {
      this.AttachAssociation(association);
    }

    internal AssociationAct(RecordingActType recordingActType, RecordingDocument document,
                            Association association, Recording physicalRecording)
                            : base(recordingActType, document, physicalRecording) {
      this.AttachAssociation(association);
    }

    static public new AssociationAct Parse(int id) {
      return BaseObject.ParseId<AssociationAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    private void AttachAssociation(Association association) {
      Assertion.AssertObject(association, "association");

      var target = new ResourceTarget(this, association, ResourceRole.Informative);

      base.AttachTarget(target);
    }

    #endregion Private methods

  } // class AssociationAct

} // namespace Empiria.Land.Registration
