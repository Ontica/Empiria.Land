/* Empiria Land 2014 *****************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                   System   : Land Registration System            *
*  Namespace : Empiria.Land.Documentation                     Assembly : Empiria.Land.Documentation          *
*  Type      : ImagingItem                                    Pattern  : Empiria Object Type                 *
*  Version   : 2.0        Date: 23/Oct/2014                   License  : GNU AGPLv3  (See license.txt)       *
*                                                                                                            *
*  Summary   : Represents an imaging item in Land Registration System.                                       *
*                                                                                                            *
********************************* Copyright (c) 2009-2014. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections.Generic;
using System.Data;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Security;

using Empiria.Land.Registration;

namespace Empiria.Land.Documentation {

  /// <summary>Represents an imaging item in Land Registration System.</summary>
  public class ImagingItem : BaseObject {

    #region Constructors and parsers

    private ImagingItem() {
      // Required by Empiria Framework.
    }

    static public ImagingItem Parse(int id) {
      return BaseObject.ParseId<ImagingItem>(id);
    }

    static public ImagingItem Empty {
      get {
        return BaseObject.ParseEmpty<ImagingItem>();
      }
    }

    #endregion Constructors and parsers

    #region Public properties

    [DataField("RecordingBookId")]
    public RecordingBook RecordingBook {
      get;
      private set;
    }

    [DataField("DocumentId")]
    public RecordingDocument Document {
      get;
      private set;
    }

    [DataField("ManualRecordingId")]
    public Recording ManualRecording {
      get;
      private set;
    }

    [DataField("BaseFolderId")]
    private LazyInstance<ImagingItem> _baseFolder = LazyInstance<ImagingItem>.Empty;
    public ImagingItem BaseFolder {
      get {
        return _baseFolder.Value;
      }
      private set {
        _baseFolder.Value = value;
      }
    }

    [DataField("RelativePath")]
    public string RelativePath {
      get;
      private set;
    }

    [DataField("ImagingItemExtData")]
    public JsonObject ImagingItemExtData {
      get;
      private set;
    }

    [DataField("FilesCount")]
    public int FilesCount {
      get;
      private set;
    }

    [DataField("FilesTotalSize")]
    public int FilesTotalSize {
      get;
      private set;
    }

    [DataField("ProtectionSeal")]
    internal string ProtectionSeal {
      get;
      private set;
    }

    [DataField("DigitalizedById")]
    public Contact DigitalizedBy {
      get;
      private set;
    }

    #endregion Public properties

    #region Public methods

    protected override void OnSave() {
      DataServices.WriteImagingItem(this);
    }

    #endregion Public methods

  } // class ImagingItem

} // namespace Empiria.Land.Documentation
