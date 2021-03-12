/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                Component : Domain Layer                            *
*  Assembly : Empiria.Land.Core.dll                      Pattern   : Information Holder                      *
*  Type     : LandMediaFileSet                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a set of LandMediaFile objects related to a specific Land entity.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Empiria.Storage;

using Empiria.Land.Media.Adapters;

namespace Empiria.Land.Media {

  /// <summary>Holds a set of LandMediaFile objects related to a specific Land entity.</summary>
  internal class LandMediaFileSet {


    #region Fields

    private readonly List<LandMediaFile> _mediaFileslist;

    #endregion Fields


    #region Constructors and parsers

    private LandMediaFileSet(BaseObject entity) {
      this.Entity = entity;

      _mediaFileslist = GetEntityMediaFilesList(entity);
    }


    static internal LandMediaFileSet GetFor(BaseObject entity) {
      Assertion.AssertObject(entity, "entity");

      return new LandMediaFileSet(entity);
    }

    #endregion Constructors and parsers

    #region Properties

    public BaseObject Entity {
      get;
    }

    internal FixedList<LandMediaFile> GetFiles() {
      return _mediaFileslist.ToFixedList();
    }

      #endregion Properties

    #region Methods

    internal async Task Add(LandMediaFileFields fields, Stream fileStream) {
      Assertion.AssertObject(fields, "fields");
      Assertion.AssertObject(fileStream, "fileStream");

      using (var service = MediaFileServices.ServiceInteractor()) {
        var storage = service.GetStorageFor(this.Entity);

        LandMediaFile mediaFile = await service.CreateMediaFile<LandMediaFile>(storage, fields, fileStream);

        service.RelateMediaFile(mediaFile, this.Entity, $"Instrument.File.{fields.MediaContent}");

        _mediaFileslist.Add(mediaFile);
      }
    }


    internal void AssertCanBeAdded(LandMediaFileFields fields) {
      Assertion.AssertObject(fields, "fields");

      if (_mediaFileslist.Exists(x => x.MediaContent == fields.MediaContent)) {
        Assertion.AssertFail($"A file with MediaContent '{fields.MediaContent}' already exists for this entity.");
      }
    }


    internal void AssertCanBeRemoved(string mediaObjectUID) {
      // ToDo
    }


    internal void AssertCanBeReplaced(string mediaFileUID) {
      // ToDo
    }


    static private List<LandMediaFile> GetEntityMediaFilesList(BaseObject entity) {
      using (var service = MediaFileServices.ServiceInteractor()) {
        var files = service.GetRelatedMediaFiles<LandMediaFile>(entity);

        return new List<LandMediaFile>(files);
      }
    }


    public void Remove(string mediaFileUID) {
      Assertion.AssertObject(mediaFileUID, "mediaFileUID");

      using (var service = MediaFileServices.ServiceInteractor()) {
        var mediaFileToRemove = _mediaFileslist.Find(x => x.UID == mediaFileUID);

        service.RemoveMediaFile(mediaFileToRemove, this.Entity);

        _mediaFileslist.Remove(mediaFileToRemove);
      }
    }


    public async Task Replace(string mediaFileUID, LandMediaFileFields fields, Stream fileStream) {
      Assertion.AssertObject(mediaFileUID, "mediaFileUID");
      Assertion.AssertObject(fields, "fields");
      Assertion.AssertObject(fileStream, "fileStream");

      using (var service = MediaFileServices.ServiceInteractor()) {
        var mediaFileToReplace = _mediaFileslist.Find(x => x.UID == mediaFileUID);

        LandMediaFile newMediaFile = await service.ReplaceMediaFile<LandMediaFile>(mediaFileToReplace,
                                                                                   fields, fileStream);

        _mediaFileslist.Remove(mediaFileToReplace);
        _mediaFileslist.Add(newMediaFile);
      }
    }


    #endregion Methods

  }  // class LandMediaFileSet

}  // namespace Empiria.Land.Media
