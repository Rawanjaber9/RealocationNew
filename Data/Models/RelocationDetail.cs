    using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class RelocationDetail
{
    public int RelocationId { get; set; }

    public int UserId { get; set; }

    public string DestinationCountry { get; set; } = null!;

    public DateTime MoveDate { get; set; }

    public bool HasChildren { get; set; }

    public DateTime CreatedAt { get; set; }
}
