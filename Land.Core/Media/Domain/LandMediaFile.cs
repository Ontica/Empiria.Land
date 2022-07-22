/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : LandMediaFile                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : A media file related to an Empiria Land entity like instrument, transaction or recording book. *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.Json;
using Empiria.Storage;

using Empiria.StateEnums;
using Empiria.Security;


namespace Empiria.Land.Media {

  /// <summary>A media file related to an Empiria Land entity like instrument,
  /// transaction or recording book.</summary>
  internal class LandMediaFile : BaseObject, IProtected {

    #region Constructors and parsers

    protected LandMediaFile() {
      //  no-op
    }

    static public LandMediaFile Parse(int id) {
      return BaseObject.ParseId<LandMediaFile>(id);
    }


    static public LandMediaFile Parse(string uid) {
      return BaseObject.ParseKey<LandMediaFile>(uid);
    }

    static public LandMediaFile Empty {
      get {
        return BaseObject.ParseEmpty<LandMediaFile>();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("MediaContent")]
    public string MediaContent {
      get;
      private set;
    }

    [DataField("MediaType")]
    public string MediaType {
      get;
      private set;
    }

    [DataField("MediaLength")]
    public int Length {
      get;
      private set;
    }

    [DataField("OriginalFileName")]
    public string OriginalFileName {
      get;
      private set;
    }


    [DataField("StorageId", Default = "Empiria.Storage.MediaStorage.Default")]
    public MediaStorage Storage {
      get;
      private set;
    }


    [DataField("BaseFolderId")]
    internal int BaseFolderId {
      get;
      private set;
    }


    [DataField("FilePath")]
    internal string FilePath {
      get;
      private set;
    }


    [DataField("FileName")]
    internal string FileName {
      get;
      private set;
    }


    public string FullPath {
      get {
        return StorageUtilityMethods.GetFileFullName(this.Storage,
                                                     StorageUtilityMethods.CombinePaths(this.FilePath, this.FileName));
      }
    }


    [DataField("FileHashCode")]
    public string HashCode {
      get;
      private set;
    }


    [DataField("MediaFileExtData")]
    protected internal JsonObject ExtensionData {
      get;
      private set;
    }


    [DataField("PostingTime")]
    public DateTime PostingTime {
      get;
      private set;
    } = ExecutionServer.DateMaxValue;


    [DataField("PostedById")]
    public Contact PostedBy {
      get;
      private set;
    }


    [DataField("MediaFileStatus", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get;
      private set;
    }


    public string Url {
      get {
        return $"{this.Storage.Url}/{this.FilePath}/{this.FileName}";
      }
    }

    #endregion Properties

    #region Integrity protection members

    int IProtected.CurrentDataIntegrityVersion {
      get {
        return 1;
      }
    }


    object[] IProtected.GetDataIntegrityFieldValues(int version) {
      if (version == 1) {
        return new object[] {
          1, "MediaId", this.Id, "MediaUID", this.UID, "MediaType", this.MediaType,
          "MediaLength", this.Length, "OriginalFileName", this.OriginalFileName,
          "StorageId", this.Storage.Id, "FilePath", this.FilePath, "FileName", this.FileName,
          "MediaHashCode", this.HashCode, "ExtData", this.ExtensionData.ToString(),
          "PostingTime", this.PostingTime, "PostedById", this.PostedBy.Id,
          "MediaStatus", (char) this.Status
        };
      }
      throw new SecurityException(SecurityException.Msg.WrongDIFVersionRequested, version);
    }


    private IntegrityValidator _validator = null;
    public IntegrityValidator Integrity {
      get {
        if (_validator == null) {
          _validator = new IntegrityValidator(this);
        }
        return _validator;
      }
    }


    #endregion Integrity protection members

    #region Methods

    internal void Delete() {
      Assertion.Require(this.Status == EntityStatus.Active,
                       "MediaObject must be in 'Active' status.");

      this.Status = EntityStatus.Deleted;

      this.Save();
    }


    //private void LoadFields(MediaFileFields fields) {
    //  this.MediaContent = fields.MediaContent;
    //  this.MediaType = fields.MediaType;
    //  this.Length = fields.MediaLength;
    //  this.OriginalFileName = fields.OriginalFileName;
    //  this.FolderPath = fields.FolderPath;
    //  this.HashCode = fields.FileHashCode;
    //}


    protected override void OnSave() {
      if (base.IsNew) {
        this.PostedBy = EmpiriaUser.Current.AsContact();
        this.PostingTime = DateTime.Now;
      }

      throw new NotImplementedException("OnSave()");
      // MediaRepository.WriteMediaFile(this);
    }

    #endregion Methods

  }  // class LandMediaFile

}  // namespace Empiria.Land.Media
