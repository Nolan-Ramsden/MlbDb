using NLog;
using System.Web.Http.Filters;

namespace MlbDb.Filters
{
    public class ErrorLoggerAttribute : ExceptionFilterAttribute
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void OnException(HttpActionExecutedContext context)
        {
            Logger.Error("Unhandled exception from {0} {1}, {2}",
                context.Request.Method,
                context.ActionContext.RequestContext.RouteData.Route.RouteTemplate,
                context.Exception);
        }
    }
}