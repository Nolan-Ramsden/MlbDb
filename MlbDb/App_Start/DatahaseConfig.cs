using System.Web.Http;

namespace MlbDb
{
    public static class DatahaseConfig
    {
        public static void Register(HttpConfiguration config)
        {
            new Storage.MlbDatabase().Dispose();
        }
    }
}