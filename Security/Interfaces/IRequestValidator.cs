using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Security.Interfaces
{
    public interface IRequestValidator
    {
        Task<HttpResponseData> ValidateRequest(HttpRequestData Req, FunctionContext ExecutionContext, string RoleType, Func<ClaimsPrincipal, Task<HttpResponseData>> Delegate);
        ClaimsPrincipal GetJwtData(FunctionContext executionContext, string roleType);
    }
}
