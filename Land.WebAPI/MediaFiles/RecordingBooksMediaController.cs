/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                  Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RecordingBooksMediaController                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : API to retrieve and process recording book media files.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Linq;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Media.Adapters;
using Empiria.Land.Media.UseCases;

namespace Empiria.Land.Media.WebApi {

  /// <summary>API to retrieve and process recording book media files.</summary>
  public class RecordingBooksMediaController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/media-files/files-count")]
    public CollectionModel FilesCount([FromUri] string path) {
      var di = new System.IO.DirectoryInfo(path);

      var files = di.GetFiles().Select(x => x.Name).ToArray();

      return new CollectionModel(this.Request, files);
    }


    [HttpGet]
    [Route("v5/land/media-files/recording-books/{recordingBookUID:guid}")]
    public CollectionModel GetRecordingBookImages([FromUri] string recordingBookUID) {

      using (var usecases = RecordingBookMediaUseCases.UseCaseInteractor()) {
        FixedList<LandMediaFileDto> recordingBookImages = usecases.GetRecordingBookImages(recordingBookUID);

        return new CollectionModel(this.Request, recordingBookImages);
      }
    }


    [HttpPost]
    [Route("v5/land/media-files/process-images")]
    public NoDataModel ProcessRecordingBooksImages() {
      var processor = MediaFilesProcessor.GetInstance();

      processor.ProcessImages();

      return new NoDataModel(this.Request);
    }

    #endregion Web Apis

  }  // class RecordingBooksMediaController

} //  namespace Empiria.Land.Media.WebApi
