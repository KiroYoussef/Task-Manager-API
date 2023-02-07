using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskAPI.Data;
using TaskAPI.DTO;
using TaskAPI.Repository;
using static TaskAPI.DTO.RoleEnums;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ContextDb _context;
        private readonly IUserRepository _UsersService;

        public UserController(ContextDb context, IUserRepository user)
        {
            _context = context;
            _UsersService = user;   
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetUsers(int page=1, int limit=10, string? name="4|0|4")
        {
            if (name== "4|0|4" || name==null)
            {
                return Ok(_UsersService.GetUsersByName(page, limit));

            }
            return Ok(_UsersService.GetUsersByName(page, limit, name));
        }
        [HttpPut]
        [Route("ChangeStatus")]
        public async Task<IActionResult> ChangeUserStatus([FromBody] UserStatus UserStatus)
        {
            if (_UsersService.ChangeUserStatus(UserStatus.id, UserStatus.status))
            {
                return Ok("Status Changed");

            }
            return BadRequest("Error occurred");
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        //[Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(string id)
        {
            if (_UsersService.DeleteUser(id))
            {
                return Ok("User Deleted");
            }
            return BadRequest("User Not Found");
        }
    }
}
