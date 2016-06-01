/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.Land.WebApi                              Assembly : Empiria.Land.WebApi.dll           *
*  Type      : RecordingBooksController                         Pattern  : Web API                           *
*  Version   : 2.1                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Web API used to retrive physical recording books and recordings information.                  *
*                                                                                                            *
********************************* Copyright (c) 2014-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections;
using System.Web.Http;

using Empiria.WebApi;
using Empiria.WebApi.Models;

using Empiria.Land.Registration;

namespace Empiria.Land.WebApi {

  /// <summary>Web API used to retrive physical recording books and recordings information.</summary>
  public class RecordingBooksController : WebApiController {

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/books/{recordingBookId}")]
    public PagedCollectionModel GetRecordingBook(int recordingBookId) {
      throw new NotImplementedException();
    }

    [HttpGet, AllowAnonymous]
    [Route("v1/books/sections/{sectionId}/recording-offices/{recordingOfficeId}")]
    public PagedCollectionModel GetRecordingBooks(int sectionId, int recordingOfficeId) {
      try {
        base.RequireResource(sectionId, "sectionId");
        base.RequireResource(recordingOfficeId, "recordingOfficeId");

        return new PagedCollectionModel(base.Request,
                                        this.GetRecordingBooksList(sectionId, recordingOfficeId),
                                        "Empiria.Land.RecordingBooksList");
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet, AllowAnonymous]
    [Route("v1/books/sections")]
    public CollectionModel GetRecordingSections() {
      try {
        return new CollectionModel(base.Request, this.GetSectionsList(),
                                   "Empiria.Land.RecordingSectionList");
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet, AllowAnonymous]
    [Route("v1/books/sections/{sectionId}")]
    public SingleObjectModel GetRecordingSection(int sectionId) {
      try {
        base.RequireResource(sectionId, "sectionId");

        return new SingleObjectModel(this.Request, this.GetSection(sectionId),
                                     "Empiria.Land.RecordingSection");
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

    #region Private methods

    private ArrayList GetRecordingBooksList(int sectionId, int recordingOfficeId) {
      var section = RecordingSection.Parse(sectionId);
      var recorderOffice = RecorderOffice.Parse(recordingOfficeId);

      FixedList<RecordingBook> books = section.GetRecordingBooks(recorderOffice);

      ArrayList array = new ArrayList(books.Count);

      foreach (RecordingBook book in books) {
        var item = new {
          id = book.Id,
          type = book.Status == RecordingBookStatus.Closed ? "Incorporación" : "Resumen",
          number = book.BookNumber,
          name = book.AsText,
          startRecordingIndex = book.StartRecordingIndex,
          endRecordingIndex = book.EndRecordingIndex,
          section = new {
            id = book.RecordingSection.Id,
            name = book.RecordingSection.Name,
          },
          recorderOffice = new {
            id = book.RecorderOffice.Id,
            name = book.RecorderOffice.Alias,
          },
        };
        array.Add(item);
      }
      return array;
    }

    private object GetSection(int sectionId) {
      var section = RecordingSection.Parse(sectionId);

      FixedList<RecorderOffice> recorderOfficesList = section.GetRecorderOffices();

      ArrayList recorderOffices = new ArrayList(recorderOfficesList.Count);
      foreach (RecorderOffice recorderOffice in recorderOfficesList) {
        var item = new {
          id = recorderOffice.Id,
          name = recorderOffice.Alias,
        };
        recorderOffices.Add(item);
      }

      return new {
        id = section.Id,
        name = section.Name,
        recorderOffices = recorderOffices,
      };
    }

    private ArrayList GetSectionsList() {
      var recordingSections = RecordingSection.GetList();

      ArrayList array = new ArrayList(recordingSections.Count);
      foreach (RecordingSection section in recordingSections) {
        var item = new {
          id = section.Id,
          name = section.Name,
        };
       array.Add(item);
      }
      return array;
    }

    #endregion Private methods

  }  // class RecordingBooksController

}  // namespace Empiria.Land.WebApi
