using MySql.Data.MySqlClient;
using Portal.DTO;
using System.Data;

namespace Portal.DBMethods
{
    public class UserDBOperations
    {
        private MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private const string _user_table = "user";
        private const string _login_table = "login";

        public void InsertUser(User model)
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

            var insertQuery2 = $"INSERT INTO {_login_table} SET id=@id, password=@password, username=@username";
            cmd.CommandText = insertQuery2;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = model.Password;
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public User GetUserById(string? id)
        {
            if (id is null)
                return null;

            var sqlCmd = $"SELECT * FROM {_user_table} WHERE id=@id";
            var sqlCmdpass = $"SELECT * FROM {_login_table} WHERE id=@id";

            var da = new MySqlDataAdapter(sqlCmd, conn);
            var dapass = new MySqlDataAdapter(sqlCmdpass, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;

            dapass.SelectCommand.CommandType = CommandType.Text;
            dapass.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;

            var dt = new DataTable();
            var dtpass = new DataTable();
            da.Fill(dt);
            dapass.Fill(dtpass);
            var row = dt.AsEnumerable().FirstOrDefault();
            var rowpass = dtpass.AsEnumerable().FirstOrDefault();
            if (row is null || rowpass is null)
                return null;
            return new User
            {
                ID = (string)row["id"],
                Name = (string)row["name"],
                Username = (string)row["username"],
                Email = (string)row["email"],
                Password = (string)rowpass["password"],
            };
        }

        public string? GetUserIDByUserName(string? username)
        {
            if (username is null)
                return null;

            var sqlCmd = $"SELECT * FROM {_user_table} WHERE username=@username";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;

            var dt = new DataTable();
            da.Fill(dt);

            var row = dt.AsEnumerable().FirstOrDefault();
            if (row is null) return null;

            return (string)row["id"];
        }

        public bool UserExistsByUserName(string? username)
        {
            if (username is null)
                return false;

            var sqlCmd = $"SELECT * FROM {_user_table} WHERE username=@username";
            var sqlCmdpass = $"SELECT * FROM {_login_table} WHERE username=@username";

            var da = new MySqlDataAdapter(sqlCmd, conn);
            var dapass = new MySqlDataAdapter(sqlCmdpass, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;

            dapass.SelectCommand.CommandType = CommandType.Text;
            dapass.SelectCommand.Parameters.Add("@username", MySqlDbType.VarChar).Value = username;

            var dt = new DataTable();
            var dtpass = new DataTable();
            da.Fill(dt);
            dapass.Fill(dtpass);

            var row = dt.AsEnumerable().FirstOrDefault();
            if (row is null) return false;
            var rowpass = dtpass.AsEnumerable().FirstOrDefault();
            if (rowpass is null) return false;

            return true;
        }

        public void UpdateUser(User? user)
        {
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            if (user is null)
                throw new Exception("Organisation was null");

            var updateQuery = $"UPDATE {_user_table} SET name=@name, email=@email WHERE id=@id";
            cmd.CommandText = updateQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = user.ID;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = user.Name;
            cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = user.Email;
            cmd.ExecuteNonQuery();

            var updatePass = $"UPDATE {_login_table} SET password=@password WHERE id=@id";
            cmd.CommandText = updatePass;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = user.Password;
            cmd.ExecuteNonQuery();

            conn.Close();
        }
    }
}