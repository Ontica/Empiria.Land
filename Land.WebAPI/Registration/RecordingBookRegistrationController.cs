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


    [HttpGet]
    [Route("v5/land/registration/recorder-offices/{recorderOfficeUID:guid}/" +
           "recording-sections/{recordingSectionUID}/recording-books")]
    public CollectionModel GetRecordingBooksList([FromUri] string recorderOfficeUID,
                                                 [FromUri] string recordingSectionUID,
                                                 [FromUri] string keywords = "") {

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> recordingBooks = usecases.GetRecordingBooksList(recorderOfficeUID,
                                                                                  recordingSectionUID,
                                                                                  keywords);

        return new CollectionModel(this.Request, recordingBooks);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/recording-books/{recordingBookUID:guid}")]
    public SingleObjectModel GetRecordingBook([FromUri] string recordingBookUID) {

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        RecordingBookDto recordingBook = usecases.GetRecordingBook(recordingBookUID);

        return new SingleObjectModel(this.Request, recordingBook);
      }
    }



    [HttpGet]
    [Route("v5/land/registration/recording-books/instrument-types")]
    public CollectionModel GetInstrumentTypesForRecordingBooks() {

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> instrumentTypes = usecases.InstrumentTypesForRecordingBooks();

        return new CollectionModel(this.Request, instrumentTypes);
      }
    }



    [HttpGet]
    [Route("v5/land/registration/recording-books/{recordingBookUID:guid}/book-entries")]
    public CollectionModel GetRecordingBookEntries([FromUri] string recordingBookUID) {

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        FixedList<RecordingBookEntryShortDto> bookEntries = usecases.GetRecordingBookEntries(recordingBookUID);

        return new CollectionModel(this.Request, bookEntries);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/recording-books/{recordingBookUID:guid}/book-entries")]
    public SingleObjectModel CreateBookEntry([FromUri] string recordingBookUID,
                                             [FromBody] CreateManualBookEntryFields fields) {

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        RecordingBookDto recordingBook = usecases.CreateBookEntry(recordingBookUID, fields);

        return new SingleObjectModel(this.Request, recordingBook);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/book-entries/create-next-book-entry")]
    public SingleObjectModel CreateNextBookEntry([FromUri] string instrumentRecordingUID,
                                                 [FromBody] CreateNextBookEntryFields fields) {
      base.RequireBody(fields);

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording =
                                     usecases.CreateNextBookEntry(instrumentRecordingUID, fields);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/recording-books/{recordingBookUID:guid}/book-entries/{bookEntryUID:guid}")]
    public SingleObjectModel RemoveBookEntry([FromUri] string recordingBookUID,
                                             [FromUri] string bookEntryUID) {
      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        RecordingBookDto recordingBook = usecases.RemoveBookEntry(recordingBookUID, bookEntryUID);

        return new SingleObjectModel(this.Request, recordingBook);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/{instrumentRecordingUID:guid}/book-entries/{bookEntryUID:guid}")]
    public SingleObjectModel RemoveInstrumentBookEntry([FromUri] string instrumentRecordingUID,
                                                       [FromUri] string bookEntryUID) {
      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        InstrumentRecordingDto instrumentRecording =
                                      usecases.RemoveBookEntryFromInstrument(instrumentRecordingUID, bookEntryUID);

        return new SingleObjectModel(this.Request, instrumentRecording);
      }
    }

    #endregion Web Apis

  }  // class RecordingBookRegistrationController

}  //namespace Empiria.Land.Registration.WebApi
