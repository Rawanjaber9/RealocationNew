using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Priority
{
    public int PriorityId { get; set; }

    public string? PriorityName { get; set; }

    public virtual ICollection<RelocationTask> RelocationTasks { get; set; } = new List<RelocationTask>();

    public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
}
