using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portal.DBMethods;
using Portal.DTO;

namespace Portal.Controllers
{
    public class BoardGameController : ControllerBase
    {

        private readonly BoardGameDBOperations _boardGameDBOperations;
        private readonly BoardGamePlayDBOperations _boardGamePlayDBOperations;

        public BoardGameController(BoardGameDBOperations boardGameDBOperations, BoardGamePlayDBOperations boardGamePlayDBOperations)
        {
            _boardGameDBOperations = boardGameDBOperations;
            _boardGamePlayDBOperations = boardGamePlayDBOperations;
        }

        [HttpPost]
        [Route("AddUserBG")]///taisyti
        public IActionResult AddBGOfUser([FromBody] object requestBody)
        {
            BoardGame? boardGame = JsonConvert.DeserializeObject<BoardGame>(requestBody.ToString());
            if (boardGame is null)
                return BadRequest("Invalid request body");

            _boardGameDBOperations.InsertBGOfOrganisation(boardGame);
            var allOrgBoardGames = _boardGameDBOperations.GetAllBGByOrganisation(boardGame.OrganisationId);

            return Ok(allOrgBoardGames);
        }

        [HttpGet]
        [Route("GetAllBGDataByUser/{userID}")]
        public IActionResult GetAllDataBoardGameByOrganisation(string userID)
        {
            var boardGames = _boardGameDBOperations.GetAllBGByUserId(userID);

            return Ok(boardGames);
        }
    }
}
