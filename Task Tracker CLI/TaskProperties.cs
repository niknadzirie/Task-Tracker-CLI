using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CLI
{
    internal class TaskProperties
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public TaskStatusEnum Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
