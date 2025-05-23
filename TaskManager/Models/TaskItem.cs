﻿using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required string TaskTitle { get; set; }
        public required string TaskDescription { get; set; }
        public bool IsClosed { get; set; } = false;
        public DateTime CreatedDate { get; set; }  = DateTime.UtcNow;
        
        public void ChangeTaskState(bool stateBool)
        {
            IsClosed = stateBool;
        }
    }
}
