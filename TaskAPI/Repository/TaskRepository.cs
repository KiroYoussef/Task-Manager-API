using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using TaskAPI.Data;
using TaskAPI.DTO;
using TaskAPI.Model;
using static System.Net.WebRequestMethods;

namespace TaskAPI.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ContextDb _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TaskRepository(ContextDb context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this._webHostEnvironment = webHostEnvironment;

        }

        public bool CheckIfImage(IFormFile img)
        {
            var name = img.FileName;
            return name.EndsWith(".png") || name.EndsWith(".jpg") || name.EndsWith(".jpeg");
        }

        public bool DeleteImage(string img)
        {

            var FilePath = Path.Combine
            (Directory.GetCurrentDirectory(), "wwwroot//images//task", img);
           if(System.IO.File.Exists(FilePath))
            {
                System.IO.File.Delete(FilePath);
            };
            return true;
        }


        public async Task<IEnumerable<Tasks>> GetAllTasks( string? UserId,  string? Keyword,TasksFilter TaskDTO)
        {
            IEnumerable<Tasks> Tasks;

            if (UserId!= "-1" && Keyword == "-1") 
            {
                   
                 Tasks= _context.Tasks.Where(
                p => p.Completion == TaskDTO.status &&
                p.UserID == UserId &&
                p.StartingDate >= TaskDTO.FromDate &&
                p.StartingDate <= TaskDTO.ToDate 
                ).Skip(((TaskDTO.page - 1) * TaskDTO.limit)).Take(TaskDTO.limit).ToList();
            }
            else if (UserId != "-1" && Keyword != "-1")
            {
                Tasks = _context.Tasks.Where(p =>
                    p.Tite.Contains(Keyword) &&
                p.Completion == TaskDTO.status &&
                p.UserID == UserId &&
                p.StartingDate >= TaskDTO.FromDate &&
                p.StartingDate <= TaskDTO.ToDate)
                   .Skip(((TaskDTO.page - 1) * TaskDTO.limit)).Take(TaskDTO.limit).ToList();
            }else if (UserId == "-1" && Keyword != "-1")
            {
                Tasks = _context.Tasks.Where(p =>
                   p.Tite.Contains(Keyword) &&
               p.Completion == TaskDTO.status &&
               p.StartingDate >= TaskDTO.FromDate &&
               p.StartingDate <= TaskDTO.ToDate)
                  .Skip(((TaskDTO.page - 1) * TaskDTO.limit)).Take(TaskDTO.limit).ToList();

            }
            Tasks = _context.Tasks.Where(
            p => p.Completion == TaskDTO.status &&
            p.StartingDate >= TaskDTO.FromDate &&
            p.StartingDate <= TaskDTO.ToDate
            ).Skip(((TaskDTO.page - 1) * TaskDTO.limit)).Take(TaskDTO.limit).ToList();

            foreach(var task in Tasks)
            {
                task.PhotoName = "https://localhost:7179/images/task/" + task.PhotoName;
            }
            return Tasks;
        }

        public async Task<string> NewTask( NewTask Data)
        {
            var ImageName=Guid.NewGuid()+ Data.img.FileName;
            if (await UploadeImage(Data.img, ImageName))
            {
                Tasks Task = new Tasks();
                Task.id = new Guid();
                Task.Tite = Data.Tite;
                Task.DeadLine = Data.DeadLine;
                Task.StartingDate = DateTime.Now;
                Task.Discreption = Data.Discreption;
                Task.Completion = false;
                Task.PhotoName = ImageName;
                Task.UserID = Data.UserID;
                _context.Add(Task);
                _context.SaveChanges();
                return "ok";
            }
            return "Error While Uploding";
        }

        public async Task<bool> UpdateTask(Guid Id,NewTask UpdatedTask)
        {
            if( TaskIsFound(Id))
            {
                var OldTask = _context.Tasks.Where(t => t.id == Id).FirstOrDefault();
                OldTask.Tite = (UpdatedTask.Tite == null) ? OldTask.Tite : UpdatedTask.Tite;
                OldTask.DeadLine = UpdatedTask.DeadLine;
                OldTask.Discreption = (UpdatedTask.Discreption == null) ? OldTask.Discreption : UpdatedTask.Discreption;
                OldTask.UserID = (UpdatedTask.UserID == null) ? OldTask.UserID : UpdatedTask.UserID;
                if (OldTask.PhotoName != UpdatedTask.img.FileName)
                {
                    var ImgName = Guid.NewGuid() + UpdatedTask.img.FileName;
                    if (DeleteImage(OldTask.PhotoName) && await UploadeImage(UpdatedTask.img, ImgName))
                    {
                        OldTask.PhotoName = ImgName;
                    }
                    else
                    {
                        return false;
                    }

                }

                _context.Tasks.Update(OldTask);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public  async Task<bool> UploadeImage(IFormFile img,string ImageName)
        {
            var FilePath = Path.Combine
                (
              _webHostEnvironment.WebRootPath, "images/task", ImageName
                );
            using (FileStream FS = new FileStream(FilePath, FileMode.Create))
            {
                await img.CopyToAsync(FS);
                FS.Close();

            };
           return  System.IO.File.Exists(FilePath);
        }
        public async Task<bool> CompleteTask(string Id)
        {
            Guid ID;
            bool isValid = Guid.TryParse(Id, out ID);
            if (isValid && TaskIsFound(ID))
            {
                Tasks OldTask = _context.Tasks.Where(t => t.id == ID).FirstOrDefault();
                OldTask.Completion = true;
                _context.Tasks.Update(OldTask);
                _context.SaveChanges();
                return true;
            }
            return false;
           
        }

        public async Task<bool> DeleteTask(string Id)
        {
            Guid ID;
            bool isValid = Guid.TryParse(Id, out ID);
            if (isValid && TaskIsFound(ID))
            {
                Tasks OldTask = _context.Tasks.Where(t => t.id == ID).FirstOrDefault();
                DeleteImage(OldTask.PhotoName);
                _context.Tasks.Remove(OldTask);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<Tasks> TaskDetail(string Id)
        {
            Guid ID;
            var IsValid =   Guid.TryParse(Id,out ID);
            if (IsValid && TaskIsFound(ID))
            {
                Tasks task =_context.Tasks.Where(O=>O.id==ID).FirstOrDefault();
                task.PhotoName = "https://localhost:7179/images/task/" + task.PhotoName;
                return task;
            }
            return  new Tasks() { Tite="404Error"} ;
        }

        public  bool TaskIsFound(Guid Id)
        {
            var TaskId = _context.Tasks.Where(I => I.id == Id).Select(k => k.id).FirstOrDefault();
            return TaskId != null && TaskId != Guid.Empty;
        }
    }
}

