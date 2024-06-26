using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class UserCategory
{
    public int UserCategoryId { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
