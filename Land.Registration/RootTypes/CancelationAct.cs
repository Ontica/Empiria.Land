/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : CancelationAct                                 Pattern  : Empiria Object Type                 *
*  Version   : 2.1                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Recording act that serves for cancel other recording act.                                     *
*                                                                                                            *
********************************* Copyright (c) 2009-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Recording act that serves for cancel other recording act.</summary>
  public class CancelationAct : RecordingAct {

    #region Constructors and parsers

    private CancelationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal CancelationAct(RecordingActType recordingActType, RecordingDocument document,
                            Resource resource) : base(recordingActType, document) {
      Assertion.AssertObject(resource, "resource");

      base.AttachResource(resource);

      this.Save();
    }

    internal CancelationAct(RecordingActType recordingActType,
                            RecordingDocument document, Resource resource,
                            RecordingAct recordingActToCancel) : base(recordingActType, document) {
      Assertion.AssertObject(resource, "resource");
      Assertion.AssertObject(recordingActToCancel, "recordingActToCancel");

      base.AttachResource(resource);

      recordingActToCancel.Amend(this);
    }

    static public new CancelationAct Parse(int id) {
      return BaseObject.ParseId<CancelationAct>(id);
    }

    #endregion Constructors and parsers

  } // class CancelationAct

} // namespace Empiria.Land.Registration
