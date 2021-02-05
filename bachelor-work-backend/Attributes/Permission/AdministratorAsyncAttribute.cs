using bachelor_work_backend.Database;
using bachelor_work_backend.Services.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace bachelor_work_backend.Filters.Permission
{
    public class AdministratorAsyncAttribute : IAsyncActionFilter
    {

        private readonly BachContext context;
        public IConfiguration Configuration { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }

        public AdministratorAsyncAttribute(BachContext context, IConfiguration configuration,
            IHttpClientFactory clientFactory)
        {
            this.context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claims = context.HttpContext.User.Claims;

            //var stagApiService = new StagApiService(Configuration, ClientFactory);
            //context.Result = new Bad

            //context.HttpContext.Request.Headers.TryGetValue("Authorization", out authorizationToken);

          //  context.Result = new ForbidResult("invalid-permission");
        }
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    var svc = filterContext.HttpContext.RequestServices;
        //}
    }
}
