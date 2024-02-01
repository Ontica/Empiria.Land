/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Digitalization Services                 Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : DigitalizationController                     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web Api to process digital media files.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

namespace Empiria.Land.Digitalization.WebApi {

  /// <summary> Web Api to process digital media files.</summary>
  public class DigitalizationController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/digitalization/process-recording-books-images")]
    public NoDataModel ProcessRecordingBooksImages() {
      var processor = MediaFilesProcessor.GetInstance();

      processor.ProcessImages();

      return new NoDataModel(this.Request);
    }

    #endregion Web Apis

  }  // class DigitalizationController

} //  namespace Empiria.Land.Media.WebApi
