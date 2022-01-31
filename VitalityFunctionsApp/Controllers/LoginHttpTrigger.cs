using Domains;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Services.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace VitalityFunctionsApp.Controllers
{
    public class LoginHttpTrigger
    {
        IJwtTokenService JwtTokenService { get; }
        ILogger Logger;
        IEmailValidationService ValidationService { get; }
        IInputSanitizationService InputSanitizationService { get; }

        public LoginHttpTrigger(IJwtTokenService JwtTokenService, ILogger<LoginHttpTrigger> Logger, IEmailValidationService ValidationService, IInputSanitizationService InputSanitizationService)
        {
            this.JwtTokenService = JwtTokenService;
            this.Logger = Logger;
            this.ValidationService = ValidationService;
            this.InputSanitizationService = InputSanitizationService;
        }

        [Function(nameof(Login))]
        [OpenApiOperation(operationId: "login", tags: new[] { "login" }, Summary = "Login for a user", Description = "Logs in a user, and returns a JWT bearer token")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(LoginRequest), Required = true, Description = "The user credentials")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginResult), Description = "Login successful")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid credentials", Description = "Invalid credentials")]
        public async Task<HttpResponseData> Login([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "login")] HttpRequestData req, FunctionContext executionContext)
        {
            LoginRequest login = JsonConvert.DeserializeObject<LoginRequest>(await new StreamReader(req.Body).ReadToEndAsync());
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            try
            {
                login.Email = await InputSanitizationService.SanitizeInput(login.Email);
                LoginResult result = await JwtTokenService.LoginUser(login);
                await response.WriteAsJsonAsync(result);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                response = req.CreateResponse(HttpStatusCode.Unauthorized);
                //await response.WriteAsJsonAsync("Invalid credentials");
            }


            return response;
        }

        [Function(nameof(LoginRefresh))]
        [OpenApiOperation(operationId: "loginRefresh", tags: new[] { "login" }, Summary = "Login for a user", Description = "Re-logs in a user using a refresh token, and returns a JWT bearer token")]
        [OpenApiParameter(name: "refreshToken", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Refresh token of the user", Description = "Refresh token of the user", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginResult), Description = "Login successful")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid credentials", Description = "Invalid credentials")]
        public async Task<HttpResponseData> LoginRefresh([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "login/refresh")] HttpRequestData req, FunctionContext executionContext, string refreshToken)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            try
            {
                LoginResult result = await JwtTokenService.LoginUserByRefresh(refreshToken);
                await response.WriteAsJsonAsync(result);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                response = req.CreateResponse(HttpStatusCode.Unauthorized);
            }


            return response;
        }

    }
}
