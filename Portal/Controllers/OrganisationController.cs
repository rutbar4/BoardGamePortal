using Microsoft.AspNetCore.Mvc;
using Portal.DBMethods;
using Portal.DTO;

namespace Portal.Controllers
{
    [Route("api/Organisation")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly OrganisationDBOperations _organisationDBOperations;

        public OrganisationController(OrganisationDBOperations organisationDBOperations)
        {
            _organisationDBOperations = organisationDBOperations;
        }

        [HttpPut]
        [Route("UpdateOrganisation")]
        public IActionResult UpdateOrganisation([FromBody] Organisation organisation)
        {
            if (organisation is null)
                return BadRequest("Invalid request body");

            _organisationDBOperations.UpdateOrganisation(organisation);

            return Ok(organisation);
        }
    }
}