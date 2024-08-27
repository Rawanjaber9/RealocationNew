using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class UserTask
{
    public int UserTaskId { get; set; }

    public int UserId { get; set; }

    public int? TaskId { get; set; }

    public string? TaskName { get; set; }

    public string? TaskDescription { get; set; }

    public bool IsRecommended { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Priority { get; set; }

    public string? PersonalNote { get; set; }

    public bool IsNewUserTask { get; set; }

    public bool WantsNotification { get; set; }

    public bool IsBeforeMove { get; set; }

    public virtual Priority? PriorityNavigation { get; set; }

    public virtual RelocationTask? Task { get; set; }

    public virtual User User { get; set; } = null!;
}
