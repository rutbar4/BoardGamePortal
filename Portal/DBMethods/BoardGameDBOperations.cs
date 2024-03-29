﻿using MySql.Data.MySqlClient;
using Portal.DTO;
using Portal.Utils;
using System.Data;

namespace Portal.DBMethods
{
    public class BoardGameDBOperations
    {
        private readonly MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private const string _played_game_table = "played_game";
        private const string _board_game_table = "board_game";
        private const string _organisation_table = "organisation";
        private const string _user_table = "user";
        private const string _board_game_player_table = "board_game_player";
        private readonly UserDBOperations _userDBOperations;

        public BoardGameDBOperations(UserDBOperations userDBOperations)
        {
            _userDBOperations = userDBOperations;
        }

        internal string[] GetAllBoardGamesNamesByOrganisationName(string organisationName)
        {
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();

                var orgId = GetOrganisationId(organisationName);

                var sqlCmd = $"SELECT * FROM {_board_game_table} WHERE fk_organisationId=@orgId";

                using (MySqlDataAdapter da = new(sqlCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@orgId", MySqlDbType.VarChar).Value = orgId;

                    var dt = new DataTable();
                    da.Fill(dt);
                    var boardGames = dt.AsEnumerable().Select(r => (string)r["name"]).ToArray();

                    return boardGames;
                }
            }
        }

        internal string[] GetAllOrganisationsNames()//perkelti į organizacijų kontrollerį
        {
            var sqlCmd = $"SELECT name FROM {_organisation_table}";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;

            var dt = new DataTable();
            da.Fill(dt);
            var names = dt.AsEnumerable().Select(r => (string)r["name"]).ToArray();

            return names;
        }

        internal Organisation[]? GetAllOrganisations()//perkelti į organizacijų kontrollerį //pabandyt su null
        {
            var sqlCmd = $"SELECT name, description, address, city, email FROM {_organisation_table}";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;

            var dt = new DataTable();
            da.Fill(dt);
            var names = dt.AsEnumerable();
            if (names is null) return null;
            var rez = names
                .Select(r => new Organisation
                {
                    Name = (string)r["name"],
                    Description = DBUtils.ConvertFromDBVal<string>(r["description"]),
                    Address = (string)r["address"],
                    City = (string)r["city"],
                    Email = (string)r["email"],
                })
                .ToArray();

            return rez;
        }

        public BoardGame GetBGbyOrgIdAndBGName(string? organisationid, string boardGameName)
        {
            if (organisationid is null)
                return null;

            var sqlCmd = $"SELECT * FROM {_board_game_table} WHERE fk_organisationId=@organisationid AND name=@boardGameName";

            MySqlDataAdapter da = new(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@organisationid", MySqlDbType.VarChar).Value = organisationid;
            da.SelectCommand.Parameters.Add("@boardGameName", MySqlDbType.VarChar).Value = boardGameName;

            DataTable dt = new();
            da.Fill(dt);
            var row = dt.AsEnumerable().FirstOrDefault();
            return new BoardGame
            {
                ID = (string)row["id"],
                Name = (string)row["name"],
            };
        }

        public BoardGame GetBGByBDId(string? boardGameid)
        {
            if (boardGameid is null)
                return null;

            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();

                var sqlCmd = $"SELECT * FROM {_board_game_table} WHERE id=@id";

                using (MySqlDataAdapter da = new(sqlCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = boardGameid;

                    DataTable dt = new();
                    da.Fill(dt);
                    var row = dt.AsEnumerable().FirstOrDefault();

                    return new BoardGame
                    {
                        ID = (string)row["id"],
                        Name = (string)row["name"],
                        Description = DBUtils.ConvertFromDBVal<string>(row["description"]),
                    };
                }
            }
        }

        public List<BoardGame>? GetAllBGByOrganisation(string? organisationid)
        {
            if (organisationid is null)
                return null;

            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmd = $"SELECT * FROM {_board_game_table} WHERE fk_organisationId=@organisationid";

                using (MySqlDataAdapter da = new(sqlCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@organisationid", MySqlDbType.VarChar).Value = organisationid;

                    DataTable dt = new();
                    da.Fill(dt);
                    var rows = dt.AsEnumerable().Select(row => new BoardGame
                    {
                        ID = (string)row["id"],
                        Name = (string)row["name"],
                        Description = DBUtils.ConvertFromDBVal<string>(row["description"])
                    }).ToList();

                    return rows;
                }
            }
        }

        public List<BoardGame> GetAllBGByOrganisationName(string? organisationName)
        {
            if (organisationName is null)
                return null;

            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmd = $"SELECT * FROM {_board_game_table} WHERE name=@name";

                using (MySqlDataAdapter da = new(sqlCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = organisationName;

                    DataTable dt = new();
                    da.Fill(dt);
                    var rows = dt.AsEnumerable().Select(row => new BoardGame
                    {
                        ID = (string)row["id"],
                        Name = (string)row["name"],
                        Description = DBUtils.ConvertFromDBVal<string>(row["description"])
                    }).ToList();

                    return rows;
                }
            }
        }

        public List<BoardGame> GetAllBGByUserId(string? userid)
        {
            if (userid is null)
                return null;

            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmd = $"SELECT * FROM {_board_game_table} WHERE fk_organisationId=@userid";
                List<BoardGame> rows;
                using (MySqlDataAdapter da = new(sqlCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@userid", MySqlDbType.VarChar).Value = userid;

                    DataTable dt = new();
                    da.Fill(dt);
                    rows = dt.AsEnumerable().Select(row => new BoardGame
                    {
                        ID = (string)row["id"],
                        Name = (string)row["name"],
                    }).ToList();
                }
                return rows;
            }
        }

        internal string[] GetAllBoardGamesNamesByUserID(string userId)
        {
            var sqlCmd = $"SELECT * FROM {_board_game_table} WHERE fk_organisationId=@orgId";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@orgId", MySqlDbType.VarChar).Value = userId;

            var dt = new DataTable();
            da.Fill(dt);
            var boardGames = dt.AsEnumerable().Select(r => (string)r["name"]).ToArray();

            return boardGames;
        }

        internal string GetBGId(BoardGamePlayData boardGamePlayData)
        {
            var organisation = GetOrganisation(boardGamePlayData.Organisation);
            if (organisation is null)
            {
                var user = GetUser(boardGamePlayData.Organisation);
                var bgu = GetBGbyOrgIdAndBGName(user.ID, boardGamePlayData.BoardGameName);
                return bgu.ID;
            }

            var bg = GetBGbyOrgIdAndBGName(organisation.ID, boardGamePlayData.BoardGameName);
            return bg.ID;
        }

        internal string GetBGIdByUser(BoardGamePlayData boardGamePlayData)
        {
            var organisation = GetUser(boardGamePlayData.Organisation);
            var bg = GetBGbyOrgIdAndBGName(organisation.ID, boardGamePlayData.BoardGameName);
            return bg.ID;
        }

        private Organisation GetOrganisation(string? organisationName)
        {
            if (organisationName is null)
                return null;

            var sqlCmd = $"SELECT * FROM {_organisation_table} WHERE name=@organisationName";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@organisationName", MySqlDbType.VarChar).Value = organisationName;

            var dt = new DataTable();
            da.Fill(dt);
            var row = dt.AsEnumerable().FirstOrDefault();
            if (row is null) return null;
            return new Organisation
            {
                ID = (string)row["id"],
                Name = (string)row["name"]
            };
        }

        public User GetUser(string? name)
        {
            if (name is null)
                return null;

            var sqlCmd = $"SELECT * FROM {_user_table} WHERE name=@name";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;

            var dt = new DataTable();
            da.Fill(dt);
            var row = dt.AsEnumerable().FirstOrDefault();
            return new User
            {
                ID = (string)row["id"],
                Name = (string)row["name"]
            };
        }

        private string GetOrganisationId(string organisationName)
        {
            if (organisationName is null)
                return null;
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmd = $"SELECT * FROM {_organisation_table} WHERE name=@organisationName";

                using (MySqlDataAdapter da = new(sqlCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@organisationName", MySqlDbType.VarChar).Value = organisationName;

                    var dt = new DataTable();
                    da.Fill(dt);
                    var row = dt.AsEnumerable().FirstOrDefault();
                    return (string)row["id"];
                }
            }
        }

        internal void DeleteBGOfOrganisation(string boardGameId)
        {
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            var boardGame = GetBGByBDId(boardGameId);
            var insertQuery = $"DELETE FROM {_board_game_table} WHERE id=@id";
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = boardGame.ID;
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        internal void InsertBGOfOrganisation(BoardGame model)
        {
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            var insertQuery = $"INSERT INTO {_board_game_table} SET id=@id, name=@name, description=@description, fk_organisationId=@fk_organisationId";
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = model.ID;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = model.Name;
            cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = model.Description;
            cmd.Parameters.Add("@fk_organisationId", MySqlDbType.VarChar).Value = model.OrganisationId;
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        internal void InsertBGGGameID(BoardGame model)
        {
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            var insertQuery = $"SELECT * FROM {_board_game_table} WHERE id=@id SET boardGameGeeksGameId=@boardGameGeeksGameId";
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = model.ID;
            cmd.Parameters.Add("@boardGameGeeksGameId", MySqlDbType.VarChar).Value = null;
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        internal void InsertBGPlayData(BoardGamePlayData model, string boardGameId)
        {
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            var insertQuery = $"INSERT INTO {_played_game_table} SET id=@id, fk_boardGameId=@boardgamename, playerWinner=@playerWinner, gameType=@gameType, playTime=@timePlayed, datePlayed=@datePlayed, " +
                $"winnerPoints=@winnerpoints";
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = model.ID;
            cmd.Parameters.Add("@boardgamename", MySqlDbType.VarChar).Value = boardGameId;
            cmd.Parameters.Add("@playerWinner", MySqlDbType.VarChar).Value = model.Winner;
            cmd.Parameters.Add("@gameType", MySqlDbType.VarChar).Value = model.BoardGameType;
            cmd.Parameters.Add("@timePlayed", MySqlDbType.DateTime).Value = new DateTime(1000, 1, 1, int.Parse(model.Time_h), int.Parse(model.Time_m), 0);
            cmd.Parameters.Add("@winnerpoints", MySqlDbType.Int32).Value = model.WinnerPoints;
            if (model.DatePlayed is not null)
                cmd.Parameters.Add("@datePlayed", MySqlDbType.DateTime).Value = model.DatePlayed;
            else
                cmd.Parameters.Add("@datePlayed", MySqlDbType.DateTime).Value = DateTime.Today;
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        internal void InsertBGPlayers(BoardGamePlayers players)
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.Connection = conn;

            if (players.Players is null)
                return;

            foreach (BGPlayer player in players.Players)
            {
                string insertQuery = $"INSERT INTO {_board_game_player_table} SET id=@id, nickname=@nickname, fk_playedGameId=@fk_playedGameId, fk_userId=@userId";

                cmd.CommandText = insertQuery;
                cmd.Parameters.Clear();
                if (player.Nickname.Substring(0, 1) == "@")
                {
                    string cleanPlayerUsername = player.Nickname.Substring(1);
                    if (_userDBOperations.UserExistsByUserName(cleanPlayerUsername))
                    {
                        var userId = _userDBOperations.GetUserIDByUserName(cleanPlayerUsername);
                        cmd.Parameters.Add("@userId", MySqlDbType.VarChar).Value = userId;
                        cmd.Parameters.Add("@nickname", MySqlDbType.VarChar).Value = cleanPlayerUsername;
                    }
                }
                else
                {
                    cmd.Parameters.Add("@userId", MySqlDbType.VarChar).Value = null;
                    cmd.Parameters.Add("@nickname", MySqlDbType.VarChar).Value = player.Nickname;
                }
                cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = player.ID;
                cmd.Parameters.Add("@fk_playedGameId", MySqlDbType.VarChar).Value = players.PlayedGameId;

                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }
}