using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.InteropServices;
using TaskAPI.DTO;
using TaskAPI.Model;
using TaskAPI.Repository;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _TaskService;
        private readonly IUserRepository _UsersService;

        public TaskController(ITaskRepository TaskService, IUserRepository users)
        {
            _TaskService = TaskService;
            _UsersService = users;
        }
        [HttpGet]
        [Route("TaskFilter")]
        [Authorize]
        public async Task<IActionResult> GetTasks([FromQuery] TasksFilter task, [FromQuery] string? UserId = "-1", [FromQuery] string? Keyword = "-1")
        {
            var tasks = await _TaskService.GetAllTasks(UserId, Keyword, task);
            return Ok(tasks);
        }
        [HttpPost]
        [Route("AddTask")]
        public async Task<IActionResult> AddTask([FromForm] NewTask Data)

        {
            if (
                Data.DeadLine >= DateTime.Now &&
                _UsersService.CheckUser(Data.UserID) == true &&
                _TaskService.CheckIfImage(Data.img)
                )
            {
                await _TaskService.NewTask(Data);
                return Ok("Task Added successfully");
            }
            return BadRequest
            (">> User Not Found |OR| Deadline Not Valid |OR| Image Extension Not Supported (.png/.jpg/.jpeg) <<");
        }
        [HttpPut]
        [Route("UpdateTask/{ID}")]
        public async Task<IActionResult> UpdateTask(Guid ID, [FromForm] NewTask UpdatedTask)
        {
            if (UpdatedTask.DeadLine >= DateTime.Now &&
                _UsersService.CheckUser(UpdatedTask.UserID) == true &&
                _TaskService.CheckIfImage(UpdatedTask.img))
            {
                if (await _TaskService.UpdateTask(ID, UpdatedTask))
                {
                    return Ok("TASK UPDATED");
                }

                return BadRequest("ERROR");

            }
            return BadRequest
           (">> User Not Found |OR| Deadline Not Valid |OR| Image Extension Not Supported (.png/.jpg/.jpeg) <<");

        }

        [HttpPut]
        [Route("CompleteTask")]
        [Authorize]

        public async Task<IActionResult> CompleteTask([FromBody] string ID)
        {
            if (await _TaskService.CompleteTask(ID))
            {
                return Ok("Task Completed");
            }
            return BadRequest
           (">> Task Not Found <<");

        }
        [HttpDelete]
        [Route("Delete/{ID}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteTask(string ID)
        {
            if (await _TaskService.DeleteTask(ID))
            {
                return Ok("Task Deleted");
            }
            return BadRequest
           (">> Task Not Found <<");

        }
        [HttpGet]
        [Route("TaskDetails/{ID}")]
        public async Task<IActionResult> TaskDetails(string ID)
        {
            Tasks Details =await _TaskService.TaskDetail(ID);
            if (Details.Tite== "404Error")
            {
                return BadRequest("Task Not Found");

            }
            return Ok(Details);
        }
    }
}

