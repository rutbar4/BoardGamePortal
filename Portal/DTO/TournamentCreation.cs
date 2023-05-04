using Portal.Utils;

namespace Portal.DTO
{
    public class TournamentCreation
    {
        public string ID { get; set; } = GuidUtils.GenerateGUID();
        public string? Name { get; set; }
        public DateTime? Date { get; set; }
        public List<string>? Players { get; set; }
        public string? Description { get; set; }
        public string? BoardGameId { get; set; }
        public string? OrganisationId { get; set; }
    }
}
