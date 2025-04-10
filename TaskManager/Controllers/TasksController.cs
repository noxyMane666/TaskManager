using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using TaskManager.Data;
using TaskManager.DTO;
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
                            .Select(t => new TaskItemDto
                            {
                                Id = t.Id,
                                TaskTitle = t.TaskTitle,
                                TaskDescription = t.TaskDescription,
                                IsClosed = t.IsClosed
                            })
                            .ToListAsync();
    
            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTaskState([FromBody] UpdateTaskStateDto dto)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                                    .Where(e => e.Value?.Errors.Count > 0)
                                    .ToDictionary(
                                        kvp => kvp.Key,
                                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                                    );

                    _logger.LogWarning("Ошибка валидации при добавлении задачи: {@Errors}", errors);

                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var task = await _baseContext.Tasks.FirstOrDefaultAsync(t => t.Id == dto.Id);

                if (task is null)
                {
                    return Json(new
                    {
                        sucсess = false,
                        error = $"Задача с id {dto.Id} не найдена"
                    });
                }

                task.ChangeTaskState(dto.IsClosed);
                await _baseContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    taskId = dto.Id,
                    newState = dto.IsClosed
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления задачи {dto.Id}", (dto.Id));
                return Json(new
                {
                    success = false,
                    error = "Не удалось обновить задачу",
                    detailedError = ex.Message
                });
            }
            
;        }

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
