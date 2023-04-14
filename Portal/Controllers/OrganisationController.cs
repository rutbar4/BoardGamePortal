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
    }
}
