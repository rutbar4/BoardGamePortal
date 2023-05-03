using Microsoft.AspNetCore.Mvc;
using Portal.DTO;
using Portal.Utils;

namespace Portal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        //// GET: api/<TournamentController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<TournamentController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<TournamentController>
        [HttpPost]
        public IActionResult Post([FromBody] List<TournamentParticipant> players)
        {
            return Ok(TournamentGenerator.Generate(TournamentGenerator.GetTestPlayers(12)));
        }

        //// PUT api/<TournamentController>/5 //for winning points
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<TournamentController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
