using MySql.Data.MySqlClient;
using Portal.DTO;
using Portal.Utils;
using System.Data;

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

            var insertQuery = $"INSERT INTO {_user_table} SET id=@id, name=@name, username=@username, email=@email";
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = model.ID;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = model.Name;
            cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = model.Username;
            cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = model.Email;
            cmd.ExecuteNonQuery();

            var insertQuery2 = $"INSERT INTO {login_table} SET id=@id, password=@password, username=@username";
            cmd.CommandText = insertQuery2;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = model.Password;
            cmd.ExecuteNonQuery();

            conn.Close();
        }
        public User GetUser(string? id)
        {
            if (id is null)
                return null;

            var sqlCmd = $"SELECT * FROM {_user_table} WHERE id=@id";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;

            var dt = new DataTable();
            da.Fill(dt);
            var row = dt.AsEnumerable().FirstOrDefault();
            return new User
            {
                ID = (string)row["id"],
                Name = (string)row["name"],
                Username = (string)row["username"],
                Email = (string)row["email"],
            };
        }
    }
}
