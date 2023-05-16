using MySql.Data.MySqlClient;
using Portal.DBMethods;
using Portal.DTO;

namespace TestProject;

public class OrganisationDBOTests
{
    MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
    private OrganisationDBOperations _dbOperations;
    private Organisation _testOrg;

    [SetUp]
    public void Setup()
    {
        _dbOperations = new OrganisationDBOperations();

        _testOrg = new Organisation
        {
            ID = "testID",
            Name = "testName",
            Username = "unitTestUsername",
            Email = "testEmail@example.com",
            Address = "testAddress",
            City = "testCity",
            Password = "testPassword",
            Description = "testDescription"
        };

        _dbOperations.InsertOrganisation(_testOrg);
    }

    [Test]
    public void InsertOrganisation_ValidOrganisation_InsertsSuccessfully()
    {
        var newOrg = new Organisation
        {
            ID = "newID",
            Name = "newName",
            Username = "newUsername",
            Email = "newEmail@example.com",
            Address = "newAddress",
            City = "newCity",
            Password = "newPassword",
            Description = "newDescription"
        };

        _dbOperations.InsertOrganisation(newOrg);
        
        var insertedOrg = _dbOperations.GetOrganisation(newOrg.ID);
        Assert.AreEqual(newOrg.ID, insertedOrg.ID);
    }

    [Test]
    public void GetOrganisation_ValidId_ReturnsCorrectOrganisation()
    {
        var org = _dbOperations.GetOrganisation(_testOrg.ID);
        Assert.AreEqual(_testOrg.ID, org.ID);
    }

    [Test]
    public void GetOrganisationIdByName_ValidName_ReturnsCorrectId()
    {
        var id = _dbOperations.GetOrganisationIdByName(_testOrg.Name);
        Assert.AreEqual(_testOrg.ID, id);
    }

    [Test]
    public void UpdateOrganisation_ValidOrganisation_UpdatesSuccessfully()
    {
        _testOrg.Name = "updatedName";
        _testOrg.Email = "updatedEmail@example.com";
        _dbOperations.UpdateOrganisation(_testOrg);

        var updatedOrg = _dbOperations.GetOrganisation(_testOrg.ID);
        Assert.AreEqual("updatedName", updatedOrg.Name);
        Assert.AreEqual("updatedEmail@example.com", updatedOrg.Email);
    }
    
    [TearDown]
    public void Teardown()
    {
        DeleteItem("organisation", "id", "newID");
        DeleteItem("organisation", "id", _testOrg.ID);
        DeleteItem("login", "id", _testOrg.ID);
        DeleteItem("login", "id", "newID");
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