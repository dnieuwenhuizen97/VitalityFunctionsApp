using Domains;
using Domains.DTO;
using Domains.Enums;
using HttpMultipartParser;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Security.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VitalityApp.Authentication;


namespace VitalityFunctionsApp.Controllers
{

    public class TimelinePostHttpTrigger
    {
        public ITimelineService _timelineService { get; set; }
        public IRequestValidator _requestValidator { get; set; }
        public IBlobStorageService _blobStorageService { get; set; }

        public TimelinePostHttpTrigger(ITimelineService service, IRequestValidator requestValidator, IBlobStorageService blobStorageService)
        {
            _timelineService = service;
            _requestValidator = requestValidator;
            _blobStorageService = blobStorageService;
        }

        [Function(nameof(TimelinePostHttpTrigger.CreatePost))]
        [OpenApiOperation(operationId: "createPost", tags: new[] { "timelinepost" }, Summary = "Create a new post", Description = "Create a new post.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "multipart/form-data;", bodyType: typeof(TimelinePostCreationRequest), Required = true, Description = "The text content of the post to create.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TimelinePostDTO), Summary = "Successful operation", Description = "Successful operation", Example = typeof(TimelinePostExample))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Post not found", Description = "Post not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> CreatePost([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "timelinepost")] HttpRequestData req, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                TimelinePostDTO timelinePost = new TimelinePostDTO();

                try
                {
                // Rip the multiformdata into pieces
                MultipartFormDataParser parsedFormBody = await MultipartFormDataParser.ParseAsync(req.Body);
                var parameters = parsedFormBody.Parameters;

                // Retrieve the "Text" from request
                List<ParametersKeys> parametersKeysDTO = parameters.Select(x => new ParametersKeys { Data = x.Data, Text = x.Name }).ToList();
                ParametersKeys text = parametersKeysDTO.FirstOrDefault(x => x.Text.ToLower() == "text");

                // Retrieve images and video from request
                List<StreamContentDTO> files = new List<StreamContentDTO>();

                files.AddRange(parsedFormBody.Files.Select(x =>
                    new StreamContentDTO
                    {
                        Name = x.Name,
                        FileName = x.FileName,
                        Data = x.Data,
                        ContentType = x.ContentType
                    }
                ).ToList());

                // create
                TimelinePostCreationRequest request = new TimelinePostCreationRequest(text.Data, files);
                timelinePost = await _timelineService.CreatePost(request, currentUser.FindFirst(ClaimTypes.Sid).Value);

                }
                catch (Exception ex)
                {
                    throw;
                }

                await response.WriteAsJsonAsync(timelinePost);
                return response;
            });
        }

        [Function(nameof(TimelinePostHttpTrigger.GetTimelinePosts))]
        [OpenApiOperation(operationId: "getTimelinePosts", tags: new[] { "timelinepost" }, Summary = "Get all posts", Description = "Returns a list of posts on the timeline", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The limit of determining the pagination", Description = "The limit of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The offset of determining the pagination", Description = "The offset of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<TimelinePostDTO>), Summary = "Successful operation", Description = "Successful operation", Example = typeof(List<TimelinePostExample>))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad request", Description = "Bad request")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "No posts found", Description = "No posts found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> GetTimelinePosts([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "timelinepost")] HttpRequestData req, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(req.Url.Query);
                int limit = int.Parse(queryParams["limit"]);
                int offset = int.Parse(queryParams["offset"]);
                List<TimelinePostDTO> timelinePosts = new List<TimelinePostDTO>();

                try
                {
                    timelinePosts = await _timelineService.GetTimelinePosts(limit, offset, currentUser.FindFirst(ClaimTypes.Sid).Value);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(timelinePosts);

                return response;
            });
        }

        [Function(nameof(TimelinePostHttpTrigger.GetTimelinePostById))]
        [OpenApiOperation(operationId: "getTimelinePostById", tags: new[] { "timelinepost" }, Summary = "Get a single timeline post", Description = "Returns a single of post on the timeline", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "timelinePostId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "The id of the timeline post to return", Description = "The id of the timeline post to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TimelinePostDTO), Summary = "Successful operation", Description = "Successful operation", Example = typeof(TimelinePostExample))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Bad request", Description = "Bad request")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "No posts found", Description = "No posts found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> GetTimelinePostById([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "timelinepost/{timelinePostId}")] HttpRequestData req, FunctionContext executionContext, string timelinePostId)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                TimelinePostDTO timelinePost = new TimelinePostDTO();

                try
                {
                    timelinePost = await _timelineService.GetTimelinePostById(timelinePostId, currentUser.FindFirst(ClaimTypes.Sid).Value);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(timelinePost);

                return response;
            });
        }

        [Function(nameof(TimelinePostHttpTrigger.DeletePost))]
        [OpenApiOperation(operationId: "deletePost", tags: new[] { "timelinepost" }, Summary = "Deletes a post", Description = "Deletes a post.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "timelinePostId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the timelinepost to delete", Description = "Id of timelinepost to delete", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Summary = "True if deleting was successful", Description = "True if deleting was successful", Example = typeof(bool))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Post not found", Description = "Post not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> DeletePost([HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "timelinepost/{timelinePostId}")] HttpRequestData req, string timelinePostId, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                bool success = false;

                try
                {
                    success = await _timelineService.DeletePost(timelinePostId, currentUser.FindFirst(ClaimTypes.Sid).Value);
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

        [Function(nameof(TimelinePostHttpTrigger.PostComment))]
        [OpenApiOperation(operationId: "postComment", tags: new[] { "timelinepost" }, Summary = "Post a comment on a specific post.", Description = "Post a comment on a specific post.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "timelinePostId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the timelinepost to comment on", Description = "Id of the timelinepost to comment on", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CommentCreationRequest), Required = true, Description = "The comment to post.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(CommentDTO), Summary = "Successful operation", Description = "Successful operation", Example = typeof(CommentExample))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Post not found", Description = "Post not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> PostComment([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "timelinepost/{timelinePostId}/comment")] HttpRequestData req, string timelinePostId, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                CommentDTO comment = new CommentDTO();
                try
                {
                    CommentCreationRequest request = JsonConvert.DeserializeObject<CommentCreationRequest>(await new StreamReader(req.Body).ReadToEndAsync());
                    comment = await _timelineService.PostComment(request, timelinePostId, currentUser.FindFirst(ClaimTypes.Sid).Value);
                }
                catch (Exception)
                {
                    throw;
                }

                await response.WriteAsJsonAsync(comment);
                return response;
            });
        }

        [Function(nameof(TimelinePostHttpTrigger.GetLikersOnPost))]
        [OpenApiOperation(operationId: "getTimeline", tags: new[] { "timelinepost" }, Summary = "Find all the users who likes the post, only returns basic info of user (Fullname, userId and profilepicture)", Description = "Returns the users who liked the post", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The limit of determining the pagination", Description = "The limit of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The offset of determining the pagination", Description = "The offset of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "timelinePostId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the timelinepost to return", Description = "Id of timelinepost to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<LikeExample>), Summary = "Successful operation", Description = "Successful operation", Example = typeof(List<LikeExample>))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Timeline not found", Description = "Timeline not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> GetLikersOnPost([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "timelinepost/{timelinePostId}/likers")] HttpRequestData req, string timelinePostId, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(req.Url.Query);
                int limit = int.Parse(queryParams["limit"]);
                int offset = int.Parse(queryParams["offset"]);
                List<LikeDTO> likersOfPost = new List<LikeDTO>();

                try
                {
                    likersOfPost = await _timelineService.GetLikersOnPost(timelinePostId, limit, offset);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(likersOfPost);

                return response;
            });
        }

        [Function(nameof(TimelinePostHttpTrigger.GetCommentsOnPost))]
        [OpenApiOperation(operationId: "getCommentsOnPost", tags: new[] { "timelinepost" }, Summary = "Find all the comments of the post", Description = "Returns the comments of the post", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The limit of determining the pagination", Description = "The limit of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The offset of determining the pagination", Description = "The offset of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "timelinePostId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the timelinepost to return", Description = "Id of timelinepost to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<CommentDTO>), Summary = "Successful operation", Description = "Successful operation", Example = typeof(List<CommentExample>))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Timeline not found", Description = "Timeline not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> GetCommentsOnPost([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "timelinepost/{timelinePostId}/comments")] HttpRequestData req, string timelinePostId, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(req.Url.Query);
                int limit = int.Parse(queryParams["limit"]);
                int offset = int.Parse(queryParams["offset"]);
                List<CommentOfUserDTO> commentsOnPost = new List<CommentOfUserDTO>();

                try
                {
                    commentsOnPost = await _timelineService.GetCommentsOnPost(timelinePostId, limit, offset);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(commentsOnPost);

                return response;
            });
        }

        [Function(nameof(TimelinePostHttpTrigger.PutLikeOnPost))]
        [OpenApiOperation(operationId: "putLikeOnPost", tags: new[] { "timelinepost" }, Summary = "Puts a like on a post", Description = "Returns true/false if update was successful", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "timelinePostId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the timelinepost to return", Description = "Id of timelinepost to return", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Summary = "Successful operation", Description = "Successful operation", Example = typeof(bool))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Timeline not found", Description = "Timeline not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> PutLikeOnPost([HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "timelinepost/{timelinePostId}/like")] HttpRequestData req, string timelinePostId, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                bool success = false;
                try
                {
                    success = await _timelineService.PutLikeOnPost(currentUser.FindFirst(ClaimTypes.Sid).Value, timelinePostId);
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

        [Function(nameof(TimelinePostHttpTrigger.DeleteLikeOnPost))]
        [OpenApiOperation(operationId: "deleteLikeOnPost", tags: new[] { "timelinepost" }, Summary = "Deletes a like on a post", Description = "Returns true/false if delete was successful", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "timelinePostId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Id of the timelinepost to delete", Description = "Id of timelinepost to delete", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Summary = "Successful operation", Description = "Successful operation", Example = typeof(bool))]
        [UnauthorizedRequest]
        [ForbiddenRequest]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid id supplied", Description = "Invalid id supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "Timeline not found", Description = "Timeline not found")]
        [VitalityAppAuth]
        public async Task<HttpResponseData> DeleteLikeOnPost([HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "timelinepost/{timelinePostId}/like")] HttpRequestData req, string timelinePostId, FunctionContext executionContext)
        {
            return await _requestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                bool success = false;

                try
                {
                    success = await _timelineService.DeleteLikeOnPost(currentUser.FindFirst(ClaimTypes.Sid).Value, timelinePostId);
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
