using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TasksController(ILogger<TasksController> logger, AppDbContext baseContext) : Controller
    {
        private readonly ILogger<TasksController> _logger = logger;
        private readonly AppDbContext _baseContext = baseContext;

        [HttpGet]
        async public Task<IActionResult> MyTasks(bool isClosed)
        {
            var tasks = await _baseContext.Tasks
                            .Where(t => t.IsClosed == isClosed)
                            .Select(t => new TaskItem
                            {
                                Id = t.Id,
                                TaskTitle = t.TaskTitle,
                                TaskDescription = t.TaskDescription,
                                IsClosed = t.IsClosed
                            })
                            .ToListAsync();
    
            return View(tasks);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
