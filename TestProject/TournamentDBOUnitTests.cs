using System.Data;
using MySql.Data.MySqlClient;
using Portal.DBMethods;
using Portal.DTO;
using Portal.Utils;

namespace TestProject;

public class Tests
{
    MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
    private TournamentDBO _tournamentDBO;

    [SetUp]
    public void Setup()
    {
        _tournamentDBO = new TournamentDBO();
    }

    [Test]
    public void InsertTournament_WhenCalled_InsertsTournamentIntoDatabase()
    {
        var model = new TournamentCreation
        {
            ID = "TestID",
            Name = "TestName",
            OrganisationId = "TestOrgId",
            BoardGameId = "TestGameId",
            Description = "TestDescription",
            Date = DateTime.Now
        };
        DeleteItem("tournament", "id", model.ID);

        _tournamentDBO.InsertTournament(model);

        var insertedTournament = _tournamentDBO.SelectOrganisationsTournaments(model.OrganisationId).FirstOrDefault(t => t.ID == model.ID);
        Assert.IsNotNull(insertedTournament);
        Assert.AreEqual(model.ID, insertedTournament.ID);
        DeleteItem("tournament", "id", model.ID);
    }
    
    [Test]
    public void InsertTournamentMatches_WhenCalled_InsertsMatchesIntoDatabase()
    {
        var tournament = new TournamentCreation
        {
            ID = "TestID",
            Name = "TestName",
            OrganisationId = "TestOrgId",
            BoardGameId = "TestGameId",
            Description = "TestDescription",
            Date = DateTime.Now
        };
        DeleteItem("tournament", "id", tournament.ID);

        _tournamentDBO.InsertTournament(tournament);
        var tournamentMatches = new List<TournamentMatch>
        {
            new TournamentMatch
            {
                ID = "TestMatchID1",
                NextMatchId = "TestNextMatchId1",
                TournamentRoundText = "Round 1",
                State = "TestState1",
                PlayerA = 
                    new TournamentParticipant
                    {
                        ID = "TestParticipantID1",
                        Name = "TestParticipantName1",
                        Points = 10,
                    },
                PlayerB = 
                    new TournamentParticipant
                    {
                        ID = "TestParticipantID2",
                        Name = "TestParticipantName2",
                        Points = 12,
                    }
            },
        };
        DeleteItem("tournament_match", "id", tournamentMatches[0].ID);
        DeleteItem("tournament_player", "id", tournamentMatches[0].PlayerA.ID);
        DeleteItem("tournament_player", "id", tournamentMatches[0].PlayerB.ID);
        _tournamentDBO.InsertTournamentMatches(tournamentMatches, tournament.ID);

        var insertedMatches = _tournamentDBO.SelectTournamentMatches(tournament.ID);
        Assert.AreEqual(tournamentMatches.Count, insertedMatches.Count);

        for (int i = 0; i < tournamentMatches.Count; i++)
        {
            var expectedMatch = tournamentMatches[i];
            var actualMatch = insertedMatches[i];
            Assert.AreEqual(expectedMatch.ID, actualMatch.ID);
            Assert.AreEqual(expectedMatch.NextMatchId, actualMatch.NextMatchId);
            Assert.AreEqual(expectedMatch.TournamentRoundText, actualMatch.TournamentRoundText);
            Assert.AreEqual(expectedMatch.State, actualMatch.State);
        }
        DeleteItem("tournament_match", "id", tournamentMatches[0].ID);
        DeleteItem("tournament_player", "id", tournamentMatches[0].PlayerA.ID);
        DeleteItem("tournament_player", "id", tournamentMatches[0].PlayerB.ID);
        DeleteItem("tournament", "id", tournament.ID);

    }
    [Test]
    public void SelectAllTournaments_WhenCalled_ReturnsAllTournamentsFromDatabase()
    {
        var model = new TournamentCreation
        {
            ID = "TestID",
            Name = "TestName",
            OrganisationId = "0_8aa2d4f624c749628f417948f9d7e85e",
            BoardGameId = "TestGameId",
            Description = "TestDescription",
            Date = DateTime.Now
        };
        DeleteItem("tournament", "id", model.ID);

        _tournamentDBO.InsertTournament(model);
        
        var model2 = new TournamentCreation
        {
            ID = "TestID2",
            Name = "TestName",
            OrganisationId = "0_8aa2d4f624c749628f417948f9d7e85e",
            BoardGameId = "TestGameId",
            Description = "TestDescription",
            Date = DateTime.Now
        };
        DeleteItem("tournament", "id", model2.ID);

        _tournamentDBO.InsertTournament(model2);

        var tournaments = _tournamentDBO.SelectAllTournaments();

        Assert.IsTrue(tournaments.Count > 2);
        Assert.IsTrue(tournaments?.Select(t=>t.ID).Contains(model.ID));
        Assert.IsTrue(tournaments?.Select(t=>t.ID).Contains(model2.ID));
    }
    
    [Test]
    public void SelectTournament_WhenCalled_ReturnsTournamentFromDatabase()
    {
        var model = new TournamentCreation
        {
            ID = "TestID",
            Name = "TestName",
            OrganisationId = "0_8aa2d4f624c749628f417948f9d7e85e",
            BoardGameId = "TestGameId",
            Description = "TestDescription",
            Date = DateTime.Now
        };
        DeleteItem("tournament", "id", model.ID);
        _tournamentDBO.InsertTournament(model);

        var tournament = _tournamentDBO.SelectTournament(model.ID);

        Assert.IsNotNull(tournament);
        Assert.AreEqual(model.ID, tournament.ID);
        DeleteItem("tournament", "id", model.ID);
    }

    private void DeleteItem(string tableName, string comparisonColumnName, string comparisonValue)
    {
        conn.Open();
        MySqlCommand cmd = conn.CreateCommand();
        cmd.Connection = conn;
        var insertQuery = $"DELETE FROM {tableName} WHERE {comparisonColumnName}=@comparisonValue";
        cmd.CommandText = insertQuery;
        cmd.Parameters.Add("@comparisonValue", MySqlDbType.VarChar).Value = comparisonValue;
        cmd.ExecuteNonQuery();
        conn.Close();
    }
}