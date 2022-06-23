using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Puzzle.Compound.Common.Models;
using Puzzle.Compound.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Compound.Authorization
{
    public class ApiAuthorizationFilter : ActionFilterAttribute
    {
        public ApiAuthorizationFilter(string actionName)
        {
            this.actionName = actionName;
        }

        public string actionName;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();
            var userIdentity = context.HttpContext.RequestServices.GetService<UserIdentity>();

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
            }

            var hasAccess = Task.Run(() => authorizationService.Validate(userIdentity.CompanyId.ToString(),
                                                                         userIdentity.Id.ToString(), 
                                                                         actionName)).Result;

            if (hasAccess)
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new ForbidResult();
            }
            
        }
    }
}
