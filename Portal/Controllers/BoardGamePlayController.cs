using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portal.DBMethods;
using Portal.DTO;

namespace Portal.Controllers
{
    [Route("api/BoardGamePlay")]
    [ApiController]
    public class BoardGamePlayController :ControllerBase
    {
        private readonly BoardGameDBOperations _boardGameDBOperations;

        public BoardGamePlayController(BoardGameDBOperations boardGameDBOperations)
        {
            _boardGameDBOperations = boardGameDBOperations;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterPlay([FromBody] object requestBody)
        {
            BoardGamePlayData? boardGamePlayData = JsonConvert.DeserializeObject<BoardGamePlayData>(requestBody.ToString());
            if (boardGamePlayData == null)
                return BadRequest("Invalid request body");
            string bgId = _boardGameDBOperations.GetBGId(boardGamePlayData);

            var players = new BoardGamePlayers(boardGamePlayData.BoardGameName, boardGamePlayData.Players, boardGamePlayData.ID);

            _boardGameDBOperations.InsertBGPlayers(players);
            _boardGameDBOperations.InsertBGPlayData(boardGamePlayData, bgId);

            return Ok();
        }

        [HttpGet]
        [Route("GetAllOrganisationsNames")]
        public IActionResult GetAllOrganisations()
        {
            var organisations = _boardGameDBOperations.GetAllOrganisationsNames();

            return Ok(organisations);
        }

        [HttpGet]
        [Route("GetBGByOrganisation/{organisationName}")]
        public IActionResult GetBoardGameByOrganisation(string organisationName)
        {
            var boardGames = _boardGameDBOperations.GetAllBoardGamesByOrganisationName(organisationName);

            return Ok(boardGames);
        }
    }
}
