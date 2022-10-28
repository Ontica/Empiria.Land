/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : ModificationAct                                Pattern  : Empiria Object Type                 *
*  Version   : 3.0                                            License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Recording act that modifies another recording act, resource, document or party.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Recording act that modifies another recording act, resource, document or party.</summary>
  public class ModificationAct : RecordingAct {

    #region Constructors and parsers

    private ModificationAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal ModificationAct(RecordingActType recordingActType, RecordingDocument document,
                             Resource resource) : base(recordingActType, document) {
      Assertion.Require(resource, "resource");

      this.SetResource(resource);

      this.Save();
    }

    internal ModificationAct(RecordingActType recordingActType,
                             RecordingDocument document, Resource resource,
                             RecordingAct recordingActToModify) : base(recordingActType, document) {
      Assertion.Require(resource, "resource");
      Assertion.Require(recordingActToModify, "recordingActToModify");

      base.SetResource(resource);

      recordingActToModify.Amend(this);
    }

    static public new ModificationAct Parse(int id) {
      return BaseObject.ParseId<ModificationAct>(id);
    }


    #endregion Constructors and parsers

  } // class ModificationAct

} // namespace Empiria.Land.Registration
