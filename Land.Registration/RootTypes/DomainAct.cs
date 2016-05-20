/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DomainAct                                      Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a domain traslative recording act.                                                 *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
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
      Assertion.AssertObject(property, "property");

      base.AttachResource(property, percentage);
    }

    internal DomainAct(RecordingActType recordingActType, RecordingDocument document,
                       RealEstate property, Recording physicalRecording,
                       decimal percentage = decimal.One) : base(recordingActType, document, physicalRecording) {
      Assertion.AssertObject(property, "property");

      base.AttachResource(property, percentage);
    }

    static public new DomainAct Parse(int id) {
      return BaseObject.ParseId<DomainAct>(id);
    }

    #endregion Constructors and parsers

  } // class DomainAct

} // namespace Empiria.Land.Registration
