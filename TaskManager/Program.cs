using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Abstractions;
using TaskManager.Data;
using TaskManager.Core.Mappers;
using TaskManager.Core.Services;
using TaskManager.Data.Repositories;
using TaskManager.Models;

namespace TaskManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<ITaskMapper, TaskItemMapper>();
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{Id?}");

            app.Run();
        }
    }
}
