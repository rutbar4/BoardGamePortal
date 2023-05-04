using Portal.Utils;

namespace Portal.DTO
{
    public class Tournament
    {
        public string ID { get; set; } = GuidUtils.GenerateGUID();
        public string? Name { get; set; }
        public DateTime? Date { get; set; }
        public List<TournamentMatch> matches { get; set; }
        public string? Description { get; set; }
        public string? BoardGameId { get; set; }
        public string? OrgansiationName { get; set; } = null;
    }
}
