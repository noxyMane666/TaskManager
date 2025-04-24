using System.Threading.Tasks;
using TaskManager.Core.Abstractions;
using TaskManager.DTO;
using TaskManager.Models;

namespace TaskManager.Core.Mappers
{
    public class TaskItemMapper : ITaskMapper
    {
        public TaskItem ToModel (TaskItemDto dto, int userId)
        {
            return new TaskItem
            {
                Id = dto.Id,
                UserId = userId,
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

        public TaskItem MapUpdates(TaskItem model, UpdateTaskItemDto dto)
        {
            var dtoProps = typeof(UpdateTaskItemDto).GetProperties();
            foreach (var prop in dtoProps)
            {
                var dtoValue = prop.GetValue(dto);

                if (dtoValue is null) continue;
                
                var modelProperty = typeof(TaskItem).GetProperty(prop.Name);

                if (modelProperty != null && modelProperty.CanWrite)
                {
                    modelProperty.SetValue(model, dtoValue);
                }
            }

            return model;
        }

        public IEnumerable<TaskItemDto> ToDtoList(IEnumerable<TaskItem> models)
        {
            var dtoList = models.Select(ToGetDto).ToList();
            return dtoList;
        }
    }
}
