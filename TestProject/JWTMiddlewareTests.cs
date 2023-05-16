using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Portal;

namespace TestProject;

public class JwtMiddlewareTests
{
    private JwtMiddleware _jwtMiddleware;
    private IConfiguration _configuration;
    private RequestDelegate _next;

    [SetUp]
    public void Setup()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["JWT:Secret"]).Returns("ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM");
        _configuration = configMock.Object;
        _next = context => Task.CompletedTask;
        _jwtMiddleware = new JwtMiddleware(_next, _configuration);
    }

    private string GenerateToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("UserId", userId) }),
            Claims = new Dictionary<string, object>() { { "Role", "User" } },
            Expires = DateTime.UtcNow.AddMinutes(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [Test]
    public async Task Invoke_ValidToken_AddsUserIdToContext()
    {
        var userId = "testUserId";
        var token = GenerateToken(userId);
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = $"Bearer {token}";

        await _jwtMiddleware.Invoke(context);

        Assert.IsTrue(context.Items.ContainsKey("UserId"));
        Assert.That(context.Items["UserId"], Is.EqualTo(userId));
    }

    [Test]
    public async Task Invoke_InvalidToken_SetsUnauthorizedStatusCode()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer invalid_token";

        await _jwtMiddleware.Invoke(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
    }

    [Test]
    public async Task Invoke_MissingToken_SetsUnauthorizedStatusCode()
    {
        var context = new DefaultHttpContext();

        await _jwtMiddleware.Invoke(context);

        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
    }
}