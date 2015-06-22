using System;
using System.Web.Http;

using Empiria.Data;
using Empiria.DataTypes;
using Empiria.Json;

using Empiria.WebApi;
using Empiria.WebApi.Models;

namespace Empiria.Land.WebApi {

  public class PropertyController : WebApiController {

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/properties/{cadastralKey}")]
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

  }  // class PropertyController

}  // namespace Empiria.Land.WebApi
