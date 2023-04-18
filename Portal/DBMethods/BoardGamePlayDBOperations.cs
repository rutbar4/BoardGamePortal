using MySql.Data.MySqlClient;
using Portal.DTO;
using Portal.Utils;
using System.Data;

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

        public BoardGamePlayData GetBGPlayByBgIdWithPlayersCount(string? boardGameid)
        {
            if (boardGameid is null)
                return null;

            var sqlCmd = $"SELECT * FROM {_played_game} WHERE fk_boardGameId=@fk_boardGameId";

            MySqlDataAdapter da = new(sqlCmd, conn);

            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("@fk_boardGameId", MySqlDbType.VarChar).Value = boardGameid;

            DataTable dt = new();
            da.Fill(dt);
            var row = dt.AsEnumerable().FirstOrDefault();
            if (row is null || row.Equals("")) return null;

            var sqlCmdBoardGame = $"SELECT * FROM {_board_game_table} WHERE id=@id";

            MySqlDataAdapter daBoardGame = new(sqlCmdBoardGame, conn);

            daBoardGame.SelectCommand.CommandType = CommandType.Text;
            daBoardGame.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = boardGameid;

            DataTable dtBoardGame = new();
            daBoardGame.Fill(dtBoardGame);
            var rowBoardGame = dtBoardGame.AsEnumerable().FirstOrDefault();

            var playedGameID = (string)row["id"];

            var playersCount = GetPlayersCountByPlayedGameID(playedGameID);
            string Time_m = DBUtils.ConvertFromDBVal<DateTime>(row["playTime"]).Hour.ToString();

            return new BoardGamePlayData
            {
                ID = (string)row["id"],
                BoardGameName = (string)rowBoardGame["name"],
                BoardGameType = (string)rowBoardGame["gameType"],
                Winner = (string)row["playerWinner"],
                PlayersCount = (int)playersCount,
                Time_m = DBUtils.ConvertFromDBVal<DateTime>(row["playTime"]).Minute.ToString(),
                Time_h = DBUtils.ConvertFromDBVal<DateTime>(row["playTime"]).Hour.ToString(),
                WinnerPoints = (int)row["winnerPoints"],
                DatePlayed = DBUtils.ConvertFromDBVal<DateTime>(row["datePlayed"]).Date,
            };
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
