using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using TaskManager.Data;
using TaskManager.DTO;
using TaskManager.Mappers;
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

                    _logger.LogError("Ошибка валидации при добавлении задачи: {@Errors}", errors);

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
                    return StatusCode(404, $"Задача с id {dto.Id} не найдена");
                }

                task.ChangeTaskState(dto.IsClosed);
                await _baseContext.SaveChangesAsync();

                return Ok(new { success = true, updatedTask = dto });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления статуса задачи {dto.Id}", (dto.Id));
                return StatusCode(500, $"При обновлении статуса задачи произошла ошибка: {ex.Message}");
            }
            
;        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskItemDto dto)
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

                    _logger.LogError("Ошибка валидации при добавлении задачи: {@Errors}", errors);

                    return BadRequest(new
                    {
                        success = false,
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var task = await _baseContext.Tasks.FindAsync(dto.Id);

                if (task is null)
                {
                    return StatusCode(404, $"Задача с id {dto.Id} не найдена");
                }

                TaskItemMapper.MapUpdates(task, dto);

                await _baseContext.SaveChangesAsync();
                return Ok(new { success = true, updatedTask = dto });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка обновления задачи {dto.Id}", (dto.Id));
                return StatusCode(500, $"При обновлении задачи произошла ошибка: {ex.Message}");
            }
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
