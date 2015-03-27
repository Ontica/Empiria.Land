using System;
using System.Web.Http;

using Empiria.DataTypes;
using Empiria.WebServices;

using Empiria.Data;
using Empiria.Json;
using Empiria.WebAPI;

namespace Empiria.Land.WebAPI {

  public class PropertyController : WebApiController {

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v0/properties/{cadastralKey}")]
    public object GetPropertyWithCadastralKey(string cadastralKey) {
      cadastralKey = EmpiriaString.TrimAll(cadastralKey);
      //base.AssertHeader("");
      base.AssertValue(cadastralKey, "cadastralKey");

      string sql = "SELECT * FROM vwLRSCadastralWS WHERE CadastralKey = '{0}'";

      return DataReader.GetDataTable(DataOperation.Parse(String.Format(sql, cadastralKey)));
    }

    #endregion Public APIs

  }  // class PropertyController

}  // namespace Empiria.Land.WebAPI
