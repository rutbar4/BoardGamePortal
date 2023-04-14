using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Portal.DBMethods;
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
        private const string _login_table = "login";
        IConfiguration _configuration;
        private readonly UserDBOperations _userDBOperations;
        private readonly OrganisationDBOperations _organisationDBOperations;

        public AuthController(IConfiguration configuration, UserDBOperations userDBOperations, OrganisationDBOperations organisationDBOperations)
        {
            _configuration = configuration;
            _userDBOperations = userDBOperations;
            _organisationDBOperations = organisationDBOperations;
        }

        [HttpPost]
        public IActionResult LogIn([FromBody] LogInData logInData)
        {
            if (logInData is null)
                return BadRequest();

            var sqlCmd = $"SELECT * FROM {_login_table} WHERE username=@username AND password=@password";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@username", MySqlDbType.VarChar).Value = logInData.Username;
            da.SelectCommand.Parameters.Add("@password", MySqlDbType.VarChar).Value = logInData.Password;

            var dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0)
                return Unauthorized();

            string id = (string)dt.Rows[0]["id"];
            var role = (UserType)int.Parse(id[0].ToString());
            if (role == UserType.User)
            {
                var token = CreateJWTToken(id, UserType.User);
                var response = new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    User = _userDBOperations.GetUser(id)
                };

                return Ok(response);
            }
            if (role == UserType.Orgasnisation)
            {
                var token = CreateJWTToken(id, UserType.Orgasnisation);
                var response = new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Organisation = _organisationDBOperations.GetOrganisation(id)
                };
                return Ok(response);
            }
            return BadRequest();
        }

        private JwtSecurityToken CreateJWTToken(string userId, UserType userType)
        {
            var authClaims = new List<Claim>
        {
            new Claim("UserId", userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Role", userType.ToString())
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

        [HttpPost]
        [Route("registerUser")]
        public IActionResult RegisterUser([FromBody] object requestBody)
        {
            User? user = JsonConvert.DeserializeObject<User>(requestBody.ToString());
            if (user == null)
                return BadRequest("Invalid request body");

            _userDBOperations.InsertUser(user); 

            var token = CreateJWTToken(user.ID, UserType.User);

            var response = new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                User = user,
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("registerOrganisation")]
        public IActionResult RegisterOrganisation([FromBody] object requestBody)
        {
            Organisation? organisation = JsonConvert.DeserializeObject<Organisation>(requestBody.ToString());
            if (organisation == null)
                return BadRequest("Invalid request body");

            _organisationDBOperations.InsertOrganisation(organisation);

            var token = CreateJWTToken(organisation.ID, UserType.Orgasnisation);

            var response = new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Organisation = organisation,
            };

            return Ok(response);
        }

    }
}
