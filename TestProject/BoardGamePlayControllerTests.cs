using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Portal.Controllers;
using Portal.DBMethods;
using Portal.DTO;

namespace TestProject;

public class BoardGamePlayControllerTests
{
    MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");

    private BoardGameDBOperations _boardGameDBO;
    private UserDBOperations _userDBO;
    private OrganisationDBOperations _organisationsDBO;
    private BoardGamePlayDBOperations _bgPlayDBO;
    private BoardGamePlayController _boardGamePlayController;
    
    private Organisation _testOrg;
    private BoardGamePlayData _bgPlayDataInput;
    private BoardGame _testBg;
    
    [SetUp]
    public void Setup()
    {
        _organisationsDBO = new OrganisationDBOperations();
        _userDBO = new UserDBOperations();
        _boardGameDBO = new BoardGameDBOperations(_userDBO);
        _bgPlayDBO = new BoardGamePlayDBOperations();
        _boardGamePlayController = new BoardGamePlayController(_boardGameDBO, _bgPlayDBO, _organisationsDBO);
        
        _testOrg = new Organisation
        {
            ID = "UnitTestOrgID",
            Name = "TestOrgName",
            Username = "UnitOrgTestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", _testOrg.ID);
        DeleteItem("login", "id", _testOrg.ID);
        _organisationsDBO.InsertOrganisation(_testOrg);
        _testBg = new BoardGame
        {
            ID = "UnitTestBoardGameId",
            Name = "TestBoardGameName",
            Description = "TestDescription",
            OrganisationId = _testOrg.ID
        };
        DeleteItem("board_game", "id", _testBg.ID);
        _boardGameDBO.InsertBGOfOrganisation(_testBg);
        
        _bgPlayDataInput = new BoardGamePlayData
        {
            ID = "TestBgPlayId",
            Organisation = _testOrg.Name,
            BoardGameName = _testBg.Name,
            BoardGameID = _testBg.ID,
            BoardGameType = "Classic",
            Players = new[] { "testPlayerA", "testPlayerB" },
            PlayersCount = 2,
            Winner = "testPlayerA",
            Time_m = "02",
            Time_h = "01",
            WinnerPoints = 55,
            DatePlayed = DateTime.Now
        };
        DeleteItem("played_game", "id", _bgPlayDataInput.ID);
        DeleteItem("board_game_player", "fk_playedGameId", _bgPlayDataInput.ID);
        _boardGamePlayController.RegisterPlay(JsonConvert.SerializeObject(_bgPlayDataInput));
    }

    [TearDown]
    public void Teardown()
    {
        DeleteItem("played_game", "id", _bgPlayDataInput.ID);
        DeleteItem("board_game_player", "fk_playedGameId", _bgPlayDataInput.ID);
        DeleteItem("board_game", "id", _testBg.ID);
        DeleteItem("organisation", "id", _testOrg.ID);
        DeleteItem("login", "id", _testOrg.ID);
        DeleteItem("board_game", "id", _testBg.ID);
    }

    [Test]
    public void RegisterPlay_ValidBody_InsertsPlay()
    {
        //data is inserted in setup
        Assert.AreEqual(1, _bgPlayDBO.GetBGPlayCount(_testBg.ID));
    }
    
    [Test]
    public void GetAllPlaysByOrganisaionId_ValidId_ReturnsPlays()
    {
        var response = _boardGamePlayController.GetAllPlaysByOrganisationId(_testOrg.ID);
        var okResult = response as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var plays = okResult.Value as List<BoardGamePlayData>;
        Assert.AreEqual(1, plays.Count);
        Assert.AreEqual(_bgPlayDataInput.ID, plays.First().ID);
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