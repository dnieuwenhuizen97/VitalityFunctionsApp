using Domains;
using Domains.DTO;
using Domains.Enums;
using HttpMultipartParser;
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
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using VitalityApp.Authentication;

namespace InhollandVitalityApp.Controllers
{
    public class ChallengeHttpTrigger
    {
        IRequestValidator RequestValidator { get; }
        IChallengeService _challengeService { get; set; }
        IBlobStorageService BlobStorageService { get; }
        ILogger Logger;

        public ChallengeHttpTrigger(IRequestValidator requestValidator, IChallengeService service, IBlobStorageService blobStorageService, ILogger<ChallengeHttpTrigger> logger)
        {
            this.RequestValidator = requestValidator;
            this._challengeService = service;
            this.BlobStorageService = blobStorageService;
            this.Logger = logger;
        }

        [Function(nameof(CreateChallenge))]
        [OpenApiOperation(operationId: "createChallenge", tags: new[] { "challenge" }, Summary = "Create a new challenge (admin)", Description = "This will add a new challenge to the database (admin only)", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ChallengeCreationRequest), Required = true, Description = "Challenge object that needs to be added to the database")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ChallengeCreationRequest), Summary = "New challenge created", Description = "New challenge created", Example = typeof(ChallengeCreationRequestExample))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid input", Description = "Invalid input")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> CreateChallenge([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "challenge")] HttpRequestData req, FunctionContext executionContext)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.Admin.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                ChallengeCreationRequest challenge = JsonConvert.DeserializeObject<ChallengeCreationRequest>(await new StreamReader(req.Body).ReadToEndAsync());
                try
                {
                    var result = await _challengeService.CreateChallenge(currentUser.FindFirst(ClaimTypes.Sid).Value, challenge);
                    await response.WriteAsJsonAsync(result);
                }
                catch (Exception e)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    await response.WriteAsJsonAsync(e.Message);
                }


                return response;
            });
        }

        [Function(nameof(ChallengeHttpTrigger.GetChallenge))]
        [OpenApiOperation(operationId: "getChallenge", tags: new[] { "challenge" }, Summary = "Find challenge by ID", Description = "Returns a single challenge", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "id of challenge to return", Description = "id of challenge to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ChallengeDTO), Summary = "Successful operation", Description = "Successful operation", Example = typeof(ChallengeExample))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Challenge not found", Description = "Challenge not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> GetChallenge([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "challenge/{challengeId}")] HttpRequestData req, string challengeId, FunctionContext executionContext)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                ChallengeDTO challenge = new ChallengeDTO();

                try
                {
                    challenge = await _challengeService.GetChallenge(challengeId, currentUser.FindFirst(ClaimTypes.Sid).Value);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(challenge);

                return response;
            });
        }

        [Function(nameof(ChallengeHttpTrigger.GetAllChallenges))]
        [OpenApiOperation(operationId: "getAllChallenges", tags: new[] { "challenge" }, Summary = "Find challenges grouped by parameters", Description = "Returns a list of challenges grouped by parameters, if no parameters given return all challenges", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeType", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "The type of challenge to filter by: [Exercise(1), Diet(2), Mind(3)]", Description = "Provide a challenge type to filter by: [Exercise(1), Diet(2), Mind(3)]", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The limit of determining the pagination", Description = "The limit of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The offset of determining the pagination", Description = "The offset of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "progress", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "The progress type to filter by: [NotSubscribed(0), InProgress(1), Done(2), Cancelled(3)]", Description = "The progress type to filter by: [NotSubscribed(0), InProgress(1), Done(2), Cancelled(3)]", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "The user id to look by, if nothing is entered, the id of the current user is used", Description = "The user id to look by, if nothing is entered, the id of the current user is used", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ChallengeDTO>), Summary = "Successful operation", Description = "Successful operation", Example = typeof(ChallengeDTOExamples))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid parameters are supplied", Description = "Invalid parameters supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "No challenges were found", Description = "No challenges found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> GetAllChallenges([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "challenge")] HttpRequestData req, FunctionContext executionContext, string challengeType, int limit, int offset, string progress, string userId)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                List<ChallengeDTO> challenges = new List<ChallengeDTO>();
                Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(req.Url.Query);
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

                if (userId == null)
                {
                    userId = currentUser.FindFirst(ClaimTypes.Sid).Value;
                }

                try
                {
                    if (challengeType != null && progress == null)
                    {
                        challenges = await _challengeService.GetChallengesGroupedByType((ChallengeType)int.Parse(challengeType), limit, offset, userId);

                    }
                    else if (challengeType == null && progress != null)
                    {
                        challenges = await _challengeService.GetChallengesGroupedByProgress((ChallengeProgress)int.Parse(progress), limit, offset, userId);
                    }
                    else if (challengeType != null && progress != null)
                    {
                        challenges = await _challengeService.GetChallengesGroupedByTypeAndProgress((ChallengeProgress)int.Parse(progress), (ChallengeType)int.Parse(challengeType), limit, offset, userId);
                    }
                    else
                    {
                        challenges = await _challengeService.GetAllChallenges(limit, offset, userId);
                    }

                    await response.WriteAsJsonAsync(challenges);
                }
                catch (Exception e)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                }


                return response;
            });
        }

        [Function(nameof(ChallengeHttpTrigger.GetSubscribedUsers))]
        [OpenApiOperation(operationId: "getSubscribedUsers", tags: new[] { "challenge" }, Summary = "Find all user subscribed to a specific challenge", Description = "Returns a list of user id's", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "id of challenge to find subscribers", Description = "id of challenge to find subscribers", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The limit of determining the pagination", Description = "The limit of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The offset of determining the pagination", Description = "The offset of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<string>), Summary = "Successful operation", Description = "Successful operation", Example = typeof(SubscribedUsersDTOExamples))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid challengeId supplied", Description = "Invalid challengeId supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Challenge not found", Description = "Challenge not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> GetSubscribedUsers([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "challenge/{challengeId}/subscribers")] HttpRequestData req, FunctionContext executionContext, string challengeId, int limit, int offset)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                List<SubscribedUsersDTO> subscribedUsers = new List<SubscribedUsersDTO>();
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                try
                {
                    subscribedUsers = await _challengeService.GetChallengeSubscribers(challengeId, limit, offset);
                }
                catch (Exception)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    return response;
                }

                await response.WriteAsJsonAsync(subscribedUsers);

                return response;
            });
        }

        [Function(nameof(UpdateChallenge))]
        [OpenApiOperation(operationId: "updateChallenge", tags: new[] { "challenge" }, Summary = "Update an existing challenge (admin)", Description = "This will update an existing challenge's information (admin only)", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "id of challenge to update", Description = "id of challenge to update", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ChallengeUpdatePropertiesDTO), Required = true, Description = "Challenge object that needs to be updated in the database")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ChallengeDTO), Summary = "Challenge updated", Description = "Challenge updated", Example = typeof(ChallengeDTOExample))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid input", Description = "Invalid input")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> UpdateChallenge([HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "challenge/{challengeId}")] HttpRequestData req, FunctionContext executionContext, string challengeId)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.Admin.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                ChallengeUpdatePropertiesDTO challenge = JsonConvert.DeserializeObject<ChallengeUpdatePropertiesDTO>(await new StreamReader(req.Body).ReadToEndAsync());

                ChallengeDTO updatedChallenge = new ChallengeDTO();
                try
                {
                    updatedChallenge = await _challengeService.UpdateChallenge(challenge, challengeId);
                }
                catch (Exception)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    return response;
                }

                await response.WriteAsJsonAsync(updatedChallenge);

                return response;
            });
        }

        [Function(nameof(RegisterToChallenge))]
        [OpenApiOperation(operationId: "registerToChallenge", tags: new[] { "challenge" }, Summary = "Subscribe the current user to a challenge", Description = "This will subscribe the current user to a challenge", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of challenge to subscribe to", Description = "Id of challenge to subscribe to", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Summary = "User has been subscribed to the challenge", Description = "User has been subscribed to the challenge")]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid challengeId supplied", Description = "Invalid challengeId supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Challenge not found", Description = "Challenge not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> RegisterToChallenge([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "challenge/{challengeId}/subscribe")] HttpRequestData req, string challengeId, FunctionContext executionContext)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

                try
                {
                    string currentUserId = currentUser.FindFirst(ClaimTypes.Sid).Value;
                    await _challengeService.RegisterToChallenge(challengeId, currentUserId);
                    await response.WriteAsJsonAsync("Successfully registered to challenge");
                }
                catch (Exception e)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    return response;
                }


                return response;
            });
        }

        [Function(nameof(UpdateChallengeProgress))]
        [OpenApiOperation(operationId: "updateChallengeProgress", tags: new[] { "challenge" }, Summary = "Update a user's challenge progress", Description = "Update the challenge progress of the logged in user", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the challenge which progress needs to be updated", Description = "Id of the challenge which progress needs to be updated", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeProgress", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The progress type to set the current challenge to: [InProgress(1), Done(2), Cancelled(3)]", Description = "The progress type to set the current challenge to: [InProgress(1), Done(2), Cancelled(3)]", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "The challenge progress has been updated", Description = "The challenge progress has been updated")]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid challengeId supplied", Description = "Invalid challengeId supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Challenge not found", Description = "Challenge not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> UpdateChallengeProgress([HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "challenge/{challengeId}/progress")] HttpRequestData req, FunctionContext executionContext, string challengeId, int challengeProgress)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                string currentUserId = currentUser.FindFirst(ClaimTypes.Sid).Value;

                string updatedProgress;

                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                try
                {
                    updatedProgress = await _challengeService.UpdateChallengeProgress(challengeId, (ChallengeProgress)challengeProgress, currentUserId);
                }
                catch (Exception)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    return response;
                }

                await response.WriteAsJsonAsync(updatedProgress);

                return response;
            });
        }

        [Function(nameof(UpdateChallengeImage))]
        [OpenApiOperation(operationId: "updateChallengeImage", tags: new[] { "challenge" }, Summary = "Upload a (new) image for a challenge", Description = "Upload a (new) image for a challenge", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the challenge which image needs to be updated", Description = "Id of the challenge which image needs to be updated", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "multipart/form-data", bodyType: typeof(MediaTypeNames.Image), Required = true, Description = "Image to upload.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "Successful operation", Description = "Successful operation")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> UpdateChallengeImage([HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "challenge/{challengeId}/image")] HttpRequestData req, FunctionContext executionContext, string challengeId)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.Admin.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);
                string currentUserId = currentUser.FindFirst(ClaimTypes.Sid).Value;

                var parsedFromBody = await MultipartFormDataParser.ParseAsync(req.Body);
                List<StreamContentDTO> files = new List<StreamContentDTO>();

                files.AddRange(parsedFromBody.Files.Select(x =>
                    new StreamContentDTO
                    {
                        Name = x.Name,
                        FileName = x.FileName,
                        Data = x.Data,
                        ContentType = x.ContentType
                    }));

                StreamContentDTO image = null;
                if (files.Count > 0)
                {
                    image = files[0];
                }

                if (!image.FileName.EndsWith(".jpg") && !image.FileName.EndsWith(".jpeg") && !image.FileName.EndsWith(".png"))
                {
                    return response;
                }

                var result = await BlobStorageService.UploadImage($"ChallengePic:{challengeId}", image.Data);

                await _challengeService.UpdateChallengeImage(challengeId);

                response = req.CreateResponse(HttpStatusCode.OK);
                return response;
            });
        }

        [Function(nameof(UpdateChallengeVideo))]
        [OpenApiOperation(operationId: "updateChallengeVideo", tags: new[] { "challenge" }, Summary = "Upload a (new) video for a challenge", Description = "Upload a (new) video for a challenge", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the challenge which video needs to be updated", Description = "Id of the challenge which image needs to be updated", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "multipart/form-data", bodyType: typeof(MediaTypeNames.Image), Required = true, Description = "video to upload.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "Successful operation", Description = "Successful operation")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> UpdateChallengeVideo([HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "challenge/{challengeId}/video")] HttpRequestData req, FunctionContext executionContext, string challengeId)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.Admin.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                string currentUserId = currentUser.FindFirst(ClaimTypes.Sid).Value;
                HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);

                var parsedFromBody = await MultipartFormDataParser.ParseAsync(req.Body);
                List<StreamContentDTO> files = new List<StreamContentDTO>();

                files.AddRange(parsedFromBody.Files.Select(x =>
                    new StreamContentDTO
                    {
                        Name = x.Name,
                        FileName = x.FileName,
                        Data = x.Data,
                        ContentType = x.ContentType
                    }));

                StreamContentDTO video = null;
                if (files.Count > 0)
                {
                    video = files[0];
                }

                if (!video.FileName.EndsWith(".mp4") && !video.FileName.EndsWith(".mov") && !video.FileName.EndsWith(".wmv") && !video.FileName.EndsWith(".avi"))
                {
                    return response;
                }

                var result = await BlobStorageService.UploadVideo($"ChallengeVid:{challengeId}", video.Data);

                await _challengeService.UpdateChallengeVideo(challengeId);

                response = req.CreateResponse(HttpStatusCode.OK);
                return response;
            });
        }

        [Function(nameof(DeleteChallenge))]
        [OpenApiOperation(operationId: "deleteChallenge", tags: new[] { "challenge" }, Summary = "Delete a challenge (admin)", Description = "This will remove a challenge from the database (admin only)", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "challengeId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "id of challenge to remove", Description = "id of challenge to remove", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ChallengeDTO), Summary = "The challenge has been deleted", Description = "The challenge has been deleted")]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid challengeId supplied", Description = "Invalid challengeId supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Challenge not found", Description = "Challenge not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> DeleteChallenge([HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "challenge/{challengeId}")] HttpRequestData req, FunctionContext executionContext, string challengeId)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.Admin.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                ChallengeDTO deletedChallenge = new ChallengeDTO();

                try
                {
                    deletedChallenge = await _challengeService.DeleteChallenge(challengeId);
                }
                catch (Exception)
                {
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                    return response;
                }

                await response.WriteAsJsonAsync(deletedChallenge);

                return response;
            });
        }

    }
}
