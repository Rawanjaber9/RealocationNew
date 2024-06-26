using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class TaskComment
{
    public int CommentId { get; set; }

    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
