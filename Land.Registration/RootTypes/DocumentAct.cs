/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DocumentAct                                    Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 25/Jun/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Represents a recording act that applies to documents, not to resources.                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;

namespace Empiria.Land.Registration {

  /// <summary>Represents a recording act that applies to documents, not to resources.</summary>
  public class DocumentAct : RecordingAct {

    #region Constructors and parsers

    private DocumentAct(RecordingActType powertype) : base(powertype) {
      // Required by Empiria Framework for all partitioned types.
    }

    internal DocumentAct(RecordingActType recordingActType,
                         RecordingDocument document) : base(recordingActType, document) {
      this.AttachDocument();
    }

    static public new DocumentAct Parse(int id) {
      return BaseObject.ParseId<DocumentAct>(id);
    }

    #endregion Constructors and parsers

    #region Private methods

    private void AttachDocument() {
      var target = new DocumentTarget(this, this.Document);

      base.AttachTarget(target);
    }

    #endregion Private methods

  } // class DocumentAct

} // namespace Empiria.Land.Registration
