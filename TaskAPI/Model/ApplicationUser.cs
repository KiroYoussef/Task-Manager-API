using Microsoft.AspNetCore.Identity;

namespace TaskAPI.Model
{
    public class ApplicationUser:IdentityUser
    {
        public bool Status { get; set; }
        public virtual ICollection<Tasks> Task { get; set; }
    }
}
