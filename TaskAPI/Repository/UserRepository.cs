using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TaskAPI.Data;
using TaskAPI.DTO;
using TaskAPI.Model;

namespace TaskAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ContextDb _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _UserManager;

        public UserRepository(ContextDb context, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _UserManager = userManager; 
        }

        public bool ChangeUserStatus(string id, string status)
        {
            bool Status; 
            
              if(bool.TryParse(status,out Status) && CheckUser (id))
                {
                    var user = _UserManager.Users.Where(p=>p.Id==id).FirstOrDefault();
                    user.Status = Status;
                    _context.Users.Update(user);
                    _context.SaveChanges();
                    return true;
                } 
                return false;
        }

        public IEnumerable<GetUsers> GetUsersByName(int page, int limit, string name)
        {
            List<GetUsers> AllUsersDTO = _context.Users.Where(r => r.UserName.Contains(name)).Select(
            user => new GetUsers{ Id = user.Id, PhoneNumber = user.PhoneNumber, UserName = user.UserName, Email = user.Email, Status = user.Status }
            ).Skip((page - 1) * limit).Take(limit).ToList();
            
            return AllUsersDTO;
        }

        public IEnumerable<GetUsers> GetUsersByName(int page, int limit)
        {
            List<GetUsers> AllUsersDTO = _context.Users.Select(
            user => new GetUsers  { Id = user.Id, PhoneNumber = user.PhoneNumber, UserName = user.UserName, Email = user.Email, Status = user.Status }
            ).Skip((page - 1) * limit).Take(limit).ToList();
         
            return AllUsersDTO;
        }

        public bool CheckUser(string id)
        {
            var FOUND = _context.Users.Where(r => r.Id == id).Select(I => I.Id).FirstOrDefault();
            return FOUND!=null;

        }
        public bool DeleteUser(string id)
        {
            if (CheckUser(id)){
                var user = _context.Users.FirstOrDefault(r => r.Id == id);
                _context.Users.Remove(user);
                _context.SaveChanges();
                return true;
            }
            return false;

        }
    }
}
