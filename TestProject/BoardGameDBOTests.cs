using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Portal.DBMethods;
using Portal.DTO;

namespace TestProject;

public class BoardGameDBOTests
{
    MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
    private BoardGameDBOperations _boardGameDBO;
    private UserDBOperations _userDBO;
    private OrganisationDBOperations _organisationsDBO;

    [SetUp]
    public void Setup()
    {
        _userDBO = new UserDBOperations();
        _boardGameDBO = new BoardGameDBOperations(_userDBO);
        _organisationsDBO = new OrganisationDBOperations();
    }
    
    [Test]
    public void GetAllBoardGamesNamesByOrganisationName_WhenCalled_ReturnsBoardGamesNames()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);
        var boardGame = new BoardGame
        {
            ID = "TestBoardGameId",
            Name = "TestBoardGameName",
            Description = "TestDescription",
            OrganisationId = org.ID
        };

        DeleteItem("board_game", "id", boardGame.ID);
        _boardGameDBO.InsertBGOfOrganisation(boardGame);

        var boardGamesNames = _boardGameDBO.GetAllBoardGamesNamesByOrganisationName(org.Name);

        Assert.IsNotNull(boardGamesNames);
        Assert.AreEqual("TestBoardGameName", boardGamesNames.Where(name=>name == boardGame.Name).FirstOrDefault());
        
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        DeleteItem("board_game", "id", boardGame.ID);

    }
    
     [Test]
    public void GetAllOrganisationsNames_WhenCalled_ReturnsAllOrganisationsNames()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);
        var organisationNames = _boardGameDBO.GetAllOrganisationsNames();

        Assert.IsNotNull(organisationNames);
        Assert.That(organisationNames, Contains.Item(org.Name));
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
    }

    [Test]
    public void GetAllOrganisations_WhenCalled_ReturnsAllOrganisations()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword",
            Description = "Test"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);

        var organisations = _boardGameDBO.GetAllOrganisations();

        Assert.IsNotNull(organisations);
        Assert.That(organisations.Any(o => o.Name == org.Name));
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
    }

    [Test]
    public void GetBGbyOrgIdAndBGName_WhenCalledWithValidOrgIdAndBGName_ReturnsCorrectBoardGame()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);
        var bgModel = new BoardGame
        {
            ID = "TestBoardGameId",
            Name = "TestBoardGameName",
            Description = "TestDescription",
            OrganisationId = org.ID
        };

        DeleteItem("board_game", "id", bgModel.ID);
        _boardGameDBO.InsertBGOfOrganisation(bgModel);

        var boardGame = _boardGameDBO.GetBGbyOrgIdAndBGName(org.ID, bgModel.Name);

        Assert.IsNotNull(boardGame);
        Assert.AreEqual("TestBoardGameName", boardGame.Name);
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        DeleteItem("board_game", "id", bgModel.ID);
    }

    [Test]
    public void GetBGByBDId_WhenCalledWithValidId_ReturnsCorrectBoardGame()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);
        var bgModel = new BoardGame
        {
            ID = "TestBoardGameId",
            Name = "TestBoardGameName",
            Description = "TestDescription",
            OrganisationId = org.ID
        };

        DeleteItem("board_game", "id", bgModel.ID);
        _boardGameDBO.InsertBGOfOrganisation(bgModel);

        var boardGame = _boardGameDBO.GetBGByBDId(bgModel.ID);

        Assert.IsNotNull(boardGame);
        Assert.AreEqual(bgModel.ID, boardGame.ID); 
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        DeleteItem("board_game", "id", bgModel.ID);
    }
    [Test]
    public void GetAllBGByOrganisation_WhenCalledWithValidData_ReturnsListOfBoardGames()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);
        var bgModel = new BoardGame
        {
            ID = "TestBoardGameId",
            Name = "TestBoardGameName",
            Description = "TestDescription",
            OrganisationId = org.ID
        };

        DeleteItem("board_game", "id", bgModel.ID);
        _boardGameDBO.InsertBGOfOrganisation(bgModel);

        var boardGames = _boardGameDBO.GetAllBGByOrganisation(org.ID);

        Assert.IsNotNull(boardGames);
        Assert.AreEqual(1, boardGames.Count);
        Assert.True(boardGames.Any(b=> b.ID == bgModel.ID));
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        DeleteItem("board_game", "id", bgModel.ID);
    }
    
    [Test]
    public void GetAllBGByUserId_WhenUserIdIsNull_ShouldReturnNull()
    {
        // Arrange
        string userId = null;

        // Act
        var result = _boardGameDBO.GetAllBGByUserId(userId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void GetAllBoardGamesNamesByUserID_WhenUserIdIsProvided_ShouldReturnBoardGames()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);
        var bgModel = new BoardGame
        {
            ID = "TestBoardGameId",
            Name = "TestBoardGameName",
            Description = "TestDescription",
            OrganisationId = org.ID
        };

        DeleteItem("board_game", "id", bgModel.ID);
        _boardGameDBO.InsertBGOfOrganisation(bgModel);

        // Act
        var result = _boardGameDBO.GetAllBoardGamesNamesByUserID(org.ID);

        Assert.AreEqual(bgModel.Name, result[0]);
        
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        DeleteItem("board_game", "id", bgModel.ID);
    }
    
    [Test]
    public void DeleteBGOfOrganisation_WhenBoardGameIdIsProvided_ShouldNotThrowException()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);
        var bgModel = new BoardGame
        {
            ID = "TestBoardGameId",
            Name = "TestBoardGameName",
            Description = "TestDescription",
            OrganisationId = org.ID
        };

        DeleteItem("board_game", "id", bgModel.ID);
        _boardGameDBO.InsertBGOfOrganisation(bgModel);

        Assert.DoesNotThrow(() => _boardGameDBO.DeleteBGOfOrganisation(bgModel.ID));
        
        var result = _boardGameDBO.GetAllBoardGamesNamesByUserID(org.ID);

        Assert.AreEqual(0, result.Length);
        
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        DeleteItem("board_game", "id", bgModel.ID);
    }
    
    [Test]
    public void GetBGId_WhenCalled_ShouldReturnBGId()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);
        var bgModel = new BoardGame
        {
            ID = "TestBoardGameId",
            Name = "TestBoardGameName",
            Description = "TestDescription",
            OrganisationId = org.ID
        };

        DeleteItem("board_game", "id", bgModel.ID);
        _boardGameDBO.InsertBGOfOrganisation(bgModel);
        
        var result = _boardGameDBO.GetBGId(new BoardGamePlayData{BoardGameName = bgModel.Name, Organisation = org.Name});
        
        Assert.IsNotNull(result);
        
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        DeleteItem("board_game", "id", bgModel.ID);
    }

    [Test]
    public void DeleteBGOfOrganisation_WhenCalledWithValidId_ShouldNotThrow()
    {
        Organisation org = new Organisation
        {
            ID = "TestOrgID",
            Name = "TestOrgName",
            Username = "TestUsername",
            Email = "testEmail",
            City = "testCity",
            Address = "testAddress",
            Password = "TestPassword"
        };
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        _organisationsDBO.InsertOrganisation(org);
        var bgModel = new BoardGame
        {
            ID = "TestBoardGameId",
            Name = "TestBoardGameName",
            Description = "TestDescription",
            OrganisationId = org.ID
        };
        DeleteItem("board_game", "id", bgModel.ID);
        _boardGameDBO.InsertBGOfOrganisation(bgModel);

        Assert.DoesNotThrow(() => _boardGameDBO.DeleteBGOfOrganisation(bgModel.ID));
        
        var result = _boardGameDBO.GetAllBoardGamesNamesByUserID(org.ID);

        Assert.AreEqual(0, result.Length);
        
        DeleteItem("organisation", "id", org.ID);
        DeleteItem("login", "id", org.ID);
        DeleteItem("board_game", "id", bgModel.ID);
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