using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class NewUserTask
{
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? Priority { get; set; }

    public string PersonalNote { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual User User { get; set; } = null!;
}
