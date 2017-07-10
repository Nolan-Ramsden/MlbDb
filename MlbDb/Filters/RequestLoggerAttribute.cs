using NLog;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MlbDb.Filters
{
    public class RequestLoggerAttribute : ActionFilterAttribute
    {
        static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private DateTime requestStart;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            requestStart = DateTime.Now;
            Logger.Trace("{0} {1} - Started", actionContext.Request.Method, actionContext.RequestContext.RouteData.Route.RouteTemplate);
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception == null && actionExecutedContext.Response != null)
            {
                Logger.Debug("{0} {1} {2} [{3} ms]",
                    actionExecutedContext.Request.Method,
                    actionExecutedContext.ActionContext.ControllerContext.RouteData.Route.RouteTemplate,
                    actionExecutedContext.Response.StatusCode,
                    (DateTime.Now.Subtract(requestStart)).TotalMilliseconds
                );
            }
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
