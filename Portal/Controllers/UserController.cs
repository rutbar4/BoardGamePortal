using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Portal.DTO;
using System.Data;

namespace Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private const string _organisation_table = "organisation";

        [HttpGet]
        public IActionResult GetOrganisationInfo()
        {
            string? userId = HttpContext.Items.First(i => i.Key == "profileId").Value as string;

            var sqlCmd = $"SELECT * FROM {_organisation_table} WHERE id=@id";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = userId;

            var dt = new DataTable();
            da.Fill(dt);

            var row = dt.AsEnumerable().FirstOrDefault();
            var response = new Organisation
            {
                ID = (string)row["id"],
                Name = (string)row["name"]
            };

            return Ok(response);
        }
    }
}
