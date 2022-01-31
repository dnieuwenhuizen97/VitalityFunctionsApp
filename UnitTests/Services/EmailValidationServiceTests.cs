using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services
{
    public class EmailValidationServiceTests
    {
        private readonly EmailValidationService _emailValidationService;

        public EmailValidationServiceTests()
        {
            _emailValidationService = new EmailValidationService();
        }

        [Fact]
        public async Task Validate_Employee_Email_Should_Return_True_When_Valid_Email_Is_Entered()
        {
            // Arrange
            string email = "dylan.nieuwenhuizen@inholland.nl";

            // Act
            bool result = await _emailValidationService.ValidateEmployeeEmail(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Validate_Employee_Email_Should_Return_False_When_Invalid_Email_Is_Entered()
        {
            // Arrange
            string email = "dylannieuwenhuizen@mail.nl";

            // Act
            bool result = await _emailValidationService.ValidateEmployeeEmail(email);

            // Assert
            Assert.False(result);
        }
    }
}
