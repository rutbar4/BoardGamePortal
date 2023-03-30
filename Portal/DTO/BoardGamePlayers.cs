using Portal.Utils;
using System.Security.Policy;

namespace Portal.DTO
{
    public class BoardGamePlayers
    {
        public BGPlayer[]? Players { get; set; }
        public string? BoardGameName { get; set; }
        public string? PlayedGameId { get; set; }

        public BoardGamePlayers(string? boardGameName, string[] players, string? playedGameId)
        {
            BoardGameName = boardGameName;
            Players = players.Select(p => new BGPlayer { Nickname = p}).ToArray();
            PlayedGameId = playedGameId;
        }
    }
    public class BGPlayer
    {
        public string ID { get; set; } = GuidUtils.GenerateGUID();
        public string? Nickname { get; set; }
    }
}
