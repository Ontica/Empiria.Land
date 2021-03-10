/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Recordable Subjects                          Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : RecordableSubjectsController                 License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Web API with methods that return recordable subjects related data.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.RecordableSubjects.Adapters;
using Empiria.Land.RecordableSubjects.UseCases;

namespace Empiria.Land.RecordableSubjects.WebApi {

  /// <summary>Web API with methods that return recordable subjects related data.</summary>
  public class RecordableSubjectsController : WebApiController {

    #region Web Apis


    [HttpGet]
    [Route("v5/land/registration/real-estate-kinds")]
    public CollectionModel RealEstateKinds() {

      using (var usecases = RecordableSubjectsUseCases.UseCaseInteractor()) {
        FixedList<string> realEstateKinds = usecases.RealEstateKinds();

        return new CollectionModel(this.Request, realEstateKinds);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/recorder-offices")]
    public CollectionModel RecorderOfficesList() {

      using (var usecases = RecordableSubjectsUseCases.UseCaseInteractor()) {
        FixedList<RecorderOfficeDto> recorderOffices = usecases.RecorderOffices();

        return new CollectionModel(this.Request, recorderOffices);
      }
    }

    #endregion Web Apis

  }  // class RecordableSubjectsController

}  //namespace Empiria.Land.RecordableSubjects.WebApi
