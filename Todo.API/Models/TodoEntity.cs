using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.API.Models
{
    public class TodoEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
        
        [Required]
        public string CreatedDate { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        public string UpdatedDate { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
    }
}
