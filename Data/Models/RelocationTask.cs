using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class RelocationTask
{
    public int TaskId { get; set; }

    public int CategoryId { get; set; }

    public string RecommendedTask { get; set; } = null!;

    public bool IsBeforeMove { get; set; }
}
