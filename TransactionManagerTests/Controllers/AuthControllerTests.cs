using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Security.Claims;
using TransactionManager.Controllers;
using TransactionManager.Models;
using Xunit;

namespace TransactionManagerTests.Controllers
{

    public class AuthControllerTests
    {
        [Fact]
        public void Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            //configuration.SetupGet(x => x[It.IsAny<string>()]).Returns("your-configuration-values");

            var logger = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(configuration.Object, logger.Object);

            var validModel = new LoginRequest
            {
                UserName = "lenvi",
                Password = "P4ssw0rd"
            };

            // Act
            var result = controller.Login(validModel) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = result.Value as dynamic;
            Assert.NotNull(response);
            Assert.True(response.Token != null);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(x => x[It.IsAny<string>()]).Returns("your-configuration-values");

            var logger = new Mock<ILogger<AuthController>>();
            var controller = new AuthController(configuration.Object, logger.Object);

            var invalidModel = new LoginRequest
            {
                UserName = "invalid-username",
                Password = "invalid-password"
            };

            // Act
            var result = controller.Login(invalidModel) as UnauthorizedObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = result.Value as dynamic;
            Assert.NotNull(response);
            Assert.Equal("Invalid username or password.", response.ToString());
        }
    }
}
