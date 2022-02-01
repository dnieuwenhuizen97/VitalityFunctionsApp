using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Security.Interfaces;
using Domains.Enums;

namespace Security
{
    public class RequestValidator : IRequestValidator
    {
        public async Task<HttpResponseData> ValidateRequest(HttpRequestData Req, FunctionContext ExecutionContext, string RoleType, Func<ClaimsPrincipal, Task<HttpResponseData>> Delegate)
        {
            try
            {
                ClaimsPrincipal User = GetJwtData(ExecutionContext, RoleType);
                HttpResponseData Response = Req.CreateResponse(HttpStatusCode.Forbidden);

                if ((User.IsInRole(UserType.User.ToString()) && RoleType == UserType.Admin.ToString()))
                {
                    return Response;
                }

                if (User.IsInRole(UserType.User.ToString()) || User.IsInRole(UserType.Admin.ToString()))
                {
                    try
                    {
                        return await Delegate(User).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Response = Req.CreateResponse(HttpStatusCode.BadRequest);
                        return Response;
                    }
                }
                return Response;
            }
            catch (Exception e)
            {
                HttpResponseData Response = Req.CreateResponse(HttpStatusCode.Unauthorized);
                return Response;
            }
        }

        public ClaimsPrincipal GetJwtData(FunctionContext executionContext, string roleType)
        {
            return executionContext.GetUser(roleType);
        }
    }
}
