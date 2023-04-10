using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Portal.DTO;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Portal.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private const string _user_login_table = "user_login";
        private const string _organisation_login_table = "organisation_login";
        IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult LogIn([FromBody] LogInData logInData)
        {
            if (logInData is null)
                return BadRequest();

            var sqlCmd = $"SELECT * FROM {_organisation_login_table} WHERE username=@username AND password=@password";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@username", MySqlDbType.VarChar).Value = logInData.Username;
            da.SelectCommand.Parameters.Add("@password", MySqlDbType.VarChar).Value = logInData.Password;

            var dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0)
                return Unauthorized();

            var token = GetAuthClaims((string)dt.Rows[0]["id"], (string)dt.Rows[0]["fk_profileId"]);

            var response = new LogInResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = logInData.Username
            };
            return Ok(response);
        }

        private JwtSecurityToken GetAuthClaims(string userId, string profileId)
        {
            var authClaims = new List<Claim>
        {
            new Claim("UserId", userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "User"),
            new Claim("profileId", profileId)
        };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
