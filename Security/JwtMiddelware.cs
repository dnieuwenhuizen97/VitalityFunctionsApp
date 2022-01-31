using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Security
{
    public class JwtMiddleware : IFunctionsWorkerMiddleware
    {
        IJwtTokenService JwtTokenService { get; }
        ILogger Logger { get; }

        public JwtMiddleware(IJwtTokenService jwtTokenService, ILogger<JwtMiddleware> Logger)
        {
            this.JwtTokenService = jwtTokenService;
            this.Logger = Logger;
        }

        public async Task Invoke(FunctionContext Context, FunctionExecutionDelegate Next)
        {
            string HeaderString = (string)Context.BindingContext.BindingData["Headers"];

            Dictionary<string, string> Headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(HeaderString);

            if (Headers.TryGetValue("Authorization", out string AuthorizationHeader))
            {
                try
                {
                    AuthenticationHeaderValue BearerHeader = AuthenticationHeaderValue.Parse(AuthorizationHeader);

                    ClaimsPrincipal User = await JwtTokenService.GetByValue(BearerHeader.Parameter);
                    Context.Items["User"] = User;
                    Context.Items["Admin"] = User;
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }
            }

            await Next(Context);
        }
    }
}
