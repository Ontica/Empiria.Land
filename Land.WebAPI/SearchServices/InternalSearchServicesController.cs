/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Query controller                      *
*  Type     : InternalSearchServicesController             License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Query web api with search services for the internal use of Land Recording Offices.             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.SearchServices.UseCases;

namespace Empiria.Land.SearchServices.WebApi {

  /// <summary>Query web api with search services for the internal use of Land Recording Offices.</summary>
  public class InternalSearchServicesController : WebApiController {

    #region Web Apis

    [HttpPost]
    [Route("v5/land/internal-search-services/recordable-subjects")]
    public CollectionModel SearchRecordableSubjects([FromBody] RecordableSubjectsQuery query) {

      base.RequireBody(query);

      using (var usecases = RecordableSubjectsSearchUseCases.UseCaseInteractor()) {
        FixedList<RecordableSubjectQueryResultDto> list = usecases.SearchForInternalUse(query);

        return new CollectionModel(this.Request, list);
      }
    }

    #endregion Web Apis

  }  // class InternalSearchServicesController

}  //namespace Empiria.Land.SearchServices.WebApi
