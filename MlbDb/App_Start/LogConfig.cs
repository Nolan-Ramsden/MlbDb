using DotConf;
using MlbDb.Scrapers;
using MlbDb.Storage;
using NLog;
using NLog.Targets;
using System;
using System.Web.Http;

namespace MlbDb
{
    public static class LogConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var fileConfig = new AutoConfigField<FileTarget>(
                key: "logging.file.settings",
                description: "Logger settings for logging to a file",
                defaultVal: new FileTarget()
                {
                    Name = "file",
                    Layout = @"${longdate}|${logger:shortName=true}|${level:uppercase=true}|${message}",
                    FileName = @"C:\Users\Nolan\Desktop\${shortdate}.txt",
                }
            );
            var fileLevel = new AutoConfigField<string>(
                key: "logging.file.level",
                description: "What level the file logger should log at",
                defaultVal: "Info"
            );
            
            // Setup logging
            var logConfig = new NLog.Config.LoggingConfiguration();
            
            // File logging
            if (!string.Equals("off", fileLevel, StringComparison.InvariantCultureIgnoreCase))
            {
                logConfig.AddTarget(fileConfig);
                logConfig.AddRule(LogLevel.FromString(fileLevel), LogLevel.Off, fileConfig.Value.Name);
            }

            LogManager.Configuration = logConfig;
        }
    }
}
