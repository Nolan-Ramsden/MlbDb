using NLog;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;

namespace MlbDb.Scrapers
{
    public class WebRequester : IDisposable
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected HttpClient Client = new HttpClient();

        public async Task<T> RequestJson<T>(string url, IDictionary<string, string> queryArgs = null)
        {
            string fullUrl = BuildUrl(url, queryArgs);
            Logger.Debug("Requesting JSON from {0}", fullUrl);
            try
            {
                var data = await Client.GetAsync(fullUrl);
                if (!data.IsSuccessStatusCode)
                {
                    Logger.Warn("Error downloading JSON from {0}, {1}", fullUrl, data.ReasonPhrase);
                    return default(T);
                }
                return JsonConvert.DeserializeObject<T>(await data.Content.ReadAsStringAsync());
            } catch (Exception e)
            {
                Logger.Warn("Error downloading JSON from {0}, {1}", fullUrl, e.Message);
                throw;
            }
        }

        public async Task<T> RequestXml<T>(string url, IDictionary<string, string> queryArgs = null) where T : class
        {
            string fullUrl = BuildUrl(url, queryArgs);
            Logger.Debug("Requesting XML from {0}", fullUrl);
            try
            {
                var data = await Client.GetAsync(fullUrl);
                if (!data.IsSuccessStatusCode)
                {
                    Logger.Warn("Error downloading XML from {0}, {1}", fullUrl, data.ReasonPhrase);
                    return default(T);
                }
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(await data.Content.ReadAsStringAsync());
                var jsonStr = JsonConvert.SerializeXmlNode(xml);
                return JsonConvert.DeserializeObject<T>(jsonStr);
            }
            catch (Exception e)
            {
                Logger.Warn("Error downloading XML from {0}, {1}", fullUrl, e.Message);
                throw;
            }
        }

        protected static string BuildUrl(string url, IDictionary<string, string> queryArgs)
        {
            NameValueCollection query = System.Web.HttpUtility.ParseQueryString("");
            if (queryArgs != null)
            {
                foreach (var val in queryArgs)
                {
                    query[val.Key] = val.Value;
                }
            }
            return $"{url}?{query.ToString()}";
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}