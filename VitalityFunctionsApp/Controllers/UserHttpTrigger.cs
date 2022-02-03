using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domains;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System.Net;
using VitalityApp.Authentication;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Security.Interfaces;
using System.Security.Claims;
using Domains.DTO;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using HttpMultipartParser;
using SendGrid.Helpers.Errors.Model;
using Domains.Enums;

namespace VitalityFunctionsApp.Controllers
{
    [ApiController]
    public class UserHttpTrigger
    {
        IEmailValidationService ValidationService { get; }
        IUserService UserService { get; }
        IBlobStorageService BlobStorageService { get; }
        IRequestValidator RequestValidator { get; }
        ILogger Logger;
        IInputSanitizationService InputSanitizationService { get; }

        public UserHttpTrigger(IEmailValidationService ValidationService, IUserService UserService, IBlobStorageService BlobStorageService, IRequestValidator RequestValidator, ILogger<UserHttpTrigger> Logger, IInputSanitizationService InputSanitizationService)
        {
            this.ValidationService = ValidationService;
            this.UserService = UserService;
            this.BlobStorageService = BlobStorageService;
            this.RequestValidator = RequestValidator;
            this.Logger = Logger;
            this.InputSanitizationService = InputSanitizationService;
        }

        [Function(nameof(GetUser))]
        [OpenApiOperation(operationId: "getUser", tags: new[] { "user" }, Summary = "Get your profile, or someone else's. If no userId is given, it returns your own profile", Description = "Returns the profile on userId or your own", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "The userId of the other user to see the profile of. If this is empty the default returns your profile based on authorization", Description = "The userId of the other user to see the profile of. If this is empty the default returns your profile based on authorization", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDTO), Summary = "Successfully retrieved properties of user", Description = "Successful operation", Example = typeof(UserDTOExample))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "BadRequest no parameters", Description = "BadRequest no parameters")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
        [VitalityAppAuth]
        [ForbiddenRequest]
        [UnauthorizedRequest]
        public async Task<HttpResponseData> GetUser([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "user")] HttpRequestData req, FunctionContext executionContext, string userId)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                UserDTO user = new UserDTO();
                string currentUserId = currentUser.FindFirst(ClaimTypes.Sid).Value;

                try
                {
                    if (userId == null)
                    {
                        user = UserService.GetUserDtoById(currentUserId);
                    }
                    else
                    {
                        user = UserService.GetUserDtoByIdWithFollowingProperty(userId, currentUserId);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                }

                await response.WriteAsJsonAsync(user);

                return response;
            });
        }

        // Register new user
        [Function(nameof(RegisterNewUser))]
        [OpenApiOperation(operationId: "registerUser", tags: new[] { "user" }, Summary = "Register a new user", Description = "This will add a new user to the database", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UserRegisterRequest), Required = true, Description = "User object that needs to be added to the database")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserRegisterRequest), Summary = "New user registered", Description = "New user registered", Example = typeof(UserRegisterExample))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid input", Description = "Invalid input")]
        public async Task<HttpResponseData> RegisterNewUser([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "user")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.Created);
            //string responseMessage = "An e-mail with a verification link has been sent";

            string requestbody = await new StreamReader(req.Body).ReadToEndAsync();
            UserRegisterRequest user = JsonConvert.DeserializeObject<UserRegisterRequest>(requestbody);

            //if (!await ValidationService.ValidateEmployeeEmail(user.Email) || !await ValidationService.ValidatePassword(user.Password))
            //{
            //    HttpResponseData badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            //    return badResponse;
            //}

            try
            {
                await UserService.AddUser(user);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                response = req.CreateResponse(HttpStatusCode.BadRequest);
            }



            return response;
        }

        // PUT Verification of account
        [Function(nameof(VerifyUserAccount))]
        [OpenApiOperation(operationId: "verifyUserAccount", tags: new[] { "user" }, Summary = "Activates a user account by user id", Description = "Activates a user account by user id using the verification link sent to the user's e-mail", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "Id of user to activate", Description = "The id of the user that needs to be activated", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Summary = "Successfully activated user", Description = "Successfully activated user")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "The request was invalid", Description = "The request was invalid")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "The user not found", Description = "The user not found")]
        public async Task<HttpResponseData> VerifyUserAccount([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "user/verify")] HttpRequestData req, FunctionContext executionContext)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(req.Url.Query);
            string userId = query["userId"];

            string responseMessage = "Your Inholland VitalityApp account has been verified";

            try
            {
                await UserService.ActivateUser(userId);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                response = req.CreateResponse(HttpStatusCode.NotFound);
                responseMessage = "The supplied user id does not exist.";
            }

            await response.WriteAsJsonAsync(responseMessage);
            return response;
        }

        // Get multiple users by Name
        [Function(nameof(GetUserByName))]
        [OpenApiOperation(operationId: "getUserByName", tags: new[] { "user" }, Summary = "Find user(s) by name", Description = "Returns a list of users", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Name of users to search for", Description = "Name of users to search for", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The limit of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The offset of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<UserDTO>), Summary = "Successfully retrieved users", Description = "Successful operation", Example = typeof(UserDTOExamples))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid name supplied", Description = "Invalid name supplied")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "No users found", Description = "No users found")]
        [VitalityAppAuth]
        [ForbiddenRequest]
        [UnauthorizedRequest]
        public async Task<HttpResponseData> GetUserByName([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "users/{name}")] HttpRequestData req, FunctionContext executionContext, string name, int limit, int offset)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

                List<UserSearchDTO> users = new List<UserSearchDTO>();

                string currentUserId = currentUser.FindFirst(ClaimTypes.Sid).Value;

                try
                {
                    users = await UserService.GetUsersByName(name, currentUserId, limit, offset);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }

                await response.WriteAsJsonAsync(users);

                return response;
            });
        }

        [Function(nameof(UpdateProfile))]
        [OpenApiOperation(operationId: "updateProfile", tags: new[] { "user" }, Summary = "Update user properties", Description = "Update (a selection of) profile properties for the user", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "multipart/form-data", bodyType: typeof(UserUpdatePropertiesDTO), Description = "Parameters", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDTO), Summary = "Successfully updated properties", Description = "Successfully updated properties", Example = typeof(UserUpdatePropertiesExample))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "The request was invalid", Description = "The request was invalid")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "The user not found", Description = "The user not found")]
        [VitalityAppAuth]
        [ForbiddenRequest]
        [UnauthorizedRequest]
        public async Task<HttpResponseData> UpdateProfile([HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "user")] HttpRequestData req, FunctionContext executionContext)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

                try
                {
                    // Rip the multiformdata into pieces
                    var parsedFormBody = await MultipartFormDataParser.ParseAsync(req.Body);
                    var parameters = parsedFormBody.Parameters;

                    string firstname = "";
                    string lastname = "";
                    string jobTitle = "";
                    string location = "";
                    string description = "";
                    string password = "";

                    // Retrieve the "parameters" from request
                    List<ParametersKeys> parametersKeysDTO = parameters.Select(x => new ParametersKeys { Data = x.Data, Text = x.Name }).ToList();

                    var formFirstname = parametersKeysDTO.FirstOrDefault(x => x.Text == "firstname");
                    var formLastname = parametersKeysDTO.FirstOrDefault(x => x.Text == "lastname");
                    var formJobTitle = parametersKeysDTO.FirstOrDefault(x => x.Text == "jobTitle");
                    var formLocation = parametersKeysDTO.FirstOrDefault(x => x.Text == "location");
                    var formDescription = parametersKeysDTO.FirstOrDefault(x => x.Text == "description");
                    var formPassword = parametersKeysDTO.FirstOrDefault(x => x.Text == "password");

                    if (formFirstname != null)
                        firstname = await InputSanitizationService.SanitizeInput(formFirstname.Data);
                    if (formLastname != null)
                        lastname = await InputSanitizationService.SanitizeInput(formLastname.Data);
                    if (formJobTitle != null)
                        jobTitle = await InputSanitizationService.SanitizeInput(formJobTitle.Data);
                    if (formLocation != null)
                        location = await InputSanitizationService.SanitizeInput(formLocation.Data);
                    if (formDescription != null)
                        description = await InputSanitizationService.SanitizeInput(formDescription.Data);
                    if (formPassword != null)
                    {
                        if (formPassword.Data.Length > 0)
                        {
                            password = formPassword.Data;
                            if (!await ValidationService.ValidatePassword(password))
                            {
                                return req.CreateResponse(HttpStatusCode.BadRequest);
                            }
                        }
                    }

                    // Retrieve image from request
                    List<StreamContentDTO> files = new List<StreamContentDTO>();

                    files.AddRange(parsedFormBody.Files.Select(x =>
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
                    } else
                    {
                        Logger.LogError("No image was uploaded");
                    }

                    // create
                    UserUpdatePropertiesDTO request = new UserUpdatePropertiesDTO
                    {
                        Firstname = firstname,
                        Lastname = lastname,
                        JobTitle = jobTitle,
                        Location = location,
                        Description = description,
                        Password = password,
                        Image = image
                    };
                    await UserService.UpdateUser(currentUser.FindFirst(ClaimTypes.Sid).Value, request);
                    await response.WriteAsJsonAsync(request);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }

                return response;
            });
        }

        [Function(nameof(UpdateFollowing))]
        [OpenApiOperation(operationId: "updateFollowing", tags: new[] { "user" }, Summary = "Update to (un)follow a user", Description = "Update to (un)follow a user", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "The userId to follow/unfollow", Description = "The userId to follow/unfollow", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "following", In = ParameterLocation.Query, Required = true, Type = typeof(bool), Summary = "If true, follow the user. If false, unfollow the user.", Description = "If true, follow the user. If false, unfollow the user.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDTO), Summary = "Successfully updated followers", Description = "Successfully updated followers", Example = typeof(UserDTOExample))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "The request was invalid", Description = "The request was invalid")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "The user not found", Description = "The user not found")]
        [VitalityAppAuth]
        [ForbiddenRequest]
        [UnauthorizedRequest]
        public async Task<HttpResponseData> UpdateFollowing([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "user/follow")] HttpRequestData req, FunctionContext executionContext, string userId, bool following)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                UserDTO user = new UserDTO();
                try
                {
                    user = await UserService.FollowUserById(currentUser.FindFirst(ClaimTypes.Sid).Value, userId, following);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }
                await response.WriteAsJsonAsync(user);
                return response;
            });
        }

        [Function(nameof(GetScoreboard))]
        [OpenApiOperation(operationId: "getScoreboard", tags: new[] { "user" }, Summary = "Get a list of users ordered by their amount of points", Description = "Returns a list of users ordered by their amount of points", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The limit of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Summary = "The offset of determining the pagination", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ScoreboardUserDTO), Summary = "Successfully retrieved properties of user", Description = "Successful operation")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "BadRequest no parameters", Description = "BadRequest no parameters")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
        [VitalityAppAuth]
        [ForbiddenRequest]
        [UnauthorizedRequest]
        public async Task<HttpResponseData> GetScoreboard([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "user/scoreboard")] HttpRequestData req, FunctionContext executionContext, int limit, int offset)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                List<ScoreboardUserDTO> users = new List<ScoreboardUserDTO>();

                try
                {
                    users = await UserService.GetAllUsersArrangedByPoints(limit, offset);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                }

                await response.WriteAsJsonAsync(users);

                return response;
            });
        }

        [Function(nameof(DeleteUser))]
        [OpenApiOperation(operationId: "deleteUser", tags: new[] { "user" }, Summary = "Delete the current user's account", Description = "Delete the current user's account", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = false, Type = typeof(string), Summary = "The userId of the user that needs to be deleted (admin)", Description = "Deletes a specific user by id (admin)", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserDTO), Summary = "Successfully deleted user", Description = "Successful operation", Example = typeof(UserDTOExample))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "BadRequest no parameters", Description = "BadRequest no parameters")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Summary = "User not found", Description = "User not found")]
        [VitalityAppAuth]
        [ForbiddenRequest]
        [UnauthorizedRequest]
        public async Task<HttpResponseData> DeleteUser([HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "user")] HttpRequestData req, FunctionContext executionContext, string userId)
        {
            return await RequestValidator.ValidateRequest(req, executionContext, UserType.User.ToString(), async (ClaimsPrincipal currentUser) =>
            {
                HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
                UserDTO user = new UserDTO();
                string currentUserId = currentUser.FindFirst(ClaimTypes.Sid).Value;

                try
                {
                    if (userId == null)
                    {
                        user = await UserService.DeleteUserById(currentUserId);
                    }
                    else
                    {
                        if (currentUser.FindFirst(ClaimTypes.Role).Value != UserType.Admin.ToString())
                        {
                            response = req.CreateResponse(HttpStatusCode.Forbidden);
                            return response;
                        }
                        user = await UserService.DeleteUserById(userId);
                    }
                    await response.WriteAsJsonAsync(user);
                }
                catch (NotFoundException nfe)
                {
                    Logger.LogError(nfe.Message);
                    response = req.CreateResponse(HttpStatusCode.NotFound);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    response = req.CreateResponse(HttpStatusCode.BadRequest);
                }

                return response;
            });
        }

        [Function(nameof(RecoverAccount))]
        [OpenApiOperation(operationId: "recoverAccount", tags: new[] { "user" }, Summary = "Request a recoverylink for the user", Description = "Request a recoverylink for the user")]
        [OpenApiParameter(name: "email", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "The e-mail address for the user to be recovered", Description = "Sends an e-mail with recovery link to the specified e-mail address", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid e-mail address", Description = "Invalid e-mail address")]
        public async Task<HttpResponseData> RecoverAccount([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "user/recover")] HttpRequestData req, FunctionContext executionContext, string email)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            //if (!await ValidationService.ValidateEmployeeEmail(user.Email))
            //{
            //    HttpResponseData badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            //    return badResponse;
            //}

            try
            {
                await UserService.CreateRecoveryToken(email);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                response = req.CreateResponse(HttpStatusCode.BadRequest);
            }


            return response;
        }

        [Function(nameof(ResetPassword))]
        [OpenApiOperation(operationId: "resetPassword", tags: new[] { "user" }, Summary = "Resets the user's password", Description = "Resets the user's password with the help of a recovery token")]
        [OpenApiParameter(name: "password", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "The user's new password", Description = "The user's new password", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "recoveryToken", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "The recovery token", Description = "The token that confirms that a user is allowed to reset his password", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid input", Description = "Invalid input")]
        public async Task<HttpResponseData> ResetPassword([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "user/password")] HttpRequestData req, FunctionContext executionContext, string password, string recoveryToken)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            if (!await ValidationService.ValidatePassword(password))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            try
            {
                await UserService.ResetUserPassword(password, recoveryToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                response = req.CreateResponse(HttpStatusCode.BadRequest);
            }


            return response;
        }

        [Function(nameof(CheckRecoveryTokenValidity))]
        [OpenApiOperation(operationId: "checkRecoveryTokenValidity", tags: new[] { "user" }, Summary = "Checks if a user's password recovery token is valid", Description = "Checks if a user's password recovery token is valid")]
        [OpenApiParameter(name: "recoveryToken", In = ParameterLocation.Query, Required = true, Type = typeof(string), Summary = "The recovery token", Description = "The token that confirms that a user is allowed to reset his password", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Summary = "Invalid input", Description = "Invalid input")]
        public async Task<HttpResponseData> CheckRecoveryTokenValidity([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "user/recover")] HttpRequestData req, FunctionContext executionContext, string recoveryToken)
        {
            HttpResponseData response = req.CreateResponse(HttpStatusCode.BadRequest);

            try
            {
                if (await UserService.IsRecoveryTokenValid(recoveryToken))
                {
                    response = req.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }


            return response;
        }
    }
}
