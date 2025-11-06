using System.ComponentModel.DataAnnotations;

namespace BlogShadan.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="The category name is required")]
        [MaxLength(200,ErrorMessage ="The category name cannot exceed 200 character")]
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
