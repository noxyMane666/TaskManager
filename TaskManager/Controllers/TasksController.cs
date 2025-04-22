using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskManager.Core.Abstractions;
using TaskManager.DTO;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TasksController(
        ITaskService taskService
        ) : Controller
    {
        private readonly ITaskService _taskService = taskService;

        [HttpGet]
        public async Task<IActionResult> MyTasks(bool isClosed)
        {
            var tasks = await _taskService.GetUserTasks(isClosed);

            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask([FromBody] TaskItemDto requestTask)
        {            
            await _taskService.AddTask(requestTask);
            
            return Ok(new { 
                success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTaskState([FromBody] UpdateTaskStateDto dto)
        {
            await _taskService.UpdateTaskState(dto);
            
            return Ok(new
            {
                success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskItemDto dto)
        {            
            await _taskService.UpdateTask(dto);
            
            return Ok(new { 
                success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask([FromBody] DeleteTaskItemDto dto )
        {            
            await _taskService.DeleteTask(dto);
            
            return Ok(new { 
                success = true
            });
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
