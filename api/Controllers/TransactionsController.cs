using System;
using System.Web.Http;

using Empiria.Data;
using Empiria.DataTypes;
using Empiria.Json;

using Empiria.WebApi;
using Empiria.WebApi.Models;

namespace Empiria.Land.WebApi {

  public class TransactionsController : WebApiController {

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/transactions/{transactionUID}")]
    public SingleObjectModel GetTransaction(string transactionUID) {
      try {
        base.RequireResource(transactionUID, "transactionUID");

        string sql = "SELECT * FROM vwLRSTransactionForWS WHERE TransactionKey = '" + transactionUID + "'";

        var data = DataReader.GetDataTable(DataOperation.Parse(sql));

        if (data != null) {
          return new SingleObjectModel(this.Request, data, "Empiria.Land.Transaction");
        } else {
          throw new ResourceNotFoundException("Transaction.UID",
                      String.Format("Transaction with identifier '{0}' was not found.", transactionUID));
        }

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

  }  // class TransactionsController

}  // namespace Empiria.Land.WebApi
