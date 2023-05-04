using Portal.Utils;

namespace Portal.DTO
{
    public class TournamentParticipant
    {
        public string ID { get; set; } = GuidUtils.GenerateGUID();
        public string Name { get; set; }
        public string ResultText { get; set; } = "";
        public int? Points { get; set; } = null;
    }
}
