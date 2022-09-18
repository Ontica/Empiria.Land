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
using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;

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
        SubjectHistoryDto tractIndex = usecases.AmendableRecordingActs(recordableSubjectUID,
                                                                   instrumentRecordingUID,
                                                                   amendmentRecordingActTypeUID);
        return new SingleObjectModel(this.Request, tractIndex);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/recordable-subjects/{recordableSubjectUID:guid}/tract-index")]
    public SingleObjectModel GetFullTractIndex([FromUri] string recordableSubjectUID) {

      using (var usecases = TractIndexUseCases.UseCaseInteractor()) {
        SubjectHistoryDto tractIndex = usecases.TractIndex(recordableSubjectUID);

        return new SingleObjectModel(this.Request, tractIndex);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/recordable-subjects/{recordableSubjectUID:guid}/tract-index/recording-act-types")]
    public CollectionModel GetTractIndexRecordingActTypes([FromUri] string recordableSubjectUID) {

      using (var usecases = RegistrationRulesUseCases.UseCaseInteractor()) {
        FixedList<RecordingActTypeGroupDto> groups = usecases.RecordingActTypesForRecordableSubject(recordableSubjectUID);

        return new CollectionModel(this.Request, groups);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/recordable-subjects/{recordableSubjectUID:guid}/tract-index/close")]
    public SingleObjectModel CloseTractIndex([FromUri] string recordableSubjectUID) {

      using (var usecases = TractIndexUseCases.UseCaseInteractor()) {

        usecases.CloseTractIndex(recordableSubjectUID);

        SubjectHistoryDto tractIndex = usecases.TractIndex(recordableSubjectUID);

        return new SingleObjectModel(this.Request, tractIndex);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/recordable-subjects/{recordableSubjectUID:guid}/tract-index")]
    public SingleObjectModel CreateRecordingActInTractIndex([FromUri] string recordableSubjectUID,
                                                            [FromBody] RegistrationCommand command) {
      base.RequireBody(command);

      using (var usecases = TractIndexUseCases.UseCaseInteractor()) {

        usecases.CreateRecordingAct(recordableSubjectUID, command);

        SubjectHistoryDto tractIndex = usecases.TractIndex(recordableSubjectUID);

        return new SingleObjectModel(this.Request, tractIndex);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/recordable-subjects/{recordableSubjectUID:guid}/tract-index/open")]
    public SingleObjectModel OpenTractIndex([FromUri] string recordableSubjectUID) {

      using (var usecases = TractIndexUseCases.UseCaseInteractor()) {
        usecases.OpenTractIndex(recordableSubjectUID);

        SubjectHistoryDto tractIndex = usecases.TractIndex(recordableSubjectUID);

        return new SingleObjectModel(this.Request, tractIndex);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/recordable-subjects/{recordableSubjectUID:guid}/tract-index/{recordingActUID:guid}")]
    public SingleObjectModel RemoveRecordingActFromTractIndex([FromUri] string recordableSubjectUID,
                                                              [FromUri] string recordingActUID) {

      using (var usecases = TractIndexUseCases.UseCaseInteractor()) {

        usecases.RemoveRecordingAct(recordableSubjectUID, recordingActUID);

        SubjectHistoryDto tractIndex = usecases.TractIndex(recordableSubjectUID);

        return new SingleObjectModel(this.Request, tractIndex);
      }
    }


    #endregion Web Apis

  }  // class TractIndexController

}  //namespace Empiria.Land.RecordableSubjects.WebApi
