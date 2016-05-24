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

    /// <summary>Marks a resource as canceled. The resource won't be longer available.</summary>
    internal CancelationAct(RecordingActType recordingActType, RecordingDocument document,
                            Resource resource) : base(recordingActType, document) {
      Assertion.AssertObject(resource, "resource");

      base.SetResource(resource, ResourceRole.Canceled);

      this.Save();
    }

    /// <summary>Cancels a recording act. If it's a domain act and the unique one,
    /// then the resource will be canceled too.</summary>
    internal CancelationAct(RecordingActType recordingActType,
                            RecordingDocument document, Resource resource,
                            RecordingAct recordingActToCancel) : base(recordingActType, document) {
      Assertion.AssertObject(resource, "resource");
      Assertion.AssertObject(recordingActToCancel, "recordingActToCancel");

      if (recordingActToCancel.RecordingActType.IsDomainActType) {
        AssertIsValidCancelationForDomainAct(resource, recordingActToCancel);
      }

      base.SetResource(resource, ResourceRole.Informative);

      recordingActToCancel.Amend(this);
    }

    static public new CancelationAct Parse(int id) {
      return BaseObject.ParseId<CancelationAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    private void AssertIsValidCancelationForDomainAct(Resource resource,
                                                      RecordingAct recordingActToCancel) {
      //var tract = resource.GetRecordingActsTract();

      //if (0 != tract.CountAll((x) => x.RecordingActType.IsDomainActType &&
      //                               !x.Equals(recordingActToCancel))) {

      //  Assertion.Assert(resource.LastRecordingAct.Equals(recordingActToCancel),
      //                   "Cancelation of domain acts must be applied only to the " +
      //                   "latest domain act and that act also must be the last " +
      //                   "recording act in the resource tract.");
      //}
    }

    #endregion Private methods

  } // class CancelationAct

} // namespace Empiria.Land.Registration
