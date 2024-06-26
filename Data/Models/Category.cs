using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UserCategory> UserCategories { get; set; } = new List<UserCategory>();
}
