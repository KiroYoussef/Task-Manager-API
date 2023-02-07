using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskAPI.Model;
namespace TaskAPI.Data
{
    public class ContextDb:IdentityDbContext<ApplicationUser>
    {
        public ContextDb(DbContextOptions<ContextDb> options) : base(options) 
        {
        }
        public virtual DbSet<Model.Tasks> Tasks { get; set; }
    }
}
