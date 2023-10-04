using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TransactionManager.Controllers;
using TransactionManager.Models;
using TransactionManager.Services.Authentication;

public class AuthControllerTests
{
    [Fact]
    public void Login_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<AuthController>>();
        var authService = new Mock<IAuthService>();
        var controller = new AuthController(configuration.Object, logger.Object, authService.Object);

        // Act
        var result = controller.Login(null) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result?.StatusCode);
    }

    [Fact]
    public void Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<AuthController>>();
        var authService = new Mock<IAuthService>();
        authService.Setup(a => a.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        var controller = new AuthController(configuration.Object, logger.Object, authService.Object);

        var model = new LoginRequest
        {
            UserName = "testUser",
            Password = "invalidPassword"
        };

        // Act
        var result = controller.Login(model) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result?.StatusCode);

        var response = result?.Value as LoginResponse;
        Assert.NotNull(response);
        Assert.False(response?.IsSuccess);
        Assert.Equal("Invalid Username or Password.", response?.Message);
    }

    [Fact]
    public void Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<AuthController>>();
        var authService = new Mock<IAuthService>();
        authService.Setup(a => a.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        authService.Setup(a => a.GenerateJwtToken(It.IsAny<string>())).Returns("testToken");
        configuration.SetupGet(x => x["Jwt:TokenExpirationSeconds"]).Returns("600");
        var controller = new AuthController(configuration.Object, logger.Object, authService.Object);

        var model = new LoginRequest
        {
            UserName = "testUser",
            Password = "validPassword"
        };

        // Act
        var result = controller.Login(model) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);

        var response = result?.Value as LoginResponse;
        Assert.NotNull(response);
        Assert.True(response?.IsSuccess);
        Assert.Equal("testToken", response?.Token);
        Assert.Equal("Login successful.", response?.Message);
    }
}
