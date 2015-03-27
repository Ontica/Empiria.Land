using System;
using System.Web.Http;
using System.Web.Routing;

namespace Empiria.WebAPI {

  static public class WebApiConfig {

    #region Public methods

    static public void Register(HttpConfiguration config) {
      // To enable attribute routing.
      config.MapHttpAttributeRoutes();
      // To configure convention-based routing.
      RegisterWebApiRoutes(config);
    }

    static void RegisterWebApiRoutes(HttpConfiguration config) {
      string baseApiURL = "api/v0";

      //RegisterDefaultRestRoutes(config, "Security", baseApiURL + "/security");
      //RegisterDefaultRestRoutes(config, "Member", baseApiURL + "/plans");
      //RegisterDefaultRestRoutes(config, "Ordering", baseApiURL + "/ordering");
      //RegisterDefaultRestRoutes(config, "Orders", baseApiURL + "/ordering/orders");
      //RegisterDefaultRestRoutes(config, "CurrentOrder", baseApiURL + "/ordering/orders/current");
    }

    #endregion Public methods

    #region Private methods

    static private void RegisterDefaultRestRoutes(HttpConfiguration config, string controllerName,
                                                  string rootAndResourcePath) {

      // [GET|POST] /api/ordering/products
      config.Routes.MapHttpRoute(
        name: "Default." + controllerName + ".Web.API.Resource",
        /*   /api/{controller} */
        routeTemplate: rootAndResourcePath,
        defaults: new {
          controller = controllerName
        },
        constraints: new {
          httpMethod = new HttpMethodConstraint("GET", "POST")
        }
      );

      // [GET|PUT|DELETE] /api/ordering/orders/1
      config.Routes.MapHttpRoute(
        name: "Default." + controllerName + ".Web.API.OneObject",
        /*   /api/{controller}/{id} */
        routeTemplate: rootAndResourcePath + "/{id}",
        defaults: new {
          controller = controllerName
        },
        constraints: new {
          id = @"\d+|'\S*'",
          httpMethod = new HttpMethodConstraint("GET", "PUT", "DELETE")
        }
      );

      // [GET|PUT|DELETE] /api/ordering/orders(1)
      config.Routes.MapHttpRoute(
        name: "Default." + controllerName + ".Web.API.OneObject.OData-Style",
        /*   /api/{controller}({id}) || /api/{controller}('{key}') */
        routeTemplate: rootAndResourcePath + "({id})",
        defaults: new {
          controller = controllerName
        },
        constraints: new {
          id = @"\d+|'\S*'",
          httpMethod = new HttpMethodConstraint("GET", "PUT", "DELETE")
        }
      );

      // [GET|POST] /api/ordering/orders/print, /api/ordering/orders/count
      config.Routes.MapHttpRoute(
        name: "Default." + controllerName + ".Web.API.Action",
        /* /api/{controller}/{action} */
        routeTemplate: rootAndResourcePath + "/{action}",
        defaults: new {
          controller = controllerName
        },
        constraints: new {
          action = @"[A-Za-z]+",
          httpMethod = new HttpMethodConstraint("GET", "POST")
        }
      );

      // [GET|POST] /api/{controller}/{id}/{action}
      config.Routes.MapHttpRoute(
        name: "Default." + controllerName + ".Web.API.OneObject.Action",
        /* /api/ordering/orders/12345/print || /api/ordering/orders/'K456-09'/print  */
        routeTemplate: rootAndResourcePath + "/{id}/{action}",
        defaults: new {
          controller = controllerName
        },
        constraints: new {
          action = @"[A-Za-z]+", id = @"\d+|'\S*'",
          httpMethod = new HttpMethodConstraint("GET", "POST")
        }
      );

      // [GET|POST] /api/{controller}/{id}/{action}
      config.Routes.MapHttpRoute(
        name: "Default." + controllerName + ".Web.API.OneObject.Action.ODataStyle",
        /*  /api/ordering/orders(12345)/print || /api/ordering/orders/'K456-09'/print  */
        routeTemplate: rootAndResourcePath + "({id})/{action}",
        defaults: new {
          controller = controllerName
        },
        constraints: new {
          action = @"[A-Za-z]+", id = @"\d+|'\S*'",
          httpMethod = new HttpMethodConstraint("GET", "POST")
        }
      );

    }  // RegisterDefaultRestRoutes

    #endregion Private methods

  }  // class WebApiConfig

}  // namespace Empiria.WebAPI
