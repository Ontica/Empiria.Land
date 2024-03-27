/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Land Registration                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RecordingBookRegistrationController          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API used to retrive transaction workflow data and invoke commands on it.                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
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

        RecorderOffice recorderOffice = GetRecorderOffice();

        FixedList<NamedEntityDto> sections = usecases.GetRecordingSections(recorderOffice);

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
        FixedList<BookEntryShortDto> bookEntries = usecases.GetRecordingBookEntries(recordingBookUID);

        return new CollectionModel(this.Request, bookEntries);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/recording-books/{recordingBookUID:guid}/book-entries")]
    public SingleObjectModel CreateBookEntry([FromUri] string recordingBookUID,
                                             [FromBody] ManualEditBookEntryFields fields) {

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        RecordingBookDto recordingBook = usecases.CreateBookEntry(recordingBookUID, fields);

        return new SingleObjectModel(this.Request, recordingBook);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/{landRecordUID:guid}/book-entries/create-next-book-entry")]
    public SingleObjectModel CreateNextBookEntry([FromUri] string landRecordUID,
                                                 [FromBody] CreateNextBookEntryFields fields) {
      base.RequireBody(fields);

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        LandRecordDto landRecord = usecases.CreateNextBookEntry(landRecordUID, fields);

        return new SingleObjectModel(this.Request, landRecord);
      }
    }


    [HttpPost]
    [Route("v5/land/registration/recording-books/{recordingBookUID:guid}/" +
           "book-entries/{bookEntryUID:guid}/recording-acts")]
    public SingleObjectModel CreateBookEntryRecordingAct([FromUri] string recordingBookUID,
                                                         [FromUri] string bookEntryUID,
                                                         [FromBody] RegistrationCommand command) {
      base.RequireBody(command);

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        LandRecordDto landRecord = usecases.CreateRecordingAct(recordingBookUID, bookEntryUID, command);

        return new SingleObjectModel(this.Request, landRecord);
      }
    }


    [HttpDelete]
    [Route("v5/land/registration/recording-books/{recordingBookUID:guid}/" +
           "book-entries/{bookEntryUID:guid}/recording-acts/{recordingActUID:guid}")]
    public SingleObjectModel DeleteBookEntryRecordingAct([FromUri] string recordingBookUID,
                                                         [FromUri] string bookEntryUID,
                                                         [FromUri] string recordingActUID) {

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        LandRecordDto landRecord = usecases.RemoveRecordingAct(recordingBookUID, bookEntryUID, recordingActUID);

        return new SingleObjectModel(this.Request, landRecord);
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
    [Route("v5/land/registration/{landRecordUID:guid}/book-entries/{bookEntryUID:guid}")]
    public SingleObjectModel RemoveBookEntryFromLandRecord([FromUri] string landRecordUID,
                                                           [FromUri] string bookEntryUID) {

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        LandRecordDto landRecord = usecases.RemoveBookEntryFromLandRecord(landRecordUID, bookEntryUID);

        return new SingleObjectModel(this.Request, landRecord);
      }
    }


    [HttpPut, HttpPatch]
    [Route("v5/land/registration/{landRecordUID:guid}/book-entries/{bookEntryUID:guid}/update-instrument")]
    public SingleObjectModel UpdateBookEntryInstrument([FromUri] string landRecordUID,
                                                       [FromUri] string bookEntryUID,
                                                       [FromBody] ManualEditBookEntryFields fields) {

      base.RequireBody(fields);

      using (var usecases = RecordingBookRegistrationUseCases.UseCaseInteractor()) {
        LandRecordDto landRecord = usecases.UpdateBookEntryInstrument(landRecordUID, bookEntryUID, fields);

        return new SingleObjectModel(this.Request, landRecord);
      }
    }


    #endregion Web Apis


    private RecorderOffice GetRecorderOffice() {
      try {
        return Permissions.GetUserDefaultRecorderOffice();

      } catch {
        return RecorderOffice.Empty;
      }
    }

  }  // class RecordingBookRegistrationController

}  //namespace Empiria.Land.Registration.WebApi
