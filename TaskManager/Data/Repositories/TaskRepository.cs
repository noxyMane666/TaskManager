using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Abstractions;
using TaskManager.DTO;
using TaskManager.Models;

namespace TaskManager.Data.Repositories;

public class TaskRepository(AppDbContext appDbContext) : ITaskRepository
{
    private readonly AppDbContext _context = appDbContext;
    
    public async Task AddTaskAsync(TaskItem newTaskItem)
    {
        _context.Tasks.Add(newTaskItem);
        await _context.SaveChangesAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
    {
        return await _context.Tasks.FindAsync(taskId);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksAsync(bool isClosed)
    {
        return await _context.Tasks.Where(task => task.IsClosed == isClosed).ToListAsync();
    }

    public async Task UpdateTaskAsync(TaskItem taskItem)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(TaskItem deletedTask)
    {
        _context.Tasks.Remove(deletedTask);
        await _context.SaveChangesAsync();
    }
}