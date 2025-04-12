using System.Threading.Tasks;
using TaskManager.Abstractions;
using TaskManager.DTO;
using TaskManager.Models;

namespace TaskManager.Mappers
{
    public class TaskItemMapper : ITaskMapper
    {
        public TaskItem ToModel (TaskItemDto dto)
        {
            return new TaskItem
            {
                Id = dto.Id,
                TaskTitle = dto.TaskTitle,
                TaskDescription = dto.TaskDescription,
                IsClosed = dto.IsClosed
            };
        }

        public TaskItemDto ToGetDto(TaskItem model)
        {
            return new TaskItemDto
            {
                Id = model.Id,
                TaskTitle = model.TaskTitle,
                TaskDescription = model.TaskDescription,
                IsClosed = model.IsClosed
            };
        }

        public void MapUpdates(TaskItem model, UpdateTaskItemDto dto)
        {
            var dtoProps = typeof(UpdateTaskItemDto).GetProperties();

            foreach (var prop in dtoProps)
            {
                var dtoValue = prop.GetValue(dto);

                if (dtoValue is null) continue;
                
                var modelProperty = typeof(TaskItem).GetProperty(prop.Name);

                if (modelProperty != null && modelProperty.CanWrite)
                {
                    modelProperty.SetValue(model, dto);
                }
            }
        }
    }
}
