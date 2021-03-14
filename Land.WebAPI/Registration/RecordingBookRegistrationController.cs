/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RecordingBookRegistrationController          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive transaction workflow data and invoke commands on it.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Registration.Adapters;
using Empiria.Land.Registration.UseCases;

namespace Empiria.Land.Registration.WebApi {

  /// <summary>Web API used to retrive transaction workflow data and invoke commands on it.</summary>
  public class RecordingBookRegistrationController : WebApiController {

    #region Web Apis

    [HttpGet]
    [Route("v5/land/recording-sections")]
    public CollectionModel GetRecordingSectionsList() {

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> sections = usecases.GetRecordingSections();

        return new CollectionModel(this.Request, sections);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/book-entries/create-next-book-entry")]
    public SingleObjectModel CreateNextBookEntry([FromUri] string instrumentRecordingUID,
                                                 [FromBody] RecordingBookEntryFields fields) {
      base.RequireBody(fields);

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording =
                                     usecases.CreateNextBookEntry(instrumentRecordingUID, fields);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/book-entries/{bookEntryUID:guid}")]
    public SingleObjectModel RemovePhysicalRecording([FromUri] string instrumentRecordingUID,
                                                     [FromUri] string bookEntryUID) {
      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording =
                              usecases.RemoveBookEntry(instrumentRecordingUID, bookEntryUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }

    #endregion Web Apis

  }  // class RecordingBookRegistrationController

}  //namespace Empiria.Land.Registration.WebApi
