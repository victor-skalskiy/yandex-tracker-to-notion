using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YandexTrackerToNotion.Interfaces;

namespace YandexTrackerToNotion.Extentions
{
    public class TokenAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IEnvOptions>();

            var expectedToken = options.YandexTrackerAuthorizationToken;
            var requestToken = context.HttpContext.Request.Headers["Authorization"].ToString();

            if ((string.IsNullOrEmpty(requestToken) || !expectedToken.Equals(requestToken)) && !options.DisableAuth)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}