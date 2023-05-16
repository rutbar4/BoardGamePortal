using MySql.Data.MySqlClient;
using Portal.DBMethods;
using Portal.DTO;

namespace TestProject;

public class UserDBOTests
{
    MySqlConnection conn = new MySqlConnection("server=localhost;port=3306;database=board_games_registration_system;username=dev;password=*developeR321;Allow User Variables=True;");
    private UserDBOperations _userDBOperations;
    private User _testUser;

    [SetUp]
    public void Setup()
    {
        _userDBOperations = new UserDBOperations();

        _testUser = new User
        {
            ID = "UnitTestId",
            Name = "Test User",
            Username = "TestUnitUsername",
            Email = "testuser@gmail.com",
            Password = "Test@123",
        };

        // Insert the test user into the database.
        _userDBOperations.InsertUser(_testUser);
    }

    [Test]
    public void InsertUser_ValidUser_UserIsInserted()
    {
        //User is already inserted into database in setup phase
        var retrievedUser = _userDBOperations.GetUserById(_testUser.ID);
        Assert.IsNotNull(retrievedUser);
        Assert.AreEqual(_testUser.Name, retrievedUser.Name);
        Assert.AreEqual(_testUser.Username, retrievedUser.Username);
        Assert.AreEqual(_testUser.Email, retrievedUser.Email);
        Assert.AreEqual(_testUser.Password, retrievedUser.Password);
    }

    [Test]
    public void GetUserById_ValidId_ReturnsUser()
    {
        var retrievedUser = _userDBOperations.GetUserById(_testUser.ID);
        Assert.IsNotNull(retrievedUser);
        Assert.AreEqual(_testUser.Name, retrievedUser.Name);
    }

    [Test]
    public void GetUserById_InvalidId_ReturnsNull()
    {
        var retrievedUser = _userDBOperations.GetUserById("invalid-id");
        Assert.IsNull(retrievedUser);
    }

    [Test]
    public void GetUserIDByUserName_ValidUserName_ReturnsId()
    {
        var id = _userDBOperations.GetUserIDByUserName(_testUser.Username);
        Assert.IsNotNull(id);
        Assert.AreEqual(_testUser.ID, id);
    }

    [Test]
    public void GetUserIDByUserName_InvalidUserName_ReturnsNull()
    {
        var id = _userDBOperations.GetUserIDByUserName("invalid-username");
        Assert.IsNull(id);
    }

    [Test]
    public void UserExistsByUserName_ValidUserName_ReturnsTrue()
    {
        var exists = _userDBOperations.UserExistsByUserName(_testUser.Username);
        Assert.IsTrue(exists);
    }

    [Test]
    public void UserExistsByUserName_InvalidUserName_ReturnsFalse()
    {
        var exists = _userDBOperations.UserExistsByUserName("invalid-username");
        Assert.IsFalse(exists);
    }

    [Test]
    public void UpdateUser_ValidUser_UserIsUpdated()
    {
        _testUser.Name = "Updated Test User";
        _testUser.Email = "updatedemail@gmail.com";
        _userDBOperations.UpdateUser(_testUser);

        var retrievedUser = _userDBOperations.GetUserById(_testUser.ID);
        Assert.IsNotNull(retrievedUser);
        Assert.AreEqual(_testUser.Name, retrievedUser.Name);
        Assert.AreEqual(_testUser.Email, retrievedUser.Email);
    }

    [TearDown]
    public void Teardown()
    {
       DeleteItem("user", "id", _testUser.ID);
       DeleteItem("login", "id", _testUser.ID);
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