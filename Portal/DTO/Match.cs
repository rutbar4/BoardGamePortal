namespace Portal.DTO
{
    public class Match
    {
        public string ID { get; set; }
        public string? NextMatchId { get; set; }
        public string? TournamentRoundText { get; set; }

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
