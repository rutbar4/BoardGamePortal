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
            if (boardGamePlayData is null)
                return BadRequest("Invalid request body");
            string bgId = _boardGameDBOperations.GetBGId(boardGamePlayData);

            var players = new BoardGamePlayers(boardGamePlayData.BoardGameName, boardGamePlayData.Players, boardGamePlayData.ID);

            _boardGameDBOperations.InsertBGPlayers(players);
            _boardGameDBOperations.InsertBGPlayData(boardGamePlayData, bgId);

            return Ok();
        }
        [HttpPost]
        [Route("AddOrganisationBG")]
        public IActionResult AddBGOfOrganisation([FromBody] object requestBody)
        {
            BoardGame? boardGame = JsonConvert.DeserializeObject<BoardGame>(requestBody.ToString());
            if (boardGame is null)
                return BadRequest("Invalid request body");

           _boardGameDBOperations.InsertBGOfOrganisation(boardGame);
            var allOrgBoardGames = _boardGameDBOperations.GetAllBGByOrganisation(boardGame.OrganisationId);

            return Ok(allOrgBoardGames);
        }

        [HttpDelete]
        [Route("DeleteOrganisationBG")]
        public IActionResult DeleteBGOfOrganisation([FromBody] string organisationId, [FromBody] string boardGameName)
        {
            if (organisationId is null || boardGameName is null)
                return BadRequest("Invalid request body");

            var boardGame = _boardGameDBOperations.GetBGByOrganisationIdAndBGName(organisationId, boardGameName);
            _boardGameDBOperations.DeleteBGOfOrganisation(organisationId, boardGameName);

            return Ok(boardGame);//returns deleted item
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
            var boardGames = _boardGameDBOperations.GetAllBoardGamesNamesByOrganisationName(organisationName);

            return Ok(boardGames);
        }
        [HttpGet]

        [Route("GetAllBGDataByOrganisation/{organisationName}")]
        public IActionResult GetAllDataBoardGameByOrganisation(string organisationName)
        {
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(organisationName);

            return Ok(boardGames);
        }
    }
}
