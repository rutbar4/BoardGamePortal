using Portal.Utils;
using System.Data;

namespace Portal.DTO
{
    public class Organisation
    {
        public string ID { get; set; } = GuidUtils.GenerateGUID("Org");
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        //public string? Description { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
    }
}
