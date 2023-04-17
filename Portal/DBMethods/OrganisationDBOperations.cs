using Microsoft.AspNetCore.Mvc.ModelBinding;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Portal.DTO;
using Portal.Utils;
using System.Data;
using System.Xml.Linq;

namespace Portal.DBMethods
{
    public class OrganisationDBOperations
    {
        MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private const string _organisation_table = "organisation";
        private const string login_table = "login";

        internal void InsertOrganisation(Organisation model)
        {
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            if (model.Username is null) throw new Exception("Username is not valid");

            var insertQuery = $"INSERT INTO {_organisation_table} SET id=@id, name=@name, username=@username, email=@email, address=@address, city=@city";
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = model.ID;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = model.Name;
            cmd.Parameters.Add("@username", MySqlDbType.VarChar).Value = model.Username;
            cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = model.Email;
            cmd.Parameters.Add("@address", MySqlDbType.VarChar).Value = model.Address;
            cmd.Parameters.Add("@city", MySqlDbType.VarChar).Value = model.City;
            cmd.ExecuteNonQuery();

            if (model.Password is null) throw new Exception("Password is not valid");

            var insertQuery2 = $"INSERT INTO {login_table} SET id=@id, password=@password, username=@username ";
            cmd.CommandText = insertQuery2;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = model.Password;
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public Organisation GetOrganisation(string? id)
        {
            if (id is null)
                throw new Exception("id was null");

            var sqlCmd = $"SELECT * FROM {_organisation_table} WHERE id=@id";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;

            var dt = new DataTable();
            da.Fill(dt);
            var row = dt.AsEnumerable().FirstOrDefault();
            return new Organisation
            {
                ID = (string)row["id"],
                Name = (string)row["name"],
                Username = (string)row["username"],
                Email = (string)row["email"],
                Address = (string)row["address"],
                City = (string)row["city"],
                Description = DBUtils.ConvertFromDBVal<string>(row["description"]),
            };
        }
        public void UpdateOrganisation(Organisation? organisation)
        {
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            if (organisation is null)
                throw new Exception("Organisation was null");

            var updateQuery = $"UPDATE {_organisation_table} SET name=@name, email=@email, address=@addreess, city=@city, description=@description WHERE id=@id";
            cmd.CommandText = updateQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = organisation.ID;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = organisation.Name;
            cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = organisation.Email;
            cmd.Parameters.Add("@address", MySqlDbType.VarChar).Value = organisation.Address;
            cmd.Parameters.Add("@city", MySqlDbType.VarChar).Value = organisation.City;
            cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = organisation.Description;
            cmd.ExecuteNonQuery();


            var updatePass = $"UPDATE {login_table} SET password=@password WHERE id=@id";
            cmd.CommandText = updatePass;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = organisation.Password;
            cmd.ExecuteNonQuery();

            conn.Close();
        }
    }
}
