/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Extranet Services                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RecordableDocumentsController                License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Public API to retrieve recordable documents.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Recording.UseCases;

namespace Empiria.Land.Recording.WebApi {

  /// <summary>Public API to retrieve recordable documents.</summary>
  public class RecordableDocumentsController : WebApiController {

    #region Public methods

    [HttpGet]
    [Route("v5/land/recorded-documents/{documentUID:length(20)}")]
    public SingleObjectModel GetRecordedDocument([FromUri] string documentUID) {

      using (var usecases = RecordedDocumentsUseCases.GetUseCaseInteractor()) {
        RecordedDocumentDto documentDto = usecases.GetRecordedDocument(documentUID);

        return new SingleObjectModel(this.Request, documentDto);
      }

    }

    #endregion Public methods

  }  // class RecordableDocumentsController

}  //namespace Empiria.Land.Recording.WebApi
