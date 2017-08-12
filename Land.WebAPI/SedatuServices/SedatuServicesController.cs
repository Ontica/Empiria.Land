/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.Land.WebApi.SedatuServices               Assembly : Empiria.Land.WebApi.dll           *
*  Type      : SedatuServicesController                         Pattern  : Web API                           *
*  Version   : 4.0                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Web services integration with the National Services Platform of Land Recording Data.          *
*                                                                                                            *
********************************* Copyright (c) 2014-2017. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Collections;
using System.Web.Http;

using Empiria.WebApi;
using Empiria.WebApi.Models;

using Empiria.Land.Registration;

namespace Empiria.Land.WebApi.SedatuServices {

  /// <summary>Web services integration with the National Services Platform of Land Recording Data.</summary>
  [Empiria.WebApi.Formatting.PascalCaseJsonFormatting(true)]
  public class SedatuServicesController : WebApiController {

    #region Public APIs

    [HttpGet]
    [AllowAnonymous]
    [Route("v1/sedatu/folios-reales/{propertyUID}")]
    public SingleObjectModel GetPropertyByUID(string propertyUID) {
      try {
        base.RequireResource(propertyUID, "propertyUID");

        var realEstate = Resource.TryParseWithUID(propertyUID);

        if (realEstate == null) {
          throw this.RealEstateNotFound(propertyUID);
        }

        var lastDomainAct = ((RealEstate) realEstate).LastDomainAct;
        if (lastDomainAct == null) {
          throw this.RealEstateDomainActsNotFound(propertyUID);
        }

        return new SingleObjectModel(this.Request, BuildResponse(lastDomainAct));
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("v1/sedatu/propietarios")]
    public CollectionModel GetOwners([FromUri] PartiesFilterModel partiesFilter) {
      try {
        Assertion.AssertObject(partiesFilter, "partiesFilter");

        partiesFilter.AssertValid();

        string partiesKeywords = partiesFilter.GetAsKeywords();

        var parties = SearchService.PrimaryParties(partiesKeywords);

        return new CollectionModel(this.Request, BuildResponse(parties),
                                   typeof(SedatuRealEstateModel).FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

    #region Response methods

    private object BuildResponse(RecordingAct recordingAct) {
      return new SedatuRealEstateModel(recordingAct);
    }

    private ICollection BuildResponse(FixedList<RecordingActParty> list) {
      ArrayList array = new ArrayList(list.Count);

      foreach (var party in list) {
        var item = new SedatuRealEstateModel(party);
        array.Add(item);
      }
      return array;
    }

    #endregion Response methods

    #region Private methods

    private Exception RealEstateNotFound(string propertyUID) {
      return new ResourceNotFoundException("RealEstate.UniqueID",
              $"No tengo registrado ningún predio con folio real '{propertyUID}'.");
    }

    private Exception RealEstateDomainActsNotFound(string propertyUID) {
      return new ResourceNotFoundException("RealEstate.NoRecordingActs",
              $"Tengo registrado un predio con folio real '{propertyUID}' " +
              $"pero éste aún no tiene registrados actos de dominio.");
    }

    #endregion Private methods

  }  // class SedatuServicesController

}  // namespace Empiria.Land.WebApi.SedatuServices
