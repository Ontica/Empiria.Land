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
    [Route("v5/land/registration/association-kinds")]
    public CollectionModel GetAssociationKinds() {

      using (var usecases = RecordableSubjectsUseCases.UseCaseInteractor()) {
        FixedList<string> associationKinds = usecases.AssociationKinds();

        return new CollectionModel(this.Request, associationKinds);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/no-property-kinds")]
    public CollectionModel GetNoPropertyKinds() {

      using (var usecases = RecordableSubjectsUseCases.UseCaseInteractor()) {
        FixedList<string> noPropertyKinds = usecases.NoPropertyKinds();

        return new CollectionModel(this.Request, noPropertyKinds);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/real-estate-kinds")]
    public CollectionModel GetRealEstateKinds() {

      using (var usecases = RecordableSubjectsUseCases.UseCaseInteractor()) {
        FixedList<string> realEstateKinds = usecases.RealEstateKinds();

        return new CollectionModel(this.Request, realEstateKinds);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/real-estate-partition-kinds")]
    public CollectionModel GetRealEstatePartitionKinds() {

      using (var usecases = RecordableSubjectsUseCases.UseCaseInteractor()) {
        FixedList<string> realEstateKinds = usecases.RealEstatePartitionKinds();

        return new CollectionModel(this.Request, realEstateKinds);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/real-estate-lot-size-units")]
    public CollectionModel GetRealEstateLotSizeUnits() {

      using (var usecases = RecordableSubjectsUseCases.UseCaseInteractor()) {
        FixedList<NamedEntityDto> lotSizeUnits = usecases.RealEstateLotSizeUnits();

        return new CollectionModel(this.Request, lotSizeUnits);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/recorder-offices")]
    public CollectionModel GetRecorderOfficesList() {

      using (var usecases = RecordableSubjectsUseCases.UseCaseInteractor()) {
        FixedList<RecorderOfficeDto> recorderOffices = usecases.RecorderOffices();

        return new CollectionModel(this.Request, recorderOffices);
      }
    }


    [HttpGet]
    [Route("v5/land/registration/recordable-subjects")]
    public CollectionModel SearchRecordableSubjects([FromUri] SearchRecordableSubjectsCommand searchCommand) {

      Assertion.AssertObject(searchCommand, "searchCommand");

      using (var usecases = RecordableSubjectsUseCases.UseCaseInteractor()) {
        FixedList<RecordableSubjectShortDto> list = usecases.SearchRecordableSubjects(searchCommand);

        return new CollectionModel(this.Request, list);
      }
    }

    #endregion Web Apis

  }  // class RecordableSubjectsController

}  //namespace Empiria.Land.RecordableSubjects.WebApi
