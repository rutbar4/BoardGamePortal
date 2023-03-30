using Portal.Utils;
using System.Data;

namespace Portal.DTO
{
    public class Organisation
    {

        public string ID { get; set; } = GuidUtils.GenerateGUID();
        public string? Name { get; set; }
    }
}
