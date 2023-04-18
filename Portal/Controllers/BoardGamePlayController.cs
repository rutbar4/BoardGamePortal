﻿using Microsoft.AspNetCore.Mvc;
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
