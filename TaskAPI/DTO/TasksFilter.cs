using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TaskAPI.DTO
{
    public class TasksFilter
    {
        [Required]
        public int page { get; set; } = 1;
        [Required]
        public int limit { get; set; } = 10;
        public bool? status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        

    }
    //new DateTime(1900,1,1)
}
