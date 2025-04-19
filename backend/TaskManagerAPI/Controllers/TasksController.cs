


using Microsoft.AspNetCore.Mvc;

using TaskManagerAPI.DTOs;

using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IAuthService _authService; 

        public TasksController(ITaskService taskService, IAuthService authService)
        {
            _taskService = taskService;
            _authService = authService;  
        }

        
        private async Task<string> GetUserIdAsync()
        {
            var user = await _authService.GetProfileAsync();
            if (user == null)
            {
                
                throw new InvalidOperationException("User not found in the system.");
            }
            return user.Id; 
        }

        
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = await GetUserIdAsync(); 
            var tasks = await _taskService.GetTasksAsync(userId);
            return Ok(tasks);
        }

       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var userId = await GetUserIdAsync(); 
            var task = await _taskService.GetTaskByIdAsync(id, userId);

            if (task == null)
            {
                return NotFound($"Task with ID {id} not found or you do not have permission.");
            }

            return Ok(task);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await GetUserIdAsync(); 
            var createdTask = await _taskService.CreateTaskAsync(createTaskDto, userId);

            
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await GetUserIdAsync(); 
            var success = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);

            if (!success)
            {
                
                return NotFound($"Task with ID {id} not found or update failed.");
            }

            return NoContent(); 
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = await GetUserIdAsync(); 
            var success = await _taskService.DeleteTaskAsync(id, userId);

            if (!success)
            {
                return NotFound($"Task with ID {id} not found or delete failed.");
            }

            return NoContent(); 
        }
    }
}
