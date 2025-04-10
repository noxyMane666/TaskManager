using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.DTO;
using TaskManager.Mappers;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class HomeController(ILogger<HomeController> logger, AppDbContext context) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly AppDbContext _context = context;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AddUserTask([FromBody] TaskItemDto requestTask)
        {
            try
            {
                Console.WriteLine(requestTask);
                if (ModelState.IsValid)
                {
                    if (await _context.Tasks.AnyAsync(t => t.Id == requestTask.Id))
                    {
                        return Json(new
                        {
                            success = false,
                            error = $"������ � ID {requestTask.Id} ��� ����������"
                        });
                    }

                    var taskModel = TaskItemMapper.ToModel(requestTask);

                    _context.Tasks.Add(taskModel);
                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        task = requestTask
                    });
                }
                else
                {
                    var errors = ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    _logger.LogWarning("������ ��������� ��� ���������� ������: {@Errors}", errors);

                    return Json(new
                    {
                        success = false,
                        error = "�������� ������ ������",
                        validationErrors = errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "������ ���������� ������");
                return Json(new
                {
                    success = false,
                    error = "���������� ������ �������",
                    detailedError = ex.Message
                });
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
