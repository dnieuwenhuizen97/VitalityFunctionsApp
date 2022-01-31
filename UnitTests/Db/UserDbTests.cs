using Domains;
using Infrastructure.Context;
using Infrastructure.Context.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Db
{
    public class UserDbTests
    {
        private readonly IUserDb _userDb;
        private readonly Mock<IUserDb> _userDbMock;

        public UserDbTests()
        {
            _userDbMock = new Mock<IUserDb>();

            _userDb = _userDbMock.Object;
        }

        [Fact]
        public async Task Set_Activated_Should_Return_User_Object()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            User testUser = new User { UserId = userId, IsVerified = true };

            _userDbMock.Setup(x => x.SetActivated(userId))
                .Returns(Task.FromResult(testUser));

            // Act
            User user = await _userDb.SetActivated(testUser.UserId);

            // Assert
            Assert.Equal(testUser.IsVerified, user.IsVerified);
        }

        [Fact]
        public async Task Set_Activated_Should_Throw_Exception_When_Invalid_UserId_Is_Given()
        {
            // Arrange
            _userDbMock.Setup(x => x.SetActivated(It.IsAny<string>()))
                .Returns(() => null);

            // Act
            var exception = Assert.ThrowsAnyAsync<Exception>(async () => { await _userDb.SetActivated(Guid.NewGuid().ToString()); });

            // Assert
            Assert.Equal("Object reference not set to an instance of an object.", exception.Result.Message);
        }

        [Fact]
        public async Task Check_User_Credentials_Should_Return_User_Object()
        {
            // Arrange
            LoginRequest loginRequest = new LoginRequest
            {
                Email = "dylan.nieuwenhuizen@inholland.nl",
                Password = "password"
            };

            User testUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Email = "dylan.nieuwenhuizen@inholland.nl"
            };
            testUser.SetUserPassword("password");

            _userDbMock.Setup(x => x.CheckUserCredentials(loginRequest))
                .Returns(Task.FromResult(testUser));

            // Act
            User user = await _userDb.CheckUserCredentials(loginRequest);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(loginRequest.Email, user.Email);
            Assert.Equal(loginRequest.Password, user.Password);
        }

        [Fact]
        public async Task Find_User_By_Email_Should_Return_User_Object()
        {
            // Arrange
            string email = "dylan.nieuwenhuizen@inholland.nl";
            User testUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Email = email
            };

            _userDbMock.Setup(x => x.FindUserByEmail(email))
                .Returns(testUser);

            // Act
            User user = _userDb.FindUserByEmail(email);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(testUser.Email, user.Email);
        }

        [Fact]
        public async Task Find_User_By_Email_Should_Return_Null_When_Email_Not_Found()
        {
            // Arrange
            _userDbMock.Setup(x => x.FindUserByEmail(It.IsAny<string>()))
                .Returns(() => null);

            // Act
            User user = _userDb.FindUserByEmail("dylan.nieuwenhuizen@inholland.nl");

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task Find_User_By_Id_Should_Return_User_Object()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            User testUser = new User
            {
                UserId = userId
            };

            _userDbMock.Setup(x => x.FindUserById(testUser.UserId))
                .Returns(testUser);

            // Act
            User user = _userDb.FindUserById(userId);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(testUser.UserId, user.UserId);
        }

        [Fact]
        public async Task Find_User_By_Id_Should_Return_Null_When_Id_Not_Found()
        {
            // Arrange
            _userDbMock.Setup(x => x.FindUserById(It.IsAny<string>()))
                .Returns(() => null);

            // Act
            User user = _userDb.FindUserByEmail(Guid.NewGuid().ToString());

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task Find_Users_By_Name_Should_Return_List_Of_User_Objects()
        {
            // Arrange
            string currentUserId = Guid.NewGuid().ToString();
            string name = "dylan";
            int limit = 10;
            int offset = 0;

            List<User> testUsers = new List<User>()
            {
                new User { UserId = Guid.NewGuid().ToString(), Firstname = "dylan1" },
                new User { UserId = Guid.NewGuid().ToString(), Firstname = "dylan2" },
                new User { UserId = Guid.NewGuid().ToString(), Firstname = "dylan3" },
                new User { UserId = Guid.NewGuid().ToString(), Firstname = "dylan4" },
            };

            _userDbMock.Setup(x => x.FindUsersByName(name, currentUserId, limit, offset))
                .Returns(Task.FromResult(testUsers));

            // Act
            List<User> users = await _userDb.FindUsersByName(name, currentUserId, limit, offset);

            // Arrange
            Assert.NotNull(users);
            Assert.True(users.Count == 4);
        }

        [Fact]
        public async Task Find_Users_By_Name_Should_Return_Empty_List_When_No_Users_Found_With_Given_name()
        {
            // Arrange
            string currentUserId = Guid.NewGuid().ToString();
            string name = "dylan";
            int limit = 10;
            int offset = 0;

            _userDbMock.Setup(x => x.FindUsersByName(name, currentUserId, limit, offset))
                .Returns(Task.FromResult(new List<User>()));

            // Act
            List<User> users = await _userDb.FindUsersByName(name, currentUserId, limit, offset);

            // Assert
            Assert.True(users.Count == 0);
        }

        [Fact]
        public async Task User_Exists_By_Email_Should_Return_True_when_Email_Exists()
        {
            // Arrange
            string email = "dylan.nieuwenhuizen@inholland.nl";
            User testUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Email = email
            };

            _userDbMock.Setup(x => x.UserExistsByEmail(email))
                .Returns(true);

            // Act
            bool exists = _userDb.UserExistsByEmail(email);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task User_Exists_By_Email_Should_Return_False_when_Email_Does_Not_Exist()
        {
            // Arrange
            string email = "dylan.nieuwenhuizen@inholland.nl";
            User testUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Email = email
            };

            _userDbMock.Setup(x => x.UserExistsByEmail(It.IsAny<string>()))
                .Returns(false);

            // Act
            bool exists = _userDb.UserExistsByEmail(email);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task User_Exists_By_Id_Should_Return_True_when_Id_Exists()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            User testUser = new User
            {
                UserId = userId
            };

            _userDbMock.Setup(x => x.UserExistsById(userId))
                .Returns(true);

            // Act
            bool exists = _userDb.UserExistsById(userId);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task User_Exists_By_Id_Should_Return_False_when_Id_Does_Not_Exist()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            User testUser = new User
            {
                UserId = userId
            };

            _userDbMock.Setup(x => x.UserExistsById(It.IsAny<string>()))
                .Returns(false);

            // Act
            bool exists = _userDb.UserExistsByEmail(userId);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task Find_User_With_Followers_Should_Return_User_Object()
        {
            // Arrange
            User testUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Followers = new List<Follower>()
            };

            _userDbMock.Setup(x => x.FindUserWithFollowers(testUser))
                .Returns(Task.FromResult(testUser));

            // Act
            User user = await _userDb.FindUserWithFollowers(testUser);

            // Assert
            Assert.NotNull(user);
        }

        [Fact]
        public async Task Find_User_With_Followers_Should_Return_Null_When_User_Not_Found()
        {
            // Arrange
            User testUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Followers = new List<Follower>()
            };

            User nullUser = null;

            _userDbMock.Setup(x => x.FindUserWithFollowers(nullUser))
                .Returns(() => null);

            // Act
            User user = await _userDb.FindUserWithFollowers(testUser);

            // Assert
            Assert.Null(user);
        }
    }
}
