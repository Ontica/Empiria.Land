/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RecordableSubjectsRegistrationController     License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API for recordable subjects edition in the context of a recording act.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.Json;
using Empiria.WebApi;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;
using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API for recordable subjects edition in the context of a recording act.</summary>
  public class RecordableSubjectsRegistrationController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/" +
           "recording-acts/{recordingActUID:guid}/tract-index")]
    public SingleObjectModel GetTractIndex([FromUri] string instrumentRecordingUID,
                                           [FromUri] string recordingActUID) {

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        TractIndexDto tractIndex = usecases.GetTractIndex(instrumentRecordingUID,
                                                          recordingActUID);

        return new SingleObjectModel(this.Request, tractIndex);
      }
    }


    [HttpPost, HttpPatch, HttpPut]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/" +
           "recording-acts/{recordingActUID:guid}/recordable-subject")]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/" +
           "recording-acts/{recordingActUID:guid}/update-recordable-subject")]
    public SingleObjectModel UpdateRecordableSubject([FromUri] string instrumentRecordingUID,
                                                     [FromUri] string recordingActUID,
                                                     [FromBody] object body) {

      base.RequireBody(body);

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        RecordableSubjectFields recordableSubjectFields = MapToRecordableSubjectFields(body);

        InstrumentRecordingDto instrumentRecording =
                                      usecases.UpdateRecordableSubject(instrumentRecordingUID,
                                                                       recordingActUID,
                                                                       recordableSubjectFields);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }

    #endregion Web Apis

    #region Helper methods

    private RecordableSubjectFields MapToRecordableSubjectFields(object body) {
      var json = base.GetJsonFromBody(body);

      var subjectType = json.Get<RecordableSubjectType>("type", RecordableSubjectType.None);

      switch (subjectType) {
        case RecordableSubjectType.Association:
          return JsonConverter.ToObject<AssociationFields>(json.ToString());

        case RecordableSubjectType.NoProperty:
          return JsonConverter.ToObject<NoPropertyFields>(json.ToString());

        case RecordableSubjectType.RealEstate:
          return JsonConverter.ToObject<RealEstateFields>(json.ToString());

        default:
          throw Assertion.EnsureNoReachThisCode($"Unrecognized recordable subject type {subjectType}.");
      }
    }

    #endregion Helper methods

  }  // class RecordableSubjectsRegistrationController

}  //namespace Empiria.Land.Registration.WebApi
