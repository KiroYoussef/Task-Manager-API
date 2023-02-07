using Microsoft.AspNetCore.Mvc;
using TaskAPI.DTO;
using TaskAPI.Model;

namespace TaskAPI.Repository
{
    public interface ITaskRepository
    {
        public Task<IEnumerable<Tasks>> GetAllTasks( string? UserId,  string? Keyword, TasksFilter TaskDTO);
        public  Task<string> NewTask( NewTask Task);
        public bool CheckIfImage(IFormFile img);
        public bool DeleteImage(string img);
        public Task<bool> UpdateTask(Guid Id, NewTask UpdatedTask);
        public Task<bool> CompleteTask(string Id);
        public Task<bool> DeleteTask(string Id);
        public Task<Tasks> TaskDetail(string Id);
        public bool TaskIsFound(Guid Id);

    }
}
