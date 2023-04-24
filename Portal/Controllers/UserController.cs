using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Portal.DBMethods;
using Portal.DTO;

namespace Portal.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDBOperations _userDBOperations;

        public UserController(UserDBOperations userDBOperations)
        {
            _userDBOperations = userDBOperations;
        }

        [HttpPut]
        [Route("UpdateUser")]
        public IActionResult UpdateOrganisation([FromBody] User user)
        {
            if (user is null)
                return BadRequest("Invalid request body");

            _userDBOperations.UpdateUser(user);

            return Ok(user);
        }

    }
}
