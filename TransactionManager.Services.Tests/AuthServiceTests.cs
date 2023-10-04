using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using TransactionManager.Services.Authentication;

public class AuthServiceTests
{
    [Fact]
    public void Authenticate_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.SetupGet(x => x["Authentication:UserName"]).Returns("validUser");
        configuration.SetupGet(x => x["Authentication:Password"]).Returns("validPassword");

        var authService = new AuthService(configuration.Object);

        // Act
        bool isAuthenticated = authService.Authenticate("validUser", "validPassword");

        // Assert
        Assert.True(isAuthenticated);
    }

    [Fact]
    public void Authenticate_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.SetupGet(x => x["Authentication:UserName"]).Returns("validUser");
        configuration.SetupGet(x => x["Authentication:Password"]).Returns("validPassword");

        var authService = new AuthService(configuration.Object);

        // Act
        bool isAuthenticated = authService.Authenticate("invalidUser", "invalidPassword");

        // Assert
        Assert.False(isAuthenticated);
    }

    [Fact]
    public void GenerateJwtToken_ReturnsValidToken()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.SetupGet(x => x["Jwt:SecretKey"]).Returns("Th1s 1s a very long secret key @ 2023");
        configuration.SetupGet(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        configuration.SetupGet(x => x["Jwt:Audience"]).Returns("TestAudience");
        configuration.SetupGet(x => x["Jwt:TokenExpirationSeconds"]).Returns("600");

        var authService = new AuthService(configuration.Object);

        // Act
        string token = authService.GenerateJwtToken("testUser");

        // Assert
        Assert.NotNull(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        Assert.Equal("TestIssuer", securityToken?.Issuer);
        Assert.Equal("TestAudience", securityToken?.Audiences?.First());
        Assert.True(securityToken?.ValidTo > DateTime.UtcNow);
    }

    [Fact]
    public void GenerateJwtToken_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.SetupGet(x => x["Jwt:SecretKey"]).Returns("TestSecretKey");
        configuration.SetupGet(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        configuration.SetupGet(x => x["Jwt:Audience"]).Returns("TestAudience");
        configuration.SetupGet(x => x["Jwt:TokenExpirationSeconds"]).Returns("600");

        var authService = new AuthService(configuration.Object);

        // Act
        Assert.Throws<ArgumentOutOfRangeException>(() => authService.GenerateJwtToken("testUser"));
    }
}
