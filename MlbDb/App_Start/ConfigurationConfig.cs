using DotConf;
using MlbDb.Storage;
using System;
using System.IO;
using System.Web;
using System.Web.Http;

namespace MlbDb
{
    public static class ConfigurationConfig
    {
        public static void Register(HttpConfiguration config)
        {
            Conf.Global
                .LoadAppSettings()
                .LoadJsonFile(Path.Combine(HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data"), "config.json"));
        }
    }
}
