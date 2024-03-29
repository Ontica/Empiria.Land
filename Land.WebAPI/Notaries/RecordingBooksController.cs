﻿/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.Land.WebApi                              Assembly : Empiria.Land.WebApi.dll           *
*  Type      : RecordingBooksController                         Pattern  : Web API                           *
*  Version   : 3.0                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Web API used to retrive physical recording books and book entries information.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections;
using System.Web.Http;

using Empiria.Geography;
using Empiria.WebApi;

using Empiria.Land.Registration;


namespace Empiria.Land.WebApi {


  /// <summary>Web API used to retrive physical recording books and book entries information.</summary>
  public class RecordingBooksController : WebApiController {

    #region Public APIs


    [HttpGet, AllowAnonymous]
    [Route("v2/catalogues/real-estate-types")]
    public CollectionModel GetRealEstateTypes() {
      try {
        FixedList<RealEstateType> list = RealEstateType.GetList();

        return new CollectionModel(base.Request, list.ToIdResponse(x => x.Name),
                                   typeof(RealEstateType).FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v2/catalogues/recorder-offices")]
    public CollectionModel GetRecorderOffice() {
      try {
        FixedList<RecorderOffice> list = RecorderOffice.GetList();

        return new CollectionModel(base.Request, list.ToIdentifiableResponse(x => x.ShortName),
                                   typeof(RecorderOffice).FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v2/catalogues/recorder-offices/{recorderOfficeId}/recording-books/{sectionId}")]
    public CollectionModel GetRecorderOfficeSectionRecordingBooksList(int recorderOfficeId, int sectionId) {
      try {
        var recorderOffice = RecorderOffice.Parse(recorderOfficeId);
        var section = RecordingSection.Parse(sectionId);

        FixedList<RecordingBook> list = recorderOffice.GetRecordingBooks(section);

        return new CollectionModel(base.Request, list.ToIdentifiableResponse(x => x.AsText),
                                   typeof(RecordingBook).FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v2/catalogues/recorder-offices/{recorderOfficeId}/municipalities")]
    public CollectionModel GetRecorderOfficeMunicipalityList(int recorderOfficeId) {
      try {
        var recorderOffice = RecorderOffice.Parse(recorderOfficeId);

        FixedList<Municipality> list = recorderOffice.GetMunicipalities();

        return new CollectionModel(base.Request, list.ToIdentifiableResponse(x => x.Name),
                                   typeof(Municipality).FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


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
            name = book.RecorderOffice.ShortName,
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
          name = recorderOffice.ShortName,
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
