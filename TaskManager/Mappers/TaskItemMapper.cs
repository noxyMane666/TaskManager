using System.Threading.Tasks;
using TaskManager.DTO;
using TaskManager.Models;

namespace TaskManager.Mappers
{
    public static class TaskItemMapper
    {
        public static TaskItem ToModel (TaskItemDto dto)
        {
            return new TaskItem
            {
                Id = dto.Id,
                TaskTitle = dto.TaskTitle,
                TaskDescription = dto.TaskDescription,
                IsClosed = dto.IsClosed
            };
        }

        public static TaskItemDto ToGetDto(TaskItem model)
        {
            return new TaskItemDto
            {
                Id = model.Id,
                TaskTitle = model.TaskTitle,
                TaskDescription = model.TaskDescription,
                IsClosed = model.IsClosed
            };
        }

        public static void MapUpdates(TaskItem model, UpdateTaskItemDto dto)
        {
            try
            {
                var dtoProps = typeof(UpdateTaskItemDto).GetProperties();

                foreach (var prop in dtoProps)
                {
                    var dtoValue = prop.GetValue(dto);

                    if (dtoValue is not null)
                    {
                        var modelProperty = typeof(TaskItem).GetProperty(prop.Name);

                        if (modelProperty != null && modelProperty.CanWrite)
                        {
                            modelProperty.SetValue(model, dto);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
