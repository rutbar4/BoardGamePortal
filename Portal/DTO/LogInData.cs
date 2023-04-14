namespace Portal.DTO
{
    public enum UserType
    {
        Orgasnisation = 0,
        User = 1,
    }
    public class LogInData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
    }
}
