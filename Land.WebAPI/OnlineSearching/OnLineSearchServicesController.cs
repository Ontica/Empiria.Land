/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Module   : Search services                              Component : Web Api                               *
*  Assembly : Empiria.Land.WebApi.dll                      Pattern   : Controller                            *
*  Type     : OnLineSearchServicesController               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Contains general web methods for the Empiria Land Online Search Services system.               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

using Empiria.WebApi;

namespace Empiria.Land.WebApi {

  /// <summary>Contains general web methods forthe Empiria Land Online Search Services system.</summary>
  public class OnLineSearchServicesController : WebApiController {

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/certificates/{certificateUID}")]
    public CollectionModel GetCertificate([FromUri] string certificateUID,
                                          [FromUri] string hash = "") {
      try {
        certificateUID = FormatParameter(certificateUID);
        hash = FormatParameter(hash);

        var validator = new ResourceNotFoundValidator();

        validator.ValidateCertificate(certificateUID, hash);

        var propertiesBagBuilder = new OnLineSearchPropertiesBagBuilder();

        List<PropertyBagItem> propertiesBag = propertiesBagBuilder.BuildCertificate(certificateUID, hash);

        return new CollectionModel(this.Request, propertiesBag, "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/documents/{landRecordUID}")]
    public CollectionModel GetDocument([FromUri] string landRecordUID,
                                       [FromUri] string hash = "") {
      try {
        landRecordUID = FormatParameter(landRecordUID);
        hash = FormatParameter(hash);

        var validator = new ResourceNotFoundValidator();

        validator.ValidateLandRecord(landRecordUID, hash);

        var propertiesBagBuilder = new OnLineSearchPropertiesBagBuilder();

        List<PropertyBagItem> propertiesBag = propertiesBagBuilder.BuildLandRecord(landRecordUID, hash);

        return new CollectionModel(this.Request, propertiesBag, "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/resources/{resourceUID}")]
    public CollectionModel GetResource([FromUri] string resourceUID,
                                       [FromUri] string hash = "") {
      try {
        resourceUID = FormatParameter(resourceUID);
        hash = FormatParameter(hash);

        var validator = new ResourceNotFoundValidator();

        validator.ValidateResource(resourceUID, hash);

        var propertiesBagBuilder = new OnLineSearchPropertiesBagBuilder();

        List<PropertyBagItem> propertiesBag = propertiesBagBuilder.BuildResourceStatus(resourceUID);

        return new CollectionModel(this.Request, propertiesBag, "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpGet, AllowAnonymous]
    [Route("v1/online-services/transactions/{transactionUID}")]
    public CollectionModel GetTransaction([FromUri] string transactionUID,
                                          [FromUri] string hash = "",
                                          [FromUri] string messageUID = "") {
      try {

        transactionUID = FormatParameter(transactionUID);
        hash = FormatParameter(hash);
        messageUID = FormatParameter(messageUID);

        var validator = new ResourceNotFoundValidator();

        validator.ValidateTransaction(transactionUID, hash);

        var propertiesBagBuilder = new OnLineSearchPropertiesBagBuilder();

        List<PropertyBagItem> propertiesBag = propertiesBagBuilder.BuildTransaction(transactionUID, messageUID);

        return new CollectionModel(this.Request, propertiesBag, "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }


    [HttpPost, AllowAnonymous]
    [Route("v1/online-services/transactions/{transactionUID}/electronic-delivery")]
    public async Task<CollectionModel> ElectronicDelivery([FromUri] string transactionUID,
                                                          [FromUri] string hash = "",
                                                          [FromUri] string messageUID = "") {
      try {
        if (IsPassThroughServer) {
          var apiClient = new OnLineSearchServicesClient();

          var data = await apiClient.ElectronicDelivery(this.Request);

          return new CollectionModel(this.Request, data, "Empiria.PropertyBag");
        }

        var validator = new ResourceNotFoundValidator();

        transactionUID = FormatParameter(transactionUID);
        hash = FormatParameter(hash);
        messageUID = FormatParameter(messageUID);

        validator.ValidateTransaction(transactionUID, hash);

        validator.ValidateTransactionForElectronicDelivery(transactionUID, messageUID);

        var eDeliveryService = new ElectronicDeliveryService();

        eDeliveryService.DeliverTransaction(transactionUID, messageUID);

        var propertiesBagBuilder = new OnLineSearchPropertiesBagBuilder();

        List<PropertyBagItem> propertiesBag = propertiesBagBuilder.BuildTransaction(transactionUID, messageUID);

        return new CollectionModel(this.Request, propertiesBag, "Empiria.PropertyBag");

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

    #region Helpers

    private string FormatParameter(string parameter) {
      return EmpiriaString.TrimSpacesAndControl(parameter);
    }

    #endregion Helpers

  }  // class OnLineSearchServicesController

}  // namespace Empiria.Land.WebApi
