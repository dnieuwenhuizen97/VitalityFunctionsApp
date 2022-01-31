using Domains;
using Domains.DTO;
using Infrastructure.Context;
using Infrastructure.Context.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Services;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<INotificationService> _notificationServiceMock = new Mock<INotificationService>();
        private readonly Mock<IQueueStorageService> _queueStorageMock = new Mock<IQueueStorageService>();
        private readonly Mock<IBlobStorageService> _blobStorageMock = new Mock<IBlobStorageService>();
        private readonly Mock<IPushTokenDb> _pushTokenDbMock = new Mock<IPushTokenDb>();
        private readonly Mock<IUserDb> _userDbMock = new Mock<IUserDb>();
        private readonly Mock<ILogger<UserService>> _loggerMock = new Mock<ILogger<UserService>>();


        public UserServiceTests()
        {
            _userService = new UserService(_notificationServiceMock.Object, _queueStorageMock.Object, _blobStorageMock.Object, _userDbMock.Object, _pushTokenDbMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Add_User_Should_Return_User_Object()
        {
            // Arrange
            UserRegisterRequest registerRequest = new UserRegisterRequest(
                "dylan.nieuwenhuizen@inholland.nl",
                "password");

            // Act
            User user = await _userService.AddUser(registerRequest);

            // Assert
            Assert.NotNull(user);
        }

        [Fact]
        public async Task Add_User_Should_Throw_Exception_When_User_Already_Exists()
        {
            // Arrange
            UserRegisterRequest registerRequest = new UserRegisterRequest(
                "dylan.nieuwenhuizen@inholland.nl",
                "password");

            _userDbMock.Setup(x => x.UserExistsByEmail(registerRequest.Email))
                .Returns(true);

            // Act
            var exception = Assert.Throws<Exception>(() => { _userService.AddUser(registerRequest); });

            // Assert
            Assert.Equal("A user with this e-mail address already exists", exception.Message);
        }

        [Fact]
        public async Task Get_User_By_Id_Should_Return_User_Object_When_User_Exists()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            User testUser = new User
            {
                UserId = userId,
                Firstname = "Dylan",
                Lastname = "Nieuwenhuizen",
                Email = "dnieuwenhuizen@mail.com"
            };

            _userDbMock.Setup(x => x.FindUserById(userId))
               .Returns(testUser);

            // Act
            User user = _userService.GetUserById(userId);


            // Assert
            Assert.Equal(testUser, user);
        }

        [Fact]
        public async Task Get_User_By_Id_Should_Return_Null_When_User_Does_Not_Exist()
        {
            // Arrange
            _userDbMock.Setup(x => x.FindUserById(It.IsAny<string>()))
               .Returns(() => null);

            // Act
            User user = _userService.GetUserById(Guid.NewGuid().ToString());


            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task Get_User_Dto_By_Id_Should_Return_User_Dto_Object_When_User_Exists()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            User testUser = new User
            {
                UserId = userId,
                Firstname = "Dylan",
                Lastname = "Nieuwenhuizen",
                Email = "dnieuwenhuizen@mail.com",
                Description = "testen testen testen",
                HasAdminPriviledges = true,
                IsVerified = true,
                JobTitle = "Software Tester",
                ProfilePicture = "blob.com/profilepic.png",
                Location = "Alkmaar",
                Followers = new List<Follower>(),
                SubscribedChallenges = new List<SubscribedChallenge>(),
                Points = 69420
            };

            _userDbMock.Setup(x => x.FindUserById(userId))
               .Returns(testUser);

            // Act
            UserDTO userDTO = _userService.GetUserDtoById(userId);


            // Assert
            Assert.Equal(testUser.UserId, userDTO.UserId);
            Assert.Equal(testUser.Firstname, userDTO.FirstName);
            Assert.Equal(testUser.Firstname, userDTO.FirstName);
            Assert.Equal(testUser.Lastname, userDTO.LastName);
            Assert.Equal(testUser.Location, userDTO.Location);
            Assert.True(userDTO.Followers is List<FollowerDTO>);
            Assert.True(userDTO.Challenges is List<SubscribedChallengeDTO>);
            Assert.Equal(userDTO.Points, testUser.Points);
            Assert.Equal(userDTO.ProfilePicture, testUser.ProfilePicture);
            Assert.Equal(userDTO.Description, testUser.Description);
            Assert.Equal(userDTO.JobTitle, testUser.JobTitle);
        }

        [Fact]
        public async Task Get_User_Dto_By_Id_Should_Throw_Exception_When_User_Does_Not_Exist()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();

            _userDbMock.Setup(x => x.FindUserById(It.IsAny<string>()))
               .Returns(() => null);

            // Act
            var exception = Assert.Throws<NullReferenceException>(() => { _userService.GetUserDtoById(userId); }); ;


            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Message);
        }

        [Fact]
        public async Task Activate_User_Should_Set_IsVerified_Property_To_True_When_User_Exists()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            User testUser = new User
            {
                UserId = userId,
                Firstname = "Dylan",
                Lastname = "Nieuwenhuizen",
                Email = "dnieuwenhuizen@mail.com",
                IsVerified = false
            };

            User testUserResult = new User
            {
                UserId = userId,
                Firstname = "Dylan",
                Lastname = "Nieuwenhuizen",
                Email = "dnieuwenhuizen@mail.com",
                IsVerified = true
            };

            _userDbMock.Setup(x => x.SetActivated(userId))
                .Returns(Task.FromResult(testUserResult));

            // Act
            User user = await _userService.ActivateUser(userId);

            // Assert
            Assert.True(user.IsVerified);
        }

        [Fact]
        public async Task Acitivate_User_Should_Throw_Exception_When_User_Does_Not_Exist()
        {
            // Arrange
            _userDbMock.Setup(x => x.SetActivated(It.IsAny<string>()))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAnyAsync<Exception>(async () => { await _userService.ActivateUser(Guid.NewGuid().ToString()); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }

        [Fact]
        public async Task Login_User_Should_Return_User_Object_When_Valid_Login_Request_Is_Used()
        {
            // Arrange
            LoginRequest loginRequest = new LoginRequest
            {
                Email = "dylan.nieuwenhuizen@inholland.nl",
                Password = "******"
            };

            User testUser = new User
            {
                Email = loginRequest.Email
            };
            testUser.SetUserPassword(loginRequest.Password);

            _userDbMock.Setup(x => x.CheckUserCredentials(loginRequest))
                .Returns(Task.FromResult(testUser));

            // Act
            User user = await _userService.LoginUser(loginRequest);

            // Assert
            Assert.NotNull(user);
        }

        [Fact]
        public async Task Login_User_Should_Return_Null_When_Invalid_Login_Request_Is_Used()
        {
            // Arrange
            LoginRequest validLoginRequest = new LoginRequest
            {
                Email = "dylan.nieuwenhuizen@inholland.nl",
                Password = "******"
            };

            LoginRequest invalidLoginRequest = new LoginRequest
            {
                Email = "dylan.nieuwenhuizen@inholland.nl",
                Password = "**********"
            };

            User testUser = new User
            {
                Email = validLoginRequest.Email
            };
            testUser.SetUserPassword(validLoginRequest.Password);

            _userDbMock.Setup(x => x.CheckUserCredentials(validLoginRequest))
               .Returns(Task.FromResult(testUser));

            // Act
            User user = await _userService.LoginUser(invalidLoginRequest);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task Follow_User_By_Id_Should_Return_Followed_User_Dto()
        {
            // Arrange
            string currentUserId = Guid.NewGuid().ToString();
            User currentUser = new User
            {
                UserId = currentUserId
            };

            string userToFollowId = Guid.NewGuid().ToString();
            User userToFollow = new User
            {
                UserId = userToFollowId,
                Followers = new List<Follower>(),
                SubscribedChallenges = new List<SubscribedChallenge>()
            };

            _userDbMock.Setup(x => x.FindUserById(userToFollowId))
                .Returns(userToFollow);

            // Act
            UserDTO followedUser = await _userService.FollowUserById(currentUserId, userToFollowId, true);

            // Assert
            Assert.NotNull(followedUser);
        }

        [Fact]
        public async Task Follow_User_By_Id_Should_Throw_Exception_When_Current_User_Is_Equal_To_User_To_Follow()
        {
            // Arrange
            string currentUserId = Guid.NewGuid().ToString();
            User currentUser = new User
            {
                UserId = currentUserId
            };

            User userToFollow = new User
            {
                UserId = currentUserId,
                Followers = new List<Follower>(),
                SubscribedChallenges = new List<SubscribedChallenge>()
            };

            _userDbMock.Setup(x => x.FindUserById(currentUserId))
                .Returns(userToFollow);

            // Act
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => { await _userService.FollowUserById(currentUserId, currentUserId, true); });

            // Assert
            Assert.Equal("Id of the user to follow cannot be the same as the current user id", exception.Result.Message);
        }

        [Fact]
        public async Task Get_Users_By_Name_Should_Return_List_Of_User_DTOs()
        {
            // Arrange
            string currentUserId = Guid.NewGuid().ToString();
            string name = "dylan";
            int limit = 10;
            int offset = 0;

            List<User> users = new List<User>()
            {
                new User { UserId = Guid.NewGuid().ToString(), Firstname = "dylan1", Followers = new List<Follower>(), SubscribedChallenges = new List<SubscribedChallenge>()},
                new User { UserId = Guid.NewGuid().ToString(), Firstname = "dylan2", Followers = new List<Follower>(), SubscribedChallenges = new List<SubscribedChallenge>()},
                new User { UserId = Guid.NewGuid().ToString(), Firstname = "dylan3", Followers = new List<Follower>(), SubscribedChallenges = new List<SubscribedChallenge>()},
                new User { UserId = Guid.NewGuid().ToString(), Firstname = "dylan4", Followers = new List<Follower>(), SubscribedChallenges = new List<SubscribedChallenge>()},
            };

            _userDbMock.Setup(x => x.FindUsersByName(name, currentUserId, limit, offset))
                .Returns(Task.FromResult(users));

            // Act
            List<UserSearchDTO> userDTOs = await _userService.GetUsersByName(name, currentUserId, limit, offset);

            // Assert
            Assert.NotNull(userDTOs);
            Assert.True(userDTOs.Count == 4);
        }

        [Fact]
        public async Task Get_Users_By_Name_Should_Return_Empty_List_When_No_Users_Found_With_Given_Name()
        {
            // Arrange
            string currentUserId = Guid.NewGuid().ToString();
            string name = "dylan";
            int limit = 10;
            int offset = 0;

            _userDbMock.Setup(x => x.FindUsersByName(name, currentUserId, limit, offset))
                .Returns(Task.FromResult(new List<User>()));

            // Act
            List<UserSearchDTO> userDTOs = await _userService.GetUsersByName(name, currentUserId, limit, offset);

            // Assert
            Assert.True(userDTOs.Count == 0);
        }

        [Fact]
        public async Task Update_User_Should_Be_Successful_When_Valid_UserId_Is_Given()
        {
            // Arrange
            User user = new User
            {
                UserId = Guid.NewGuid().ToString()
            };

            UserUpdatePropertiesDTO updatedProperties = new UserUpdatePropertiesDTO
            {
                Firstname = "Dylan",
                Lastname = "Nieuwenhuizen",
                Description = "Testen testen testen",
                JobTitle = "Software Tester",
                Location = "Alkmaar"
            };

            User userWithUpdatedProperties = new User
            {
                UserId = user.UserId,
                Firstname = updatedProperties.Firstname,
                Lastname = updatedProperties.Lastname,
                Description = updatedProperties.Description,
                JobTitle = updatedProperties.JobTitle,
                Location = updatedProperties.Location
            };

            _userDbMock.Setup(x => x.UpdateUserProperties(user.UserId, updatedProperties));
            _userDbMock.Setup(x => x.FindUserById(user.UserId))
                .Returns(userWithUpdatedProperties);

            // Act
            await _userService.UpdateUser(user.UserId, updatedProperties);
            User updatedUser = _userService.GetUserById(user.UserId);

            // Assert
            Assert.Equal(updatedProperties.Firstname, updatedUser.Firstname);
            Assert.Equal(updatedProperties.Lastname, updatedUser.Lastname);
            Assert.Equal(updatedProperties.Description, updatedUser.Description);
            Assert.Equal(updatedProperties.JobTitle, updatedUser.JobTitle);
            Assert.Equal(updatedProperties.Location, updatedUser.Location);
        }

        [Fact]
        public async Task Update_User_Profile_Picture_Should_Be_Successful_When_Valid_UserId_Is_Given()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            string imageUrl = $"ProfilePic:{userId}";

            User user = new User { UserId = userId, ProfilePicture = imageUrl };

            _userDbMock.Setup(x => x.UpdateUserProfilePicture(userId, imageUrl));
            _userDbMock.Setup(x => x.FindUserById(userId))
                .Returns(user);

            // Act
            await _userService.UpdateProfilePicture(userId);
            User updatedUser = _userService.GetUserById(userId);

            // Assert
            Assert.Equal(imageUrl, updatedUser.ProfilePicture);
        }

        [Fact]
        public async Task Update_User_Total_Points_Should_Be_Successful_When_Valid_UserId_Is_Given()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            int points = 150;

            User user = new User { UserId = userId, Points = points };

            _userDbMock.Setup(x => x.UpdateUserTotalPoints(userId, points));
            _userDbMock.Setup(x => x.FindUserById(userId))
                .Returns(user);

            // Act
            await _userService.UpdateUserTotalPoints(userId, points);
            User updatedUser = _userService.GetUserById(userId);

            // Assert
            Assert.Equal(user.Points, updatedUser.Points);
        }
    }
}
