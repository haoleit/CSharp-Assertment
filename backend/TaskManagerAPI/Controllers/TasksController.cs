


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
        private readonly IAuthService _authService;  // Inject AuthService

        public TasksController(ITaskService taskService, IAuthService authService)
        {
            _taskService = taskService;
            _authService = authService;  // Initialize AuthService
        }

        // Helper method to get current user ID using GetProfileAsync from AuthService
        private async Task<string> GetUserIdAsync()
        {
            var user = await _authService.GetProfileAsync();
            if (user == null)
            {
                // This should ideally not happen if [Authorize] is working correctly
                throw new InvalidOperationException("User not found in the system.");
            }
            return user.Id; // Return user ID from ApplicationUser
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = await GetUserIdAsync(); // Get user ID from AuthService
            var tasks = await _taskService.GetTasksAsync(userId);
            return Ok(tasks);
        }

        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var userId = await GetUserIdAsync(); // Get user ID from AuthService
            var task = await _taskService.GetTaskByIdAsync(id, userId);

            if (task == null)
            {
                return NotFound($"Task with ID {id} not found or you do not have permission.");
            }

            return Ok(task);
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await GetUserIdAsync(); // Get user ID from AuthService
            var createdTask = await _taskService.CreateTaskAsync(createTaskDto, userId);

            // Return 201 Created status with the location of the new resource and the resource itself
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = await GetUserIdAsync(); // Get user ID from AuthService
            var success = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);

            if (!success)
            {
                // Could be not found or a concurrency issue handled in the service
                return NotFound($"Task with ID {id} not found or update failed.");
            }

            return NoContent(); // Standard response for successful PUT update
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = await GetUserIdAsync(); // Get user ID from AuthService
            var success = await _taskService.DeleteTaskAsync(id, userId);

            if (!success)
            {
                return NotFound($"Task with ID {id} not found or delete failed.");
            }

            return NoContent(); // Standard response for successful DELETE
        }
    }
}
