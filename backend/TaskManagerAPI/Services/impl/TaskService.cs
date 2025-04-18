// backend/TaskManagerAPI/Services/impl/TaskService.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models; // Required for Task and TaskStatus

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
            var task = new Models.Task // Fully qualify Task
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status ?? Models.TaskStatus.ToDo, // Use qualified TaskStatus, default if null
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
                return false; // Task not found or doesn't belong to the user
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskDto?> GetTaskByIdAsync(int taskId, string userId)
        {
            var task = await _context.Tasks
                                     .AsNoTracking() // Read-only operation
                                     .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            return task == null ? null : MapTaskToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetTasksAsync(string userId)
        {
            var tasks = await _context.Tasks
                                      .Where(t => t.UserId == userId)
                                      .AsNoTracking() // Read-only operation
                                      .OrderByDescending(t => t.CreatedAt) // Example ordering
                                      .ToListAsync();

            return tasks.Select(MapTaskToDto); // Use the helper method for mapping
        }

        public async Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto taskDto, string userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
            {
                return false; // Task not found or doesn't belong to the user
            }

            // Update fields only if they are provided in the DTO
            if (taskDto.Title != null)
            {
                task.Title = taskDto.Title;
            }
            if (taskDto.Description != null) // Allow setting description to null/empty
            {
                task.Description = taskDto.Description;
            }
            if (taskDto.Status.HasValue)
            {
                task.Status = taskDto.Status.Value;
            }
            if (taskDto.DueDate.HasValue) // Allow setting due date to null
            {
                task.DueDate = taskDto.DueDate;
            }

            if (task.CreatedAt.Kind == DateTimeKind.Unspecified)
            {
                task.CreatedAt = task.CreatedAt.ToUniversalTime(); // Convert to UTC if unspecified
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
                // Handle concurrency conflict if necessary
                return false;
            }
        }

        // Helper method to map Task entity to TaskDto
        private static TaskDto MapTaskToDto(Models.Task task) // Fully qualify Task
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status, // No need to qualify here as it's assigned from qualified task.Status
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                UserId = task.UserId
            };
        }
    }
}
