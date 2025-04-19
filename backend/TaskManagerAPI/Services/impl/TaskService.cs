
using Microsoft.EntityFrameworkCore;

using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.impl
{
    public class TaskService : ITaskService
    {
        private readonly DataContext _context;

        public TaskService(DataContext context)
        {
            _context = context;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto taskDto, string userId)
        {
            var task = new Models.Task
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status ?? Models.TaskStatus.ToDo,
                DueDate = taskDto.DueDate,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return MapTaskToDto(task);
        }

        public async Task<bool> DeleteTaskAsync(int taskId, string userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int taskId, string userId)
        {
            var task = await _context.Tasks
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            return task == null ? null : MapTaskToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetTasksAsync(string userId)
        {
            var tasks = await _context.Tasks
                                      .Where(t => t.UserId == userId)
                                      .AsNoTracking()
                                      .OrderByDescending(t => t.CreatedAt)
                                      .ToListAsync();

            return tasks.Select(MapTaskToDto);
        }

        public async Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto taskDto, string userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
            {
                return false;
            }


            if (taskDto.Title != null)
            {
                task.Title = taskDto.Title;
            }
            if (taskDto.Description != null)
            {
                task.Description = taskDto.Description;
            }
            if (taskDto.Status.HasValue)
            {
                task.Status = taskDto.Status.Value;
            }
            if (taskDto.DueDate.HasValue)
            {
                task.DueDate = taskDto.DueDate;
            }

            if (task.CreatedAt.Kind == DateTimeKind.Unspecified)
            {
                task.CreatedAt = task.CreatedAt.ToUniversalTime();
            }


            task.UpdatedAt = DateTime.UtcNow;

            _context.Entry(task).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {

                return false;
            }
        }


        private static TaskDto MapTaskToDto(Models.Task task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                UserId = task.UserId
            };
        }
    }
}
