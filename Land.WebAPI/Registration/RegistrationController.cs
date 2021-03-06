﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RegistrationController                       License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API with methods that perform registrar tasks over legal instruments.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.Json;
using Empiria.WebApi;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;
using Empiria.Land.RecordableSubjects.Adapters;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API with methods that perform registrar tasks over legal instruments.</summary>
  public class RegistrationController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}")]
    public SingleObjectModel GetInstrumentRecording([FromUri] string instrumentRecordingUID) {

      using (var usecases = InstrumentRecordingUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording = usecases.GetInstrumentRecording(instrumentRecordingUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/open-registration")]
    public SingleObjectModel OpenInstrumentRecording([FromUri] string instrumentRecordingUID) {

      using (var usecases = InstrumentRecordingUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording = usecases.OpenInstrumentRecording(instrumentRecordingUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/close-registration")]
    public SingleObjectModel CloseInstrumentRecording([FromUri] string instrumentRecordingUID) {

      using (var usecases = InstrumentRecordingUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording = usecases.CloseInstrumentRecording(instrumentRecordingUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts")]
    public SingleObjectModel CreateRecordingAct([FromUri] string instrumentRecordingUID,
                                                [FromBody] RegistrationCommand command) {

      base.RequireBody(command);

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording =
                                    usecases.CreateRecordingAct(instrumentRecordingUID, command);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel GetRecordingAct([FromUri] string instrumentRecordingUID,
                                             [FromUri] string recordingActUID) {

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        RecordingActDto recordingAct = usecases.GetRecordingAct(instrumentRecordingUID, recordingActUID);

        return new SingleObjectModel(this.Request, recordingAct);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel DeleteRecordingAct([FromUri] string instrumentRecordingUID,
                                                [FromUri] string recordingActUID) {

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording =
                                    usecases.RemoveRecordingAct(instrumentRecordingUID, recordingActUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpPatch, HttpPut]
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


    [HttpPatch, HttpPut]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel UpdateRecordingAct([FromUri] string instrumentRecordingUID,
                                                [FromUri] string recordingActUID,
                                                [FromBody] RecordingActFields fields) {
      base.RequireBody(fields);

      using (var usecases = RegistrationUseCases.UseCaseInteractor()) {
        RecordingActDto recordingAct = usecases.UpdateRecordingAct(instrumentRecordingUID,
                                                                   recordingActUID,
                                                                   fields);

        return new SingleObjectModel(this.Request, recordingAct);
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
          throw Assertion.AssertNoReachThisCode($"Unrecognized recordable subject type {subjectType}.");
      }
    }

    #endregion Helper methods

  }  // class RegistrationController

}  //namespace Empiria.Land.Registration.WebApi
