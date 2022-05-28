/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Media Files Management                  Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : InstrumentMediaFilesController               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to add, remove and replace legal instrument's media files.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Media.Adapters;
using Empiria.Land.Media.UseCases;

namespace Empiria.Land.Media.WebApi {

  /// <summary>Web API used to add, remove and replace legal instrument's media files.</summary>
  public class InstrumentMediaFilesController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/instruments/{instrumentUID:guid}/media-files/{mediaFileUID:guid}")]
    public SingleObjectModel ReplaceInstrumentMediaFile([FromUri] string instrumentUID,
                                                        [FromUri] string mediaFileUID) {
      var httpRequest = HttpContext.Current.Request;

      AssertIsValidForUploadInstrumentMediaFile(httpRequest);

      LandMediaFileFields fields = MapToInstrumentMediaFileFields(httpRequest);

      var inputStream = httpRequest.Files[0].InputStream;

      using (var usecases = LandMediaFilesUseCases.UseCaseInteractor()) {
        var instrumentDto = usecases.ReplaceInstrumentMediaFile(instrumentUID, mediaFileUID,
                                                                fields, inputStream);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }


    [HttpDelete]
    [Route("v5/land/instruments/{instrumentUID:guid}/media-files/{mediaFileUID:guid}")]
    public SingleObjectModel RemoveInstrumentMediaFile([FromUri] string instrumentUID,
                                                       [FromUri] string mediaFileUID) {

      using (var usecases = LandMediaFilesUseCases.UseCaseInteractor()) {
        var instrumentDto = usecases.RemoveInstrumentMediaFile(instrumentUID, mediaFileUID);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }


    [HttpPost]
    [Route("v5/land/instruments/{instrumentUID:guid}/media-files")]
    public async Task<SingleObjectModel> UploadInstrumentMediaFile([FromUri] string instrumentUID) {
      var httpRequest = HttpContext.Current.Request;

      AssertIsValidForUploadInstrumentMediaFile(httpRequest);

      LandMediaFileFields fields = MapToInstrumentMediaFileFields(httpRequest);

      var inputStream = httpRequest.Files[0].InputStream;

      using (var usecases = LandMediaFilesUseCases.UseCaseInteractor()) {
        var instrumentDto = await usecases.AddInstrumentMediaFile(instrumentUID, fields, inputStream);

        return new SingleObjectModel(this.Request, instrumentDto);
      }
    }

    #endregion Web Apis

    #region Helper methods

    private void AssertIsValidForUploadInstrumentMediaFile(HttpRequest httpRequest) {
      Assertion.Require(httpRequest, "httpRequest");
      Assertion.Require(httpRequest.Files.Count == 1, "The request does not have the file to be uploaded.");

      var form = httpRequest.Form;

      Assertion.Require(form, "The request must be of type 'multipart/form-data'.");

      Assertion.Require(form["mediaContent"], "'mediaContent' form field is required");
    }


    private LandMediaFileFields MapToInstrumentMediaFileFields(HttpRequest httpRequest) {
      HttpPostedFile file = httpRequest.Files[0];
      NameValueCollection form = httpRequest.Form;

      var fields = new LandMediaFileFields();

      fields.MediaContent = LandMediaFileFields.ConvertMediaContent(form["mediaContent"]);

      fields.MediaType = file.ContentType;
      fields.MediaLength = file.ContentLength;
      fields.OriginalFileName = file.FileName;

      return fields;
    }

    #endregion Helper methods

  }  // class InstrumentMediaFilesController

}  // namespace Empiria.Land.Media.WebApi
