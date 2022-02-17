using Domains;
using Domains.DTO;
using Domains.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Security;
using Security.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using VitalityApp.Authentication;

namespace VitalityFunctionsApp.Controllers
{
    public class NotificationHttpTrigger
    {
        private INotificationService _notificationService { get; }
        private IRequestValidator _requestValidator { get; }
        private IUserService _userService { get; }

        private ILogger _logger;

        public NotificationHttpTrigger(INotificationService service, IRequestValidator requestValidator, IUserService userService, ILogger<NotificationHttpTrigger> logger)
        {
            _notificationService = service;
            _requestValidator = requestValidator;
            _userService = userService;
            _logger = logger;
        }

        [Function(nameof(NotificationHttpTrigger.GetNotifications))]
        [OpenApiOperation(operationId: "getNotifications", tags: new[] { "notification" }, Summary = "Get all notifications for user", Description = "Returns a list of notifications", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The limit of determining the pagination", Description = "The limit of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The offset of determining the pagination", Description = "The offset of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<NotificationDTO>), Summary = "Successful operation", Description = "Successful operation", Example = typeof(List<NotificationExample>))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad request", Description = "Bad request")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "No posts found", Description = "No posts found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> GetNotifications([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "notification")] HttpRequestData req, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(req.Url.Query);
                int limit = int.Parse(queryParams["limit"]);
                int offset = int.Parse(queryParams["offset"]);
                List<NotificationDTO> notifications = new List<NotificationDTO>();

                try
                {
                    notifications = await _notificationService.GetNotifications(currentUser.FindFirst(ClaimTypes.Sid).Value, limit, offset);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(notifications);

                return response;
            });
        }

        [Function(nameof(CreatePushToken))]
        [OpenApiOperation(operationId: "createPushToken", tags: new[] { "notification" }, Summary = "Create a new push token for the user", Description = "Create a new push token for the user", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PushTokenCreationRequest), Required = true, Description = "The pushtoken and devicetype that need to be added to the database")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PushTokenDTO), Summary = "New push token registered", Description = "New push token registered", Example = typeof(PushTokenCreationRequestExample))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid input", Description = "Invalid input")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> CreatePushToken([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "notification/pushtoken")] HttpRequestData req, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.Created);
                string currentUserId = currentUser.FindFirst(ClaimTypes.Sid).Value;

                string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
                PushTokenCreationRequest request = JsonConvert.DeserializeObject<PushTokenCreationRequest>(requestbody);


                try
                {
                    PushTokenDTO pushTokenDTO = await _notificationService.CreatePushToken(request, currentUserId);
                    await response.WriteAsJsonAsync(pushTokenDTO);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    response.StatusCode = HttpStatusCode.BadRequest;
                }

                return response;
            });
        }

        [Function(nameof(GetPushToken))]
        [OpenApiOperation(operationId: "getPushToken", tags: new[] { "notification" }, Summary = "Get the user's pushtoken", Description = "Get the user's pushtoken", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "pushToken", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "The pushtoken to retreive the data of", Description = "The pushtoken to retreive the data of", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PushTokenDTO), Summary = "Push token retreived", Description = "Push token retreived", Example = typeof(PushTokenDTO))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid input", Description = "Invalid input")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> GetPushToken([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "notification/pushtoken")] HttpRequestData req, FunctionContext executionContext, string pushToken)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.Created);
                string currentUserId = currentUser.FindFirst(ClaimTypes.Sid).Value;

                try
                {
                    PushTokenDTO pushTokenDTO = await _notificationService.GetPushToken(pushToken, currentUserId);
                    await response.WriteAsJsonAsync(pushTokenDTO);
                }
                catch (KeyNotFoundException knfe)
                {
                    _logger.LogError(knfe.Message);
                    response.StatusCode = HttpStatusCode.NotFound;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    response.StatusCode = HttpStatusCode.BadRequest;
                }

                return response;
            });
        }

        [Function(nameof(SendTestPushNotification))]
        [OpenApiOperation(operationId: "sendTestPushNotification", tags: new[] { "notification" }, Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "pushToken", In = ParameterLocation.Path, Required = true, Type = typeof(string), Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PushTokenDTO), Summary = "Push token retreived", Description = "Push token retreived", Example = typeof(PushTokenDTO))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid input", Description = "Invalid input")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> SendTestPushNotification([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "notification/pushtoken/{pushToken}")] HttpRequestData req, FunctionContext executionContext, string pushToken)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.Created);

            try
            {
                await _notificationService.SendPushNotification(pushToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                response.StatusCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        [Function(nameof(NotificationHttpTrigger.DeletePushToken))]
        [OpenApiOperation(operationId: "deletePushToken", tags: new[] { "notification" }, Summary = "Deletes a pushToken", Description = "Returns true/false if delete was successful", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "pushTokenId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the pushToken to delete", Description = "Id of the pushToken to delete", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Summary = "Successful operation", Description = "Successful operation", Example = typeof(bool))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "PushToken not found", Description = "PushToken not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> DeletePushToken([HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "notification/{pushTokenId}")] HttpRequestData req, string pushTokenId, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                bool success = false;

                try
                {
                    success = await _notificationService.DeletePushToken(currentUser.FindFirst(ClaimTypes.Sid).Value, pushTokenId);
                }
                catch (Exception)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    return response;
                }

                await response.WriteAsJsonAsync(success);

                return response;
            });
        }

    }
}
