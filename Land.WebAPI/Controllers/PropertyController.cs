using System;
using System.Web.Http;

using Empiria.Data;
using Empiria.WebApi;
using Empiria.WebApi.Models;

using Empiria.Land.Registration;

namespace Empiria.Land.WebApi {

  public class PropertyController : WebApiController {

    #region Public APIs

    [HttpGet]
    [Route("v1/properties/{propertyUID}")]
    public SingleObjectModel GetProperty(string propertyUID) {
      try {
        base.RequireResource(propertyUID, "propertyUID");

        string sql = "SELECT * FROM LRSProperties WHERE PropertyUID = '{0}'";

        var data = DataReader.GetDataRow(DataOperation.Parse(String.Format(sql, propertyUID)));

        if (data != null) {
          return new SingleObjectModel(this.Request, data, "Empiria.Land.Property");
        } else {
          throw new ResourceNotFoundException("Property.UniqueID",
                        String.Format("Property with unique ID '{0}' was not found.", propertyUID));
        }

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    /// <summary>Gets textual information about a registered property given its unique ID.</summary>
    [HttpGet]
    [Route("v1/properties/{propertyUID}/as-html")]
    public SingleObjectModel GetPropertyTextInfo([FromUri] string propertyUID) {
      try {
        base.RequireResource(propertyUID, "propertyUID");

        var property = Property.TryParseWithUID(propertyUID);

        if (property != null) {
          return new SingleObjectModel(this.Request, this.GetPropertyAsTextModel(property),
                                       "Empiria.Land.PropertyAsHtml");
        } else {
          throw new ResourceNotFoundException("Property.UniqueID",
                                              "Property with unique ID '{0}' was not found.", propertyUID);
        }
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet, AllowAnonymous]
    [Route("v1/properties/cadastral/{cadastralKey}")]
    public SingleObjectModel GetPropertyWithCadastralKey(string cadastralKey) {
      try {
        base.RequireResource(cadastralKey, "cadastralKey");

        string sql = "SELECT * FROM vwLRSCadastralWS WHERE CadastralKey = '{0}'";

        var data = DataReader.GetDataRow(DataOperation.Parse(String.Format(sql, cadastralKey)));

        if (data != null) {
          return new SingleObjectModel(this.Request, data, "Empiria.Land.Property");
        } else {
          throw new ResourceNotFoundException("Property.CadastralKey",
                        String.Format("Property with cadastral key '{0}' was not found.", cadastralKey));
        }

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

    #region Private methods

    private object GetPropertyAsTextModel(Property o) {
      return new {
        uid = o.UID,
        asHtml = o.AsText,
      };
    }

    #endregion Private methods

  }  // class PropertyController

}  // namespace Empiria.Land.WebApi
