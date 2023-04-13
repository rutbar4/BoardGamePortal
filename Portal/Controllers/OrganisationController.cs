using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portal.DBMethods;
using Portal.DTO;

namespace Portal.Controllers
{
    [Route("api/Organisation")]
    [ApiController]
    public class OrganisationController :ControllerBase
    {
        private readonly OrganisationDBOperations _organisationDBOperations;

        public OrganisationController(OrganisationDBOperations organisationDBOperations)
        {
            _organisationDBOperations = organisationDBOperations;
        }

        [HttpPost]
        [Route("registerOrganisation")]
        public IActionResult RegisterOrganisation([FromBody] object requestBody)
        {
            Organisation? organisation = JsonConvert.DeserializeObject<Organisation>(requestBody.ToString());
            if (organisation == null)
                return BadRequest("Invalid request body");

            _organisationDBOperations.InsertOrganisation(organisation);

            return Ok();
        }
    }
}
