﻿using System;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;

using Empiria.WebApi;

namespace Empiria.Land.WebApi {

  static public class WebApiConfig {

    #region Public methods

    static public void Register(HttpConfiguration config) {
      // To enable CORS
      var cors = new EnableCorsAttribute("*", "*", "*");
      config.EnableCors(cors);

      //config.SuppressHostPrincipal();

      // To enable attribute routing.
      config.MapHttpAttributeRoutes();

      // To configure convention-based routing.
      WebApiConfig.RegisterWebApiRoutes(config);
    }

    #endregion Public methods

    #region Private methods

    static private void RegisterHttp404ErrorHandlerRoute(HttpRouteCollection routes) {
      routes.MapHttpRoute(
        name: "Error404Handler",
        routeTemplate: "{*url}",
        defaults: new {
          controller = "Security", action = "Http404ErrorHandler"
        }
      );
    }

    static private void RegisterWebApiRoutes(HttpConfiguration config) {
      WebApiConfig.RegisterHttp404ErrorHandlerRoute(config.Routes);
    }

    #endregion Private methods

  }  // class WebApiConfig

  public class WebApiApplication : WebApiGlobal {

    protected override void Application_Start(object sender, EventArgs e) {
      base.Application_Start(sender, e);
      RegisterGlobalHandlers(GlobalConfiguration.Configuration);
      GlobalConfiguration.Configure(WebApiConfig.Register);
      RegisterFormatters(GlobalConfiguration.Configuration);
      RegisterGlobalFilters(GlobalConfiguration.Configuration);
    }

    private void RegisterGlobalHandlers(HttpConfiguration config) {
      config.MessageHandlers.Add(new AuditTrailHandler());
      config.Services.Replace(typeof(IExceptionHandler), new WebApiExceptionHandler());
      config.MessageHandlers.Add(new WebApiResponseHandler());
    }

    protected void Application_BeginRequest(object sender, EventArgs e) {

    }

    private void RegisterFormatters(HttpConfiguration config) {
      var settings = new Newtonsoft.Json.JsonSerializerSettings();

      settings.Formatting = Newtonsoft.Json.Formatting.Indented;
      settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

      // Add System.Data.DataView serializer
      settings.Converters.Add(new Empiria.Json.DataViewConverter());
      settings.Converters.Add(new Empiria.Json.DataRowConverter());

      config.Formatters.JsonFormatter.SerializerSettings = settings;

      // Remove Xml formatter
      config.Formatters.Remove(config.Formatters.XmlFormatter);
    }

    static public void RegisterGlobalFilters(HttpConfiguration config) {
      // Denies anonymous access to every controller without the AllowAnonymous attribute
      if (!ExecutionServer.IsPassThroughServer) {
        config.Filters.Add(new AuthorizeAttribute());
      }
    }

  }  // class WebApiApplication

}  // namespace Empiria.Land.WebApi
