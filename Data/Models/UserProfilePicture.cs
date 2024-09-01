using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class UserProfilePicture
{
    public int UserProfilePictureId { get; set; }

    public int UserId { get; set; }

    public byte[] ProfilePicture { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
