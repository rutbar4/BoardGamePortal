using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using Portal.DTO;
using Portal.Models;
using Portal.Utils;
using System.Data;
using System.Xml.Linq;

namespace Portal.Controllers
{
    [Route("api/BoardGamePlay")]
    [ApiController]
    public class BoardGamePlayController : ControllerBase
    {
        MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private const string _played_game_table = "played_game";
        private const string _board_game_table = "board_game";
        private const string _organisation_table = "organisation";
        private const string _board_game_player_table = "board_game_player";

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterPlay([FromBody] object requestBody)
        {
            BoardGamePlayData? boardGamePlayData = JsonConvert.DeserializeObject<BoardGamePlayData>(requestBody.ToString());
            if (boardGamePlayData == null)
                return BadRequest("Invalid request body");
            string bgId = GetBGId(boardGamePlayData);

            var players = new BoardGamePlayers(boardGamePlayData.BoardGameName, boardGamePlayData.Players, boardGamePlayData.ID);

            InsertBGPlayers(players);
            InsertBGPlayData(boardGamePlayData, bgId);

            return Ok();
        }

        [HttpGet]
        [Route("GetAllOrganisationsNames")]
        public IActionResult GetAllOrganisations()
        {
            var organisations = GetAllOrganisationsNames();

            return Ok(organisations);
        }

        [HttpGet]
        [Route("GetBGByOrganisation/{organisationName}")]
        public IActionResult GetBoardGameByOrganisation(string organisationName)
        {
            var boardGames = GetAllBoardGamesByOrganisationName(organisationName);

            return Ok(boardGames);
        }

        private string GetBGId(BoardGamePlayData boardGamePlayData)
        {
            var organisation = GetOrganisation(boardGamePlayData.Organisation);
            var bg = GetBG(organisation.ID, boardGamePlayData.BoardGameName);
            return bg.ID;
        }

        private void InsertBGPlayData(BoardGamePlayData model, string boardGameId)
        {
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;

            var insertQuery = $"INSERT INTO {_played_game_table} SET id=@id, fk_boardGameId=@boardgamename, playerWinner=@playerWinner, playTime=@timePlayed, " +
                $"winnerPoints=@winnerpoints";
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = model.ID;
            cmd.Parameters.Add("@boardgamename", MySqlDbType.VarChar).Value = boardGameId;
            cmd.Parameters.Add("@playerWinner", MySqlDbType.VarChar).Value = model.Winner;
            cmd.Parameters.Add("@timePlayed", MySqlDbType.DateTime).Value = new DateTime(1000,1,1,int.Parse(model.Time_h), int.Parse(model.Time_m), 0);
            cmd.Parameters.Add("@winnerpoints", MySqlDbType.Int32).Value = model.WinnerPoints;
            cmd.ExecuteNonQuery();

            conn.Close();
        }
        private void InsertBGPlayers(BoardGamePlayers players)
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.Connection = conn;

            if (players.Players is null) 
                return;

            foreach (BGPlayer player in players.Players)
            {
                string insertQuery = $"INSERT INTO {_board_game_player_table} SET id=@id, nickname=@nickname, fk_playedGameId=@fk_playedGameId";
                cmd.CommandText = insertQuery;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = player.ID;
                cmd.Parameters.Add("@nickname", MySqlDbType.VarChar).Value = player.Nickname;
                cmd.Parameters.Add("@fk_playedGameId", MySqlDbType.VarChar).Value = players.PlayedGameId;
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
        private List<BoardGamePlayData>? GetBGPlayData(string? id)
        {
            if (id is null)
                return null;

            var sqlCmd = $"SELECT * FROM {_played_game_table} WHERE fk_masterAccountId=@userId";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@userId", MySqlDbType.VarChar).Value = id;

            var dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count == 0)
                return new List<BoardGamePlayData>();

            return dt.AsEnumerable().ToList().Select(row => new BoardGamePlayData
            {
                ID = (string)row["id"],
                BoardGameName = (string)row["fk_boardGameId"],
                Winner = (string)row["playerWinner"],
                Time_m = DBUtils.ConvertFromDBVal<DateTime>(row["playTime"]).Date.Hour.ToString(),
                Time_h = DBUtils.ConvertFromDBVal<DateTime>(row["playTime"]).Date.Minute.ToString(),
                WinnerPoints = DBUtils.ConvertFromDBVal<int>(row["winnerPoints"])
            }).ToList();
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
            return new Organisation
            {
                ID = (string)row["id"],
                Name = (string)row["name"]
            };
        }
        private string[] GetAllOrganisationsNames()
        {
            var sqlCmd = $"SELECT name FROM {_organisation_table}";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            
            var dt = new DataTable();
            da.Fill(dt);
            var names = dt.AsEnumerable().Select(r => (string)r["name"]).ToArray();

            return names;
        }
        private string[] GetAllBoardGamesByOrganisationName(string organisationName)
        {
            var orgId = GetOrganisationId(organisationName);

            var sqlCmd = $"SELECT * FROM {_board_game_table} WHERE fk_organisationId=@orgId";

            var da = new MySqlDataAdapter(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@orgId", MySqlDbType.VarChar).Value = orgId;

            var dt = new DataTable();
            da.Fill(dt);
            var boardGames = dt.AsEnumerable().Select(r => (string)r["name"]).ToArray();

            return boardGames;
        }
        private BoardGame GetBG(string? organisationid, string boardGameName)
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
                GameType = (string)row["gameType"],
                OrganisationName = boardGameName
            };
        }

        private string GetOrganisationId(string organisationName)
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
            return (string)row["id"];
        }

        private string GetBGId(string? organisationid, string boardGameName)
        {
            if (organisationid is null)
                return null;

            var organisation = GetOrganisation(organisationid);

            var sqlCmd = $"SELECT * FROM {_board_game_table} WHERE fk_organisationId=@organisationid, name=@boardGameName";

            MySqlDataAdapter da = new(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@organisationName", MySqlDbType.VarChar).Value = organisationid;
            da.SelectCommand.Parameters.Add("@boardGameName", MySqlDbType.VarChar).Value = boardGameName;


            DataTable dt = new();
            da.Fill(dt);
            var row = dt.AsEnumerable().FirstOrDefault();
            return (string)row["id"];
        }
    }
}
