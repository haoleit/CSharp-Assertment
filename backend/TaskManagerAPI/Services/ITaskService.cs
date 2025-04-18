// backend/TaskManagerAPI/Services/ITaskService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetTasksAsync(string userId);
        Task<TaskDto?> GetTaskByIdAsync(int taskId, string userId);
        Task<TaskDto> CreateTaskAsync(CreateTaskDto taskDto, string userId);
        Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto taskDto, string userId);
        Task<bool> DeleteTaskAsync(int taskId, string userId);
    }
}
