using System.ComponentModel.DataAnnotations;

namespace TaskAPI.DTO
{
    public class NewTask
    {
        [Required]
        public string Tite { get; set; }
        public IFormFile img { get; set; }
        public DateTime DeadLine { get; set; }
        public string Discreption { get; set; }
        public string UserID { get; set; }
    }
}
