using MySql.Data.MySqlClient;
using Portal.DTO;
using Portal.Utils;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace Portal.DBMethods
{
    public class BoardGamePlayDBOperations
    {
        MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private const string _played_game_table = "played_game";
        private const string _board_game_table = "board_game";
        private const string _organisation_table = "organisation";
        private const string _board_game_player_table = "board_game_player";
        private const string _played_game = "played_game";

        public List<BoardGamePlayData> GetBGPlayByBgIdWithPlayersCount(string? boardGameid)
        {
            if (boardGameid is null)
                return null;

            var sqlCmd = $"SELECT * FROM {_played_game} WHERE fk_boardGameId=@fk_boardGameId";

            MySqlDataAdapter da = new(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@fk_boardGameId", MySqlDbType.VarChar).Value = boardGameid;

            DataTable dt = new();
            da.Fill(dt);

            var row = dt.AsEnumerable().Where(n => n is not null).ToList();
            var t = row.Where(r=> r is not null).Select(r => new BoardGamePlayData
            {
                ID = (string)r["id"],
                Winner = (string)r["playerWinner"],
                PlayersCount = GetPlayersCountByPlayedGameID((string)r["id"]),
                BoardGameID = (string)r["fk_boardGameId"],
                Time_m = DBUtils.ConvertFromDBVal<DateTime>(r["playTime"]).Minute.ToString(),
                Time_h = DBUtils.ConvertFromDBVal<DateTime>(r["playTime"]).Hour.ToString(),
                WinnerPoints = (int)r["winnerPoints"],
                DatePlayed = DBUtils.ConvertFromDBVal<DateTime>(r["datePlayed"]).Date,
            }).ToList();
            if (t.Count ==0) return null;
            var tt = t.Select(b=> new BoardGamePlayData
            {
                ID = b.ID,
                BoardGameName = GetBGByBGId(b.BoardGameID).Name,
                BoardGameType = GetBGByBGId(b.BoardGameID).GameType,
                BoardGameID = b.BoardGameID,
                Winner = b.Winner,
                PlayersCount = b.PlayersCount,
                Time_m = b.Time_m,
                Time_h = b.Time_h,
                WinnerPoints = b.WinnerPoints,
                DatePlayed = b.DatePlayed,
            }).ToList();


            //new BoardGamePlayData
            //{
            //    ID = (string)s["id"],
            //    BoardGameName = (string)rowBoardGame["name"],
            //    BoardGameType = (string)rowBoardGame["gameType"],
            //    Winner = (string)row["playerWinner"],
            //    PlayersCount = (int)playersCount,
            //    Time_m = DBUtils.ConvertFromDBVal<DateTime>(s["playTime"]).Minute.ToString(),
            //    Time_h = DBUtils.ConvertFromDBVal<DateTime>(s["playTime"]).Hour.ToString(),
            //    WinnerPoints = (int)s["winnerPoints"],
            //    DatePlayed = DBUtils.ConvertFromDBVal<DateTime>(s["datePlayed"]).Date,
            //};
            //// if (row is null || row.Equals("")) return null;
            //var playedGameID = (string)row["id"];

            //var playersCount = GetPlayersCountByPlayedGameID(playedGameID);

            return tt;
        }
        public BoardGame GetBGByBGId(string boardGameid)
        {
            var sqlCmdBoardGame = $"SELECT * FROM {_board_game_table} WHERE id=@id";

            MySqlDataAdapter daBoardGame = new(sqlCmdBoardGame, conn);

            daBoardGame.SelectCommand.CommandType = CommandType.Text;
            daBoardGame.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = boardGameid;

            DataTable dtBoardGame = new();
            daBoardGame.Fill(dtBoardGame);
            var y = dtBoardGame.AsEnumerable().FirstOrDefault();

            if (y is null) return null;

            return new BoardGame
            {
                ID = (string)y["id"],
                GameType = (string)y["gameType"],
                Name = (string)y["name"],
                OrganisationId = (string)y["fk_organisationId"],
            };
        }
        public BGPlayer[] GetBGPlayersByBGId(string playedGameId)
        {
            var sqlCmdBoardGame = $"SELECT * FROM {_board_game_player_table} WHERE fk_playedGameId=@fk_playedGameId";

            MySqlDataAdapter daBoardGame = new(sqlCmdBoardGame, conn);

            daBoardGame.SelectCommand.CommandType = CommandType.Text;
            daBoardGame.SelectCommand.Parameters.Add("@fk_playedGameId", MySqlDbType.VarChar).Value = playedGameId;

            DataTable dtBoardGame = new();
            daBoardGame.Fill(dtBoardGame);
            var y = dtBoardGame.AsEnumerable();

            if (y is null) return null;
            var res = y.Select(u => new BGPlayer { ID = (string)u["id"], Nickname = (string)u["nickname"], }).ToArray();
            return res;
        }

        private int GetPlayersCountByPlayedGameID(string gamePlayId)
        {
            var sqlCmd = $"SELECT * FROM {_board_game_player_table} WHERE fk_playedGameId=@fk_playedGameId";

            MySqlDataAdapter da = new(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@fk_playedGameId", MySqlDbType.VarChar).Value = gamePlayId;

            DataTable dt = new();
            da.Fill(dt);

            var t = dt.AsEnumerable().Count();
            return t;
        }
    }
}
