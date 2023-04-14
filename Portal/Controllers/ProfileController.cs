using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Portal.DBMethods;
using Portal.DTO;
using System.Data;

namespace Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private const string _organisation_table = "organisation"; 
        private const string _user_table = "user";

        [HttpGet]
        public IActionResult GetProfileInfo()
        {
            string? userId = HttpContext.Items.First(i => i.Key == "UserId").Value as string;
            string? role = HttpContext.Items.First(i => i.Key == "Role").Value as string;

            if (role == UserType.User.ToString())
            {
                var sqlCmd = $"SELECT * FROM {_user_table} WHERE id=@id";

                var da = new MySqlDataAdapter(sqlCmd, conn);

                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = userId;

                var dt = new DataTable();
                da.Fill(dt);

                var row = dt.AsEnumerable().FirstOrDefault();

                var userDBOperations = new UserDBOperations();
                var response = new
                {
                    User = userDBOperations.GetUser(userId)
                };

                return Ok(response);
            }
            if (role == UserType.Orgasnisation.ToString())
            {
                var sqlCmd = $"SELECT * FROM {_organisation_table} WHERE id=@id";

                var da = new MySqlDataAdapter(sqlCmd, conn);

                da.SelectCommand.CommandType = CommandType.Text;
                da.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = userId;

                var dt = new DataTable();
                da.Fill(dt);

                var row = dt.AsEnumerable().FirstOrDefault();

                var organisationDBOperations = new OrganisationDBOperations();
                var response = new
                {
                    Organisation = organisationDBOperations.GetOrganisation(userId)
                };

                return Ok(response);
            }
            return Forbid();
        }
    }
}
