using Portal.Utils;

namespace Portal.DTO
{
    public class BoardGamePlayData
    {
        public string ID { get; set; } = GuidUtils.GenerateGUID();
        public string? Organisation { get; set; }
        public string? BoardGameName { get; set; }
        public string? BoardGameType { get; set; }
        public string[]? Players { get; set; }
        public string? Winner { get; set; }
        public string? Time_m { get; set; }
        public string? Time_h { get; set; }
        public int? WinnerPoints { get; set; }
    }
}
