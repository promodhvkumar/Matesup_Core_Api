using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class UserPics
{
    public long UserId { get; set; }

    public byte[]? ProfilePic { get; set; }

    public string? ProfilePicName { get; set; }

    public byte[]? CoverPic { get; set; }

    public string? CoverPicName { get; set; }

    public virtual UserProfileMaster User { get; set; } = null!;
}
