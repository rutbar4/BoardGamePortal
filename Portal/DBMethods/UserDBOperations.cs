using MySql.Data.MySqlClient;
using Portal.DTO;
using Portal.Utils;

namespace Portal.DBMethods
{
    public class UserDBOperations
    {
        MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private const string _user_table = "user";
        private const string login_table = "login";

        internal void InsertUser(User model)
        {
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            var insertQuery = $"INSERT INTO {_user_table} SET id=@id, name=@name, username=@username, email=@email, password=@password";
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = model.ID;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = model.Name;
            cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = model.Username;
            cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = model.Email;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = model.Password;
            cmd.ExecuteNonQuery();

            var insertQuery2 = $"INSERT INTO {login_table} SET id=@id2, password=@password, username=@username";
            cmd.CommandText = insertQuery2;
            cmd.Parameters.Add("@id2", MySqlDbType.VarChar).Value = GuidUtils.GenerateGUID();
            cmd.ExecuteNonQuery();

            conn.Close();
        }
    }
}
