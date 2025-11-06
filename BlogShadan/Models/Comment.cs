using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogShadan.Models
{
 
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="The username is required")]
        [MaxLength(100,ErrorMessage ="The username cannot exceed 100 character")]
        public string UserName { get; set; }
        [DataType(DataType.Date)]
        public DateTime CommentDate { get; set; }
        [Required]
        public string Content { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

    }
}
