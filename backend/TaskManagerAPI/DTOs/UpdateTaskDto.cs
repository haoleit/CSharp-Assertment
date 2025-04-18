// backend/TaskManagerAPI/DTOs/UpdateTaskDto.cs
using System;
using System.ComponentModel.DataAnnotations;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.DTOs
{
    public class UpdateTaskDto
    {
        [MaxLength(100)]
        public string? Title { get; set; } // Optional: Only update if provided

        public string? Description { get; set; } // Optional

        public Models.TaskStatus? Status { get; set; } // Optional

        public DateTime? DueDate { get; set; } // Optional

        // Id and UserId are not updatable via this DTO
        // Timestamps (CreatedAt, UpdatedAt) are managed by the system
    }
}
