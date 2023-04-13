using Portal.Utils;

namespace Portal.DTO
{
    public class User
    {
        public string ID { get; set; } = GuidUtils.GenerateGUID();
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
