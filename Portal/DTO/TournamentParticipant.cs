namespace Portal.DTO
{
    public class TournamentParticipant
    {
        public string Name { get; set; }
        public string ResultText { get; set; } = "Win";
        public bool IsWinner { get; set; } = false;
        public double Points { get; set; } = 0;
    }
}
