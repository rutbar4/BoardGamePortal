using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portal.DBMethods;
using Portal.DTO;

namespace Portal.Controllers
{
    [Route("api/BoardGamePlay")]
    [ApiController]
    public class BoardGamePlayController : ControllerBase
    {
        private readonly BoardGameDBOperations _boardGameDBOperations;
        private readonly BoardGamePlayDBOperations _boardGamePlayDBOperations;
        private readonly OrganisationDBOperations _organisationDBOperations;

        public BoardGamePlayController(BoardGameDBOperations boardGameDBOperations, BoardGamePlayDBOperations boardGamePlayDBOperations, OrganisationDBOperations organisationDBOperations)
        {
            _boardGameDBOperations = boardGameDBOperations;
            _boardGamePlayDBOperations = boardGamePlayDBOperations;
            _organisationDBOperations = organisationDBOperations;
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

        [HttpGet]
        [Route("BGPlaysByOrganisationId/{profileId}")]
        public IActionResult GetBGPlaysByProfileId(string profileId)
        {
            if (profileId is null)
                return BadRequest("Invalid request body");
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(profileId);
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

            var t = _boardGamePlayDBOperations.GetBGPlayUserIdWithPlayersCount(profileId);

            list.AddRange(t);

            var boardGamelist = list
                .GroupBy(p => p.BoardGameName)
                .Select(g => new
                {
                    BoardGameName = g.Key,
                    Count = g.Where(l => l.BoardGameName == (g.GroupBy(q => q.BoardGameName)
                                                    .OrderByDescending(gp => gp.Count())
                                                     .First().Key)).Count(),
                    Record = g.Select(l => l.WinnerPoints).Max(),
                    MostWinningPlayer = g.GroupBy(q => q.Winner)
                                    .OrderByDescending(gp => gp.Count())
                                    .First().Key,
                    Wins = g.Where(l => l.Winner == (g.GroupBy(q => q.Winner)
                                                    .OrderByDescending(gp => gp.Count())
                                                     .First().Key)).Count(),
                    MostActivePlayer = g.Where(i => i.Players.Count() > 0)?
                                    .SelectMany(l => l.Players)
                                    .GroupBy(name => name)
                                    .OrderByDescending(gp => gp.Count())
                                    .FirstOrDefault()?.Key,
                    MostActivePlayerPlayCount = g.Where(i => i.Players.Count() > 0)?
                                    .SelectMany(l => l.Players)
                                    .GroupBy(name => name)
                                    .OrderByDescending(gp => gp.Count())
                                    .FirstOrDefault()?.Count(),
                })
                .ToList();

            return Ok(boardGamelist);
        }

        [HttpGet]
        [Route("AllPlaysByUserId/{userId}")]
        public IActionResult GetAllPlaysByUserId(string userId)
        {
            if (userId is null)
                return BadRequest("Invalid request body");
            var boardGames = _boardGameDBOperations.GetAllBGByUserId(userId);
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

            var t = _boardGamePlayDBOperations.GetBGPlayUserIdWithPlayersCount(userId);

            list.AddRange(t);
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
        public IActionResult GetTop10BGPlaysCountbyOrganisationId(string organisationId)
        {
            if (organisationId is null)
                return BadRequest("Invalid request body");
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(organisationId);
            var list = new List<BoardGamePlayCount>();

            var t = _boardGamePlayDBOperations.GetBGPlayUserIdWithPlayersCount(organisationId);

            list.AddRange(t.Select(p => new BoardGamePlayCount { BoardGameName = p.BoardGameName, PlayCount = _boardGamePlayDBOperations.GetBGPlayCount(p.BoardGameID) }));
            foreach (var play in boardGames)
            {
                if (play is not null)
                {
                    var count = _boardGamePlayDBOperations.GetBGPlayCount(play.ID);
                    var name = _boardGameDBOperations.GetBGByBDId(play.ID).Name;
                    if (count != 0)
                        list.Add(new BoardGamePlayCount { BoardGameName = name, PlayCount = count });
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
        public IActionResult GetAllOrganisationsNames()
        {
            var organisations = _boardGameDBOperations.GetAllOrganisationsNames();

            return Ok(organisations);
        }

        [HttpGet]
        [Route("AllOrganisations")]
        public IActionResult GetAllOrganisations()
        {
            var organisations = _boardGameDBOperations.GetAllOrganisations();

            return Ok(organisations);
        }

        [HttpGet]
        [Route("GetBGByOrganisation/{organisationName}")]//BGnames
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
        [Route("GetAllBGDataByOrganisation/{organisationID}")]//organisationID
        public IActionResult GetAllDataBoardGameByOrganisation(string organisationID)
        {
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(organisationID);

            return Ok(boardGames);
        }

        [HttpGet]
        [Route("GetAllBGDataByOrganisationName/{organisationName}")]
        public IActionResult GetAllDataBoardGameByOrganisationName(string organisationName)
        {
            var orgId = _organisationDBOperations.GetOrganisationIdByName(organisationName);
            var boardGames = _boardGameDBOperations.GetAllBGByOrganisation(orgId);

            return Ok(boardGames);
        }
    }
}