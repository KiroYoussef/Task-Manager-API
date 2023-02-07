using TaskAPI.DTO;
using TaskAPI.Model;

namespace TaskAPI.Repository
{
    public interface IUserRepository
    {
        public bool CheckUser(string id);
        public IEnumerable<GetUsers> GetUsersByName(int page ,int limit ,string name);
        public IEnumerable<GetUsers> GetUsersByName(int page, int limit);
        public bool ChangeUserStatus(string id, string status);
        public bool DeleteUser(string id);

        //public bool DeleteUser(string id);

    }
}
