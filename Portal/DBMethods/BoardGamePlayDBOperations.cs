using MySql.Data.MySqlClient;
using Portal.DTO;
using Portal.Utils;
using System.Data;

namespace Portal.DBMethods
{
    public class BoardGamePlayDBOperations
    {
        private MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        
        private const string _board_game_table = "board_game";
        private const string _board_game_player_table = "board_game_player";
        private const string _played_game = "played_game";

        public List<BoardGamePlayData> GetBGPlayByBgIdWithPlayersCount(string? boardGameid)
        {
            if (boardGameid is null)
                return null;
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmd = $"SELECT * FROM {_played_game} WHERE fk_boardGameId=@fk_boardGameId";
                using (MySqlDataAdapter da = new(sqlCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@fk_boardGameId", MySqlDbType.VarChar).Value = boardGameid;

                    DataTable dt = new();
                    da.Fill(dt);

                    var row = dt.AsEnumerable().Where(n => n is not null).ToList();
                    var t = row.Where(r => r is not null).Select(r => new BoardGamePlayData
                    {
                        ID = (string)r["id"],
                        Winner = (string)r["playerWinner"],
                        BoardGameName = GetBGByBGId((string)r["fk_boardGameId"]).Name,
                        PlayersCount = GetPlayersCountByPlayedGameID((string)r["id"]),
                        BoardGameID = (string)r["fk_boardGameId"],
                        Time_m = DBUtils.ConvertFromDBVal<DateTime>(r["playTime"]).Minute.ToString(),
                        Time_h = DBUtils.ConvertFromDBVal<DateTime>(r["playTime"]).Hour.ToString(),
                        WinnerPoints = (int)r["winnerPoints"],
                        DatePlayed = DBUtils.ConvertFromDBVal<DateTime>(r["datePlayed"]).Date,
                        BoardGameType = DBUtils.ConvertFromDBVal<string>(r["gameType"]),
                        Players = GetBGPlayersUsernamesByPlayId((string)r["id"])
                    }).ToList();

                    return t;
                }
            }
        }

        public List<BoardGamePlayData> GetBGPlayUserIdWithPlayersCount(string? userId)
        {
            if (userId is null)
                return null;
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmdP = $"SELECT board_game.name, played_game.gameType, played_game.playerWinner, played_game.winnerPoints, played_game.playTime, played_game.datePlayed, played_game.id, played_game.fk_boardGameId " +
                    $"FROM `board_game_player` " +
                    $"RIGHT JOIN `played_game` " +
                    $"ON `board_game_player`.`fk_playedGameId`=`played_game`.`id` " +
                    $"RIGHT JOIN `board_game` " +
                    $"ON `board_game`.`id`=`played_game`.`fk_boardGameId` " +
                    $"WHERE fk_userId=@userId";
                using (MySqlDataAdapter da = new(sqlCmdP, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@userId", MySqlDbType.VarChar).Value = userId;

                    DataTable dt = new();
                    da.Fill(dt);
                    var row = dt.AsEnumerable().Where(n => n is not null).ToList();
                    var t = row.Where(r => r is not null).Select(r => new BoardGamePlayData
                    {
                        ID = (string)r["id"],
                        Winner = (string)r["playerWinner"],
                        BoardGameName = (string)r["name"],
                        PlayersCount = GetPlayersCountByPlayedGameID((string)r["id"]),
                        Time_m = DBUtils.ConvertFromDBVal<DateTime>(r["playTime"]).Minute.ToString(),
                        Time_h = DBUtils.ConvertFromDBVal<DateTime>(r["playTime"]).Hour.ToString(),
                        WinnerPoints = (int)r["winnerPoints"],
                        DatePlayed = DBUtils.ConvertFromDBVal<DateTime>(r["datePlayed"]).Date,
                        BoardGameType = DBUtils.ConvertFromDBVal<string>(r["gameType"]),
                        BoardGameID = DBUtils.ConvertFromDBVal<string>(r["fk_boardGameId"]),
                        Players = GetBGPlayersUsernamesByPlayId((string)r["id"])
                    }).ToList();
                    return t;
                }
            }
        }

        public int GetBGPlayCount(string boardGameid)
        {
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmdBoardGame = $"SELECT * FROM {_played_game} WHERE fk_boardGameId=@fk_boardGameId";

                using (MySqlDataAdapter da = new(sqlCmdBoardGame, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@fk_boardGameId", MySqlDbType.VarChar).Value = boardGameid;

                    DataTable dtBoardGame = new();
                    da.Fill(dtBoardGame);
                    var y = dtBoardGame.AsEnumerable().Count();

                    return y;
                }
            }
        }

        public string[] GetTopMonthPlayer(List<BoardGamePlayData> list, DateTime month)
        {
            if (list.Count != 0)
            {
                List<BoardGamePlayData> listByMonth = new(list.Where(s => s.DatePlayed.Value.Month.Equals(month.Month) && s.DatePlayed.Value.Year.Equals(month.Year)));
                if (listByMonth.Count != 0)
                {
                    var result = listByMonth
                        .GroupBy(i => i.Winner)
                        .GroupBy(g => g.Count())
                        .OrderByDescending(g => g.Key)
                        .First()
                        .Select(g => g.Key)
                        .ToArray();

                    return result;
                }
                return null;
            }
            return null;
        }

        public string[] GetTopMonthGame(List<BoardGamePlayData> list, DateTime month)
        {
            if (list.Count != 0)
            {
                List<BoardGamePlayData> listByMonth = new(list.Where(s => s.DatePlayed.Value.Month.Equals(month.Month) && s.DatePlayed.Value.Year.Equals(month.Year)));
                if (listByMonth.Count != 0)
                {
                    var result = listByMonth
                    .GroupBy(i => i.BoardGameName)
                .GroupBy(g => g.Count())
                .OrderByDescending(g => g.Key)
                .First()
                .Select(g => g.Key)
                .ToArray();

                    return result;
                }
                return null;
            }
            return null;
        }

        public BoardGame GetBGByBGId(string boardGameid)
        {
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmdBoardGame = $"SELECT * FROM {_board_game_table} WHERE id=@id";

                using (MySqlDataAdapter daBoardGame = new(sqlCmdBoardGame, c))
                {
                    daBoardGame.SelectCommand.CommandType = CommandType.Text;
                    daBoardGame.SelectCommand.Parameters.Add("@id", MySqlDbType.VarChar).Value = boardGameid;

                    DataTable dtBoardGame = new();
                    daBoardGame.Fill(dtBoardGame);
                    var y = dtBoardGame.AsEnumerable().FirstOrDefault();

                    if (y is null) return null;

                    return new BoardGame
                    {
                        ID = (string)y["id"],
                        Name = (string)y["name"],
                        OrganisationId = (string)y["fk_organisationId"],
                    };
                }
            }
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

        public string[] GetBGPlayersUsernamesByPlayId(string playedGameId)
        {
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmd = $"SELECT * FROM {_board_game_player_table} WHERE fk_playedGameId=@fk_playedGameId";

                using (MySqlDataAdapter da = new(sqlCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@fk_playedGameId", MySqlDbType.VarChar).Value = playedGameId;

                    DataTable dtBoardGame = new();
                    da.Fill(dtBoardGame);
                    var y = dtBoardGame.AsEnumerable();

                    if (y is null) return null;
                    var res = y.Select(u => (string)u["nickname"]).ToArray();
                    return res;
                }
            }
        }

        private int GetPlayersCountByPlayedGameID(string gamePlayId)
        {
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                var sqlCmd = $"SELECT * FROM {_board_game_player_table} WHERE fk_playedGameId=@fk_playedGameId";

                using (MySqlDataAdapter da = new(sqlCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@fk_playedGameId", MySqlDbType.VarChar).Value = gamePlayId;

                    DataTable dt = new();
                    da.Fill(dt);

                    var t = dt.AsEnumerable().Count();
                    return t;
                }
            }
        }
    }
}