/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                          Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : TractIndexController                         License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API with methods that return tract indexes for recordable subjects.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.RecordableSubjects.Adapters;
using Empiria.Land.RecordableSubjects.UseCases;

namespace Empiria.Land.RecordableSubjects.WebApi {

  /// <summary>Web API with methods that return tract indexes for recordable subjects.</summary>
  public class TractIndexController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/registration/recordable-subjects/{recordableSubjectUID:guid}/amendable-recording-acts")]
    public SingleObjectModel GetAmendableRecordingActs([FromUri] string recordableSubjectUID,
                                                       [FromUri] string instrumentRecordingUID,
                                                       [FromUri] string amendmentRecordingActTypeUID) {

      Assertion.Require(instrumentRecordingUID, "instrumentRecordingUID");
      Assertion.Require(amendmentRecordingActTypeUID, "amendmentRecordingActTypeUID");

      using (var usecases = TractIndexUseCases.UseCaseInteractor()) {
        TractIndexDto tractIndex = usecases.AmendableRecordingActs(recordableSubjectUID,
                                                                   instrumentRecordingUID,
                                                                   amendmentRecordingActTypeUID);
        return new SingleObjectModel(this.Request, tractIndex);
      }
    }


    #endregion Web Apis

  }  // class TractIndexController

}  //namespace Empiria.Land.RecordableSubjects.WebApi
