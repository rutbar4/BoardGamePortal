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
        [Route("DeleteOrganisationBG/{boardGameid}")]
        public IActionResult DeleteBGOfOrganisation(string boardGameid)
        {
            if (boardGameid is null)
                return BadRequest("Invalid request body");

            _boardGameDBOperations.DeleteBGOfOrganisation(boardGameid);
            string? userId = HttpContext.Items.First(i => i.Key == "UserId").Value as string;
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(userId);

            return Ok(boardGames);//returns not deleted items
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
        public IActionResult GetAllBoardGamesNamesByOrganisation(string organisationName)
        {
            var boardGames = _boardGameDBOperations.GetAllBoardGamesNamesByOrganisationName(organisationName);

            return Ok(boardGames);
        }

        [HttpGet]
        [Route("GetAllBGDataByOrganisation/{organisationID}")]
        public IActionResult GetAllDataBoardGameByOrganisation(string organisationID)
        {
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(organisationID);

            return Ok(boardGames);
        }

        //[HttpGet]
        //[Route("GetBGDataByBgNameAndOrgId/{organisationName}")]
        //public IActionResult GetBGDataByBgNameAndOrgId(string organisationID, string boardgameName)
        //{
        //    var boardGames = _boardGameDBOperations.GetBGbyOrgIdAndBGName(organisationID, boardgameName);

        //    return Ok(boardGames);
        //}


    }
}
