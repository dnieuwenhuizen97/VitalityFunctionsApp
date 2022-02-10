using Domains;
using Domains.DTO;
using Domains.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Security;
using Security.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using VitalityApp.Authentication;

namespace VitalityFunctionsApp.Controllers
{
    public class NotificationHttpTrigger
    {
        public INotificationService _pushTokenService { get; set; }
        public IRequestValidator _requestValidator { get; set; }
        public NotificationHttpTrigger(INotificationService service, IRequestValidator requestValidator)
        {
            _pushTokenService = service;
            _requestValidator = requestValidator;
        }


        [Function(nameof(NotificationHttpTrigger.CreatePushToken))]
        [OpenApiOperation(operationId: "createPushToken", tags: new[] { "notification" }, Summary = "Create a new pushToken for user", Description = "Create a new pushToken for user", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "deviceType", In = ParameterLocation.Query, Required = true, Type = typeof(DeviceType), Summary = "The Device we want to create a pushtoken for. [Android, iOS]", Description = "The Device we want to create a pushtoken for. [Android, iOS]", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PushToken), Required = true, Description = "The pushToken to send to the devices of a user", Example = typeof(PushTokenExample))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PushToken), Summary = "Successful operation", Description = "Successful operation", Example = typeof(PushTokenExample))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Page not found", Description = "Page not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> CreatePushToken([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "notification")] HttpRequestData req, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                PushTokenDTO pushtoken = new PushTokenDTO();
                try
                {
                    Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(req.Url.Query);
                    int deviceTypeInt = int.Parse(queryParams["deviceType"]);
                    DeviceType type = (DeviceType)deviceTypeInt;
                    pushtoken = await _pushTokenService.CreatePushToken(currentUser.FindFirst(ClaimTypes.Sid).Value, type);

                }
                catch (Exception)
                {
                    throw;
                }

                await response.WriteAsJsonAsync(pushtoken);
                return response;
            });
        }

        [Function(nameof(NotificationHttpTrigger.UpdatePushToken))]
        [OpenApiOperation(operationId: "updatePushToken", tags: new[] { "notification" }, Summary = "Updates if notifications are turned on or off", Description = "This will update an existing pushToken's information", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "IsTurnedOn", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Summary = "IsTurnedOn = true will send notifications to user. IsTurnedOn = false will not send notification to user.", Description = "IsTurnedOn = true will send notifications to user. IsTurnedOn = false will not send notification to user.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PushTokenDTO), Summary = "Successful operation", Description = "Successful operation", Example = typeof(PushTokenExample))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "PushToken not found", Description = "PushToken not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> UpdatePushToken([HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "notification")] HttpRequestData req, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(req.Url.Query);
                string IsTurnedOn = queryParams["IsTurnedOn"];
                List<PushTokenDTO> pushtokens = new List<PushTokenDTO>();

                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                try
                {
                    bool isTurnedOn = bool.Parse(IsTurnedOn);
                    pushtokens = await _pushTokenService.UpdatePushToken(currentUser.FindFirst(ClaimTypes.Sid).Value, isTurnedOn);
                }
                catch (Exception)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    return response;
                }

                await response.WriteAsJsonAsync(pushtokens);

                return response;
            });
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
                    notifications = await _pushTokenService.GetNotifications(currentUser.FindFirst(ClaimTypes.Sid).Value, limit, offset);
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
                    success = await _pushTokenService.DeletePushToken(currentUser.FindFirst(ClaimTypes.Sid).Value, pushTokenId);
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
