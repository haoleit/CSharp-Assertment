
using System;
using System.ComponentModel.DataAnnotations;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.DTOs
{
    public class UpdateTaskDto
    {
        [MaxLength(100)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public Models.TaskStatus? Status { get; set; }

        public DateTime? DueDate { get; set; }


    }
}
