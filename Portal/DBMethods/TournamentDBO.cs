using MySql.Data.MySqlClient;
using Portal.DTO;
using Portal.Utils;
using System.Data;

namespace Portal.DBMethods
{
    public class TournamentDBO
    {
        MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
        private readonly BoardGameDBOperations _boardGameDBOperations;
        private const string _tournament_table = "tournament";
        private const string _tournament_match_table = "tournament_match";
        private const string _tournament_player_table = "tournament_player";

        internal void InsertTournament(TournamentCreation model)
        {
            using MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
            c.Open();
            MySqlCommand cmd = c.CreateCommand();
            cmd.Connection = c;

            if (model.Name is null) throw new Exception("Name is not valid");

            var insertQuery = $"INSERT INTO {_tournament_table} SET id=@id, name=@name, date=@date, fk_organisationId=@organisationId, fk_boardGame=@boardGameId, description=@description";
            cmd.CommandText = insertQuery;
            cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = model.ID;
            cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = model.Name;
            cmd.Parameters.Add("@date", MySqlDbType.DateTime).Value = model.Date.Value.AddHours(3);
            cmd.Parameters.Add("@organisationId", MySqlDbType.VarChar).Value = model.OrganisationId;
            cmd.Parameters.Add("@boardGameId", MySqlDbType.VarChar).Value = model.BoardGameId;
            cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = model.Description;
            cmd.ExecuteNonQuery();
            c.Close();
        }

        internal void InsertTournamentMatches(List<TournamentMatch> tournamentMatches, string tournamentId)
        {
            using MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
            c.Open();

            foreach (TournamentMatch tournamentMatch in tournamentMatches)
            {
                MySqlCommand cmd = c.CreateCommand();
                cmd.Connection = c;

                var insertQuery = $"INSERT INTO {_tournament_match_table} SET id=@id, fk_tournamentId=@tournamentId, fk_nextMatchId=@nextMatchId, tournament_round=@tournament_round, state=@state";
                cmd.CommandText = insertQuery;
                cmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = tournamentMatch.ID;
                cmd.Parameters.Add("@tournamentId", MySqlDbType.VarChar).Value = tournamentId;
                cmd.Parameters.Add("@nextMatchId", MySqlDbType.VarChar).Value = tournamentMatch.NextMatchId;
                cmd.Parameters.Add("@tournament_round", MySqlDbType.VarChar).Value = tournamentMatch.TournamentRoundText;
                cmd.Parameters.Add("@state", MySqlDbType.VarChar).Value = tournamentMatch.State;
                cmd.ExecuteNonQuery();

                foreach (TournamentParticipant tournamentParticipant in tournamentMatch.participants)
                {
                    var participantInsertCmd = c.CreateCommand();
                    participantInsertCmd.Connection = c;

                    insertQuery = $"INSERT INTO {_tournament_player_table} SET id=@id, fk_tournamentId=@tournamentId, name=@name, points=@points, fk_tournamentMatchId=@tournamentMatchId";
                    participantInsertCmd.CommandText = insertQuery;
                    participantInsertCmd.Parameters.Add("@id", MySqlDbType.VarChar).Value = tournamentParticipant.ID;
                    participantInsertCmd.Parameters.Add("@tournamentId", MySqlDbType.VarChar).Value = tournamentId;
                    participantInsertCmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = tournamentParticipant.Name;
                    participantInsertCmd.Parameters.Add("@points", MySqlDbType.Int32).Value = tournamentParticipant.Points;
                    participantInsertCmd.Parameters.Add("@tournamentMatchId", MySqlDbType.VarChar).Value = tournamentMatch.ID;
                    participantInsertCmd.ExecuteNonQuery();
                }
            }


            c.Close();
        }

        internal List<Tournament> SelectOrganisationsTournaments(string organisationId)
        {
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                List<Tournament> tournaments;
                var selectTournamentMatchesCmd = $"SELECT * FROM {_tournament_table} WHERE fk_organisationId=@organisationId";
                using (MySqlDataAdapter da = new(selectTournamentMatchesCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@organisationId", MySqlDbType.VarChar).Value = organisationId;

                    DataTable dt = new();
                    da.Fill(dt);
                    tournaments = dt.AsEnumerable().Select(row => new Tournament
                    {
                        ID = (string)row["id"],
                        Name = (string)row["name"],
                        BoardGameId = DBUtils.ConvertFromDBVal<string>(row["fk_boardGame"]),
                        Date = DBUtils.ConvertFromDBVal<DateTime>(row["date"]),
                        Description = DBUtils.ConvertFromDBVal<string>(row["description"])
                    }).ToList();
                }

                c.Close();
                return tournaments;
            }
        }

        internal Tournament SelectTournament(string tournamentId)
        {
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                Tournament tournament;
                var selectTournamentMatchesCmd = $"SELECT * FROM {_tournament_table} WHERE id=@tournamentId";
                using (MySqlDataAdapter da = new(selectTournamentMatchesCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@tournamentId", MySqlDbType.VarChar).Value = tournamentId;

                    DataTable dt = new();
                    da.Fill(dt);
                    tournament = dt.AsEnumerable().Select(row => new Tournament
                    {
                        ID = (string)row["id"],
                        Name = (string)row["name"],
                        BoardGameId = DBUtils.ConvertFromDBVal<string>(row["fk_boardGame"]),
                        Date = DBUtils.ConvertFromDBVal<DateTime>(row["date"]),
                        Description = DBUtils.ConvertFromDBVal<string>(row["description"])
                    }).First();
                }

                c.Close();

                tournament.matches = SelectTournamentMatches(tournamentId);
                return tournament;
            }
        }

        internal List<TournamentMatch> SelectTournamentMatches(string tournamentId)
        {
            using (MySqlConnection c = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;"))
            {
                c.Open();
                List<TournamentMatch> tournamentMatches;

                var selectTournamentMatchesCmd = $"SELECT * FROM {_tournament_match_table} WHERE fk_tournamentId=@tournamentId";
                using (MySqlDataAdapter da = new(selectTournamentMatchesCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@tournamentId", MySqlDbType.VarChar).Value = tournamentId;

                    DataTable dt = new();
                    da.Fill(dt);
                    tournamentMatches = dt.AsEnumerable().Select(row => new TournamentMatch
                    {
                        ID = (string)row["id"],
                        NextMatchId = DBUtils.ConvertFromDBVal<string>(row["fk_nextMatchId"]),
                        TournamentRoundText = (string)row["tournament_round"],
                        State = DBUtils.ConvertFromDBVal<string>(row["state"])
                    }).ToList();
                }

                var selectTournamentParticipantsCmd = $"SELECT * FROM {_tournament_player_table} WHERE fk_tournamentId=@tournamentId";
                using (MySqlDataAdapter da = new(selectTournamentParticipantsCmd, c))
                {
                    da.SelectCommand.CommandType = CommandType.Text;
                    da.SelectCommand.Parameters.Add("@tournamentId", MySqlDbType.VarChar).Value = tournamentId;

                    DataTable dt = new();
                    da.Fill(dt);
                    foreach (var row in dt.AsEnumerable())
                    {
                        var player = new TournamentParticipant
                        {
                            ID = (string)row["id"],
                            Name = (string)row["name"],
                            Points = DBUtils.ConvertFromDBVal<int>(row["points"]),
                            ResultText = DBUtils.ConvertFromDBVal<string>(row["ResultText"])
                        };
                        var match = tournamentMatches.Where(match => match.ID == (string)row["fk_tournamentMatchId"]).First();
                        
                        if(match.PlayerA == null)
                            match.PlayerA = player;
                        else
                            match.PlayerB = player;
                    }

                }
                c.Close();
                return tournamentMatches;
            }
        }
    }
}
