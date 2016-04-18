/* Empiria Land **********************************************************************************************
*                                                                                                            *
*  Solution  : Empiria Land                                     System   : Land Web API                      *
*  Namespace : Empiria.Land.WebApi                              Assembly : Empiria.Land.WebApi.dll           *
*  Type      : PropertyController                               Pattern  : Web API                           *
*  Version   : 2.1                                              License  : Please read license.txt file      *
*                                                                                                            *
*  Summary   : Contains services used to get real estate and other resources data.                           *
*                                                                                                            *
********************************* Copyright (c) 2014-2016. La Vía Óntica SC, Ontica LLC and contributors.  **/
using System;
using System.Linq;
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
          throw this.PropertyNotFound(propertyUID);
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

        var property = RealEstate.TryParseWithUID(propertyUID);

        if (property != null) {
          return new SingleObjectModel(this.Request, this.GetPropertyAsTextModel(property),
                                       "Empiria.Land.PropertyAsHtml");
        } else {
          throw this.PropertyNotFound(propertyUID);
        }
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    [HttpGet]
    [Route("v1/properties/{propertyUID}/antecedent")]
    public SingleObjectModel GetPropertyAntecedent(string propertyUID) {
      try {
        base.RequireResource(propertyUID, "propertyUID");

        var property = RealEstate.TryParseWithUID(propertyUID);

        if (property == null) {
          throw this.PropertyNotFound(propertyUID);
        }

        var domainAntecedent = property.GetDomainAntecedent();
        var provisionalAntecedent = property.GetProvisionalDomainAct();
        var fullTract = property.GetRecordingActsTractUntil(RecordingAct.Empty, false);

        var data = new {
          domain = this.GetRecordingActModel(domainAntecedent),
          provisional = this.GetRecordingActModel(provisionalAntecedent),
          fullTract = fullTract.Select((x) => this.GetRecordingActModel(x)),
        };

        return new SingleObjectModel(this.Request, data, "Empiria.Land.PropertyAntecedents");
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    private object GetRecordingActModel(RecordingAct act) {
      return new {
        id = act.Id,
        typeId = act.RecordingActType.Id,
        type = act.RecordingActType.DisplayName,
      };
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
                "No tengo registrado ningún predio con la clave catastral '{0}'.", cadastralKey);
        }
      } catch (Exception e) {
        throw base.CreateHttpException(e);
      }
    }

    #endregion Public APIs

    #region Private methods

    private object GetPropertyAsTextModel(RealEstate o) {
      return new {
        uid = o.UID,
        asHtml = "El folio electrónico del predio es <strong>" + o.UID  + "</strong>.<br/><br/>Este es el texto que debería " +
                 "desplegarse en el <i>editor de trámites</i> CITyS para confirmar que se trata del mismo predio.",
      };
    }

    private Exception PropertyNotFound(string propertyUID) {
      return new ResourceNotFoundException("Property.UniqueID",
          "No tengo registrado ningún predio con el folio real '{0}'.", propertyUID);
    }

    #endregion Private methods

  }  // class PropertyController

}  // namespace Empiria.Land.WebApi
