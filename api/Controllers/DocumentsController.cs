using System;
using System.Web.Http;

using Empiria.Data;

using Empiria.WebApi;
using Empiria.WebApi.Models;

namespace Empiria.Land.WebApi {

  public class DocumentsController : WebApiController {

    #region Public APIs

    [HttpGet, AllowAnonymous]
    [Route("v1/documents/{documentUID}")]
    public SingleObjectModel GetDocument(string documentUID) {
      try {
        base.RequireResource(documentUID, "documentUID");

        string sql = "SELECT * FROM vwLRSCadastralWS WHERE CadastralKey = '{0}'";

        var data = DataReader.GetDataRow(DataOperation.Parse(String.Format(sql, documentUID)));

        if (data != null) {
          return new SingleObjectModel(this.Request, data, "Empiria.Land.Property");
        } else {
          throw new ResourceNotFoundException("Document.UID",
                      String.Format("Document with identifier '{0}' was not found.", documentUID));
        }

      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

  }  // class DocumentsController

}  // namespace Empiria.Land.WebApi
