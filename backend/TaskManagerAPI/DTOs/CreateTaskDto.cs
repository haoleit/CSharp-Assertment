// backend/TaskManagerAPI/DTOs/CreateTaskDto.cs
using System;
using System.ComponentModel.DataAnnotations;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Status defaults to ToDo in the model, so it's optional here
        public Models.TaskStatus? Status { get; set; }

        public DateTime? DueDate { get; set; }

        // UserId will be inferred from the logged-in user context, not provided by the client
    }
}
