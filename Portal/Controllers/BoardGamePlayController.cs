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
        private readonly BoardGamePlayDBOperations _boardGamePlayDBOperations;

        public BoardGamePlayController(BoardGameDBOperations boardGameDBOperations, BoardGamePlayDBOperations boardGamePlayDBOperations)
        {
            _boardGameDBOperations = boardGameDBOperations;
            _boardGamePlayDBOperations = boardGamePlayDBOperations;
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

        [HttpGet]
        [Route("AllPlaysByOrganisationId/{organisationId}")]
        public IActionResult GetAllPlaysByOrganisationId(string organisationId)
        {
            if (organisationId is null)
                return BadRequest("Invalid request body");
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(organisationId);
            //var plays = boardGames.Where(s => s is not null).Select(s => _boardGamePlayDBOperations.GetBGPlayByBgIdWithPlayersCount(s.ID)).ToList();
            var list = new List<BoardGamePlayData>();
            foreach (var play in boardGames)
            {
                if(play is not null)
                {
                    var i = _boardGamePlayDBOperations.GetBGPlayByBgIdWithPlayersCount(play.ID);
                    if(i is not null)
                    foreach(var j in i)
                    {
                        list.Add(j);
                    }
                }
            }

            return Ok(list);
        }
        [HttpGet]
        [Route("AllPlaysByUserId/{userId}")]
        public IActionResult GetAllPlaysByUserId(string userId)
        {
            if (userId is null)
                return BadRequest("Invalid request body");
            var boardGames = _boardGameDBOperations.GetAllBGByUserId(userId);
            //var plays = boardGames.Where(s => s is not null).Select(s => _boardGamePlayDBOperations.GetBGPlayByBgIdWithPlayersCount(s.ID)).ToList();
            var list = new List<BoardGamePlayData>();
            foreach (var play in boardGames)
            {
                if (play is not null)
                {
                    var i = _boardGamePlayDBOperations.GetBGPlayByBgIdWithPlayersCount(play.ID);
                    if (i is not null)
                        foreach (var j in i)
                        {
                            list.Add(j);
                        }
                }
            }

            return Ok(list);
        }
        [HttpPost]
        [Route("TopMonthPlayers/{organisationId}")]
        public IActionResult GetTopMonthPlayers(string organisationId, [FromBody] DateTime month)
        {

            if (organisationId is null)
                return BadRequest("Invalid request body");
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(organisationId);
            var list = new List<BoardGamePlayData>();
            foreach (var play in boardGames)
            {
                if (play is not null)
                {
                    var i = _boardGamePlayDBOperations.GetBGPlayByBgIdWithPlayersCount(play.ID);
                    if (i is not null)
                        foreach (var j in i)
                        {
                            list.Add(j);
                        }
                }
            }
            var result = _boardGamePlayDBOperations.GetTopMonthPlayer(list, month);

            if (result is not null)
            {
                var count = list.Where(s => s.DatePlayed.Value.Month.Equals(month.Month) && s.DatePlayed.Value.Year.Equals(month.Year))
                    .Count(l => l.Winner == result[0]);

                return Ok(new { Players = string.Join(", ", result), WinCount = count });
            }
            return Ok(new { BoardGames = "", Count = "" });
        }

        [HttpPost]
        [Route("TopMonthBoardGames/{organisationId}")]
        public IActionResult GetTopMonthBG(string organisationId, [FromBody] DateTime month)
        {

            if (organisationId is null)
                return BadRequest("Invalid request body");

            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(organisationId);
            var list = new List<BoardGamePlayData>();
            foreach (var play in boardGames)
            {
                if (play is not null)
                {
                    var i = _boardGamePlayDBOperations.GetBGPlayByBgIdWithPlayersCount(play.ID);
                    if (i is not null)
                        foreach (var j in i)
                        {
                            list.Add(j);
                        }
                }
            }

            var result = _boardGamePlayDBOperations.GetTopMonthGame(list, month);
            if (result is not null)
            {
                var count = list.Where(s => s.DatePlayed.Value.Month.Equals(month.Month) && s.DatePlayed.Value.Year.Equals(month.Year))
                    .Count(l => l.BoardGameName == result[0]);

                return Ok(new { BoardGames = string.Join(", ", result), Count = count });
            }
            return Ok(new { BoardGames = "", Count = "" });
        }

        [HttpGet]
        [Route("BGPlaysCountbyOrganisationId/Top10/{organisationId}")]
        public IActionResult GetTop10BGPlaysCountbyOrganisationId(string organisationId) //+send max count
        {
            if (organisationId is null)
                return BadRequest("Invalid request body");
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(organisationId);
            var list = new List<BoardGamePlayCount>();
            foreach (var play in boardGames)
            {
                if (play is not null)
                {
                    var count = _boardGamePlayDBOperations.GetBGPlayCount(play.ID);
                    var name = _boardGameDBOperations.GetBGByBDId(play.ID).Name;
                    if (count != 0)
                    list.Add(new BoardGamePlayCount { BoardGameName = name, PlayCount = count});
                }
            }
            
            return Ok(list.OrderByDescending(p => p.PlayCount).Take(10));
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
        [Route("GetBGByUserId/{userId}")]
        public IActionResult GetAllBoardGamesNamesByUser(string userId)
        {
            var boardGames = _boardGameDBOperations.GetAllBoardGamesNamesByUserID(userId);

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
