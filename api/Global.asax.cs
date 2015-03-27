using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

using Empiria.WebServices;

namespace Empiria.Land.WebAPI {

  static public class WebApiConfig {

    #region Public methods

    static public void Register(HttpConfiguration config) {
      // To enable attribute routing.
      config.MapHttpAttributeRoutes();
      // To configure convention-based routing.
      RegisterWebApiRoutes(config);
    }

    static void RegisterWebApiRoutes(HttpConfiguration config) {
      //string baseApiURL = "api/v0";

     // RegisterDefaultRestRoutes(config, "Security", baseApiURL + "/security");
      //RegisterDefaultRestRoutes(config, "Member", baseApiURL + "/plans");
      //RegisterDefaultRestRoutes(config, "Ordering", baseApiURL + "/ordering");
      //RegisterDefaultRestRoutes(config, "Orders", baseApiURL + "/ordering/orders");
      //RegisterDefaultRestRoutes(config, "CurrentOrder", baseApiURL + "/ordering/orders/current");
    }

    #endregion Public methods

  }

  public class WebApiApplication : WebApiGlobal {

    protected override void Application_Start(object sender, EventArgs e) {
      base.Application_Start(sender, e);
      GlobalConfiguration.Configure(WebApiConfig.Register);
      RegisterFormatters(GlobalConfiguration.Configuration);
      RegisterGlobalFilters(GlobalConfiguration.Configuration);
    }

    protected void Application_BeginRequest(object sender, EventArgs e) {

    }

    private void RegisterFormatters(HttpConfiguration config) {
      var jsonFormatter = config.Formatters.JsonFormatter;
      jsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

      config.Formatters.Remove(config.Formatters.XmlFormatter);
    }

    static public void RegisterGlobalFilters(HttpConfiguration config) {
      // Denies anonymous access to every controller without the AllowAnonymous attribute
      config.Filters.Add(new AuthorizeAttribute());
      //config.Filters.Add(new System.Web.Mvc.RequireHttpsAttribute());
    }

  }  // class WebApiApplication

}  // namespace Empiria.Land.WebAPI
