/* Empiria Land 2015 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Registration                      Assembly : Empiria.Land.Registration           *
*  Type      : DocumentTarget                                 Pattern  : Association Class                   *
*  Version   : 2.0        Date: 04/Jan/2015                   License  : Please read license.txt file        *
*                                                                                                            *
*  Summary   : Application of a recording act with a party (person or organization).                         *
*                                                                                                            *
********************************* Copyright (c) 2009-2015. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Data;

using Empiria.Contacts;
using Empiria.DataTypes;
using Empiria.Security;

using Empiria.Land.Registration.Data;

namespace Empiria.Land.Registration {

  /// <summary>Application of a recording act with a party (person or organization).</summary>
  public class DocumentTarget : RecordingActTarget {

    #region Constructors and parsers

    protected DocumentTarget() {
      // Required by Empiria Framework.
    }

    internal DocumentTarget(RecordingAct recordingAct, RecordingDocument document) : base(recordingAct) {
      Assertion.AssertObject(document, "document");

      this.Document = document;
    }

    static public new ResourceTarget Parse(int id) {
      return BaseObject.ParseId<ResourceTarget>(id);
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("TargetDocumentId")]
    public RecordingDocument Document {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    protected override void OnSave() {
      RecordingActsData.WriteDocumentTarget(this);
    }

    #endregion Public methods

  } // class DocumentTarget

} // namespace Empiria.Land.Registration
