using Portal.Utils;

namespace Portal.DTO
{
    public class TournamentMatch
    {
        public string ID { get; set; } = GuidUtils.GenerateGUID();
        public string? NextMatchId { get; set; }
        public string? TournamentRoundText { get; set; }
        public string? State { get; set; } = "NOPLAYERS";

        public List<TournamentParticipant> participants
        {
            get
            {
                var participantsList = new List<TournamentParticipant>();
                if (PlayerA != null)
                    participantsList.Add(PlayerA);
                if (PlayerB != null)
                    participantsList.Add(PlayerB);
                return participantsList;
            }
        }

        public TournamentParticipant? PlayerA = null;
        public TournamentParticipant? PlayerB = null;
    }
}