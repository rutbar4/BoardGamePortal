using Portal.Utils;

namespace Portal.DTO
{
    public enum GameType
    {
        Classic = 0,
        Cooperative = 1,
        Solo = 2,
        TwoPlayer = 3

    }
    public class BoardGame
    {
        public string ID { get; set; } = GuidUtils.GenerateGUID();
        public string? Name { get; set; }
        public string? OrganisationId { get; set; }
        public string? Description { get; set; }
    }
}
