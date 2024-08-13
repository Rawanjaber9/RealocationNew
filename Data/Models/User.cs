using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool HasAcceptedTerms { get; set; }

    public virtual ICollection<NewUserTask> NewUserTasks { get; set; } = new List<NewUserTask>();

    public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

    public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
}
