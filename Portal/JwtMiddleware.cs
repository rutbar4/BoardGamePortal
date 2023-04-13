using Microsoft.IdentityModel.Tokens;
using Mysqlx;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Portal;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last().ToString();
        var deconstructedToken = ValidateToken(token);
        
        if (deconstructedToken is not null)
        {
            string? userId = deconstructedToken.Value.userId;
            string? profileId = deconstructedToken.Value.profileId;
            if (userId is not null && profileId is not null)
            {
                // attach user to context on successful jwt validation
                context.Items.Add("UserId", userId);
                context.Items.Add("profileId", profileId);
                _next.Invoke(context);
            }
        }
        else
        {
            context.Response.StatusCode = 401; //UnAuthorized
            await context.Response.WriteAsync("Invalid token");
            return;
        }
    }

    private (string? userId, string? profileId)? ValidateToken(string token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "UserId").Value;
            var profileId = jwtToken.Claims.First(x => x.Type == "profileId").Value;

            // return user id from JWT token if validation successful
            return (userId, profileId);
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }
}

