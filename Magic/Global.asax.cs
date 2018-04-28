using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Magic
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling =
                Newtonsoft.Json.PreserveReferencesHandling.None;
            
            GlobalConfiguration.Configure(WebApiConfig.Register);

        }
    }
}