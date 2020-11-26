/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Extranet Services                            Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : ExtranetPropertyController                   License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Public API to retrieve properties (real estate).                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Web.Http;

using Empiria.WebApi;

using Empiria.Land.Registration;

namespace Empiria.Land.WebApi.Extranet {

  /// <summary>Public API to retrieve properties (real estate).</summary>
  public class ExtranetPropertyController : WebApiController {

    #region Public methods

    [HttpGet]
    [Route("v2/extranet/properties/{propertyUID}")]
    public SingleObjectModel GetRealEstate([FromUri] string propertyUID) {
      try {
        base.RequireResource(propertyUID, "propertyUID");

        var realEstate = RealEstate.TryParseWithUID(propertyUID, true);

        if (realEstate == null) {
          throw new ResourceNotFoundException("Property.UID",
                                              $"No tenemos registrado ningún predio con folio real {propertyUID}.");
        }

        return new SingleObjectModel(this.Request, realEstate.ToResponse(),
                                     realEstate.GetType().FullName);

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public methods

  }  // class ExtranetPropertyController

}  //namespace Empiria.Land.WebApi.Extranet
