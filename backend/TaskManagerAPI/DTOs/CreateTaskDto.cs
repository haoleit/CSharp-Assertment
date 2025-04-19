

using System.ComponentModel.DataAnnotations;


namespace TaskManagerAPI.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }


        public Models.TaskStatus? Status { get; set; }

        public DateTime? DueDate { get; set; }


    }
}
