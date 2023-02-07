using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskAPI.Model
{
    public class Tasks
    {
        [Required]
        [Key]
        public Guid id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Tite { get; set; }
        [Required]
        [MinLength(100)]
        public string PhotoName  { get; set; }
        [Required]
        public DateTime DeadLine { get; set; }
        [Required]
        public DateTime StartingDate { get; set; } = DateTime.Now;
        [MaxLength(1000)]
        [MinLength(100)]
        public string Discreption { get; set; }
        public bool Completion { get; set; }=false;
        [ForeignKey("ApplicationUser")]
        public string UserID { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        
    }
}