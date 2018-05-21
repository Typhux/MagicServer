﻿using System.Web.Http;

namespace Magic
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            // Remove the XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.Add(config.Formatters.JsonFormatter);
            
        }
    }
}
