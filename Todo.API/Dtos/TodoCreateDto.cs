using System.ComponentModel.DataAnnotations;

namespace Todo.API.Dtos
{
    public class TodoCreateDto
    {
        [Required(ErrorMessage = "Title Must be added")]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; } 
    }
}
