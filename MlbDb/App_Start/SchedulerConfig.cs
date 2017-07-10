using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using NLog;

namespace MlbDb
{
    public class SchedulerConfig
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Register(HttpConfiguration config)
        {
            
        }
    }
}