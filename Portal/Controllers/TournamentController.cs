using Microsoft.AspNetCore.Mvc;
using Portal.DBMethods;
using Portal.DTO;
using Portal.Utils;

namespace Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly TournamentDBO _tournamentDBOperations;

        public TournamentController(TournamentDBO tournamentDBOperations)
        {
            _tournamentDBOperations = tournamentDBOperations;
        }

        //// GET: api/<TournamentController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        [HttpGet("{tournamentId}")]
        public IActionResult Get(string tournamentId)
        {
            var tournament = _tournamentDBOperations.SelectTournament(tournamentId);
            return Ok(tournament);
        }

        [HttpGet("organisation/{organisationId}")]
        public IActionResult GetOrganisationsTournaments(string organisationId)
        {
            var tournaments = _tournamentDBOperations.SelectOrganisationsTournaments(organisationId);
            return Ok(tournaments);
        }

        // POST api/<TournamentController>
        [HttpPost]
        public IActionResult Post([FromBody] TournamentCreation tournamentData)
        {
            var players = tournamentData.Players.Select(p => new TournamentParticipant { Name = p }).ToList();
            List<TournamentMatch> tournamentMatches = TournamentGenerator.Generate(players);

            _tournamentDBOperations.InsertTournament(tournamentData);
            _tournamentDBOperations.InsertTournamentMatches(tournamentMatches, tournamentData.ID);

            return base.Ok(tournamentData);
        }

        //// PUT api/<TournamentController>/5 //for winning points
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string tournamentMatches)
        //{
        //}

        //// DELETE api/<TournamentController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
