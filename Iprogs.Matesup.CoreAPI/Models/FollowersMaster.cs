using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class FollowersMaster
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long FollowerUserId { get; set; }

    public DateTime FollowedOn { get; set; }

    public virtual UserProfileMaster FollowerUser { get; set; } = null!;

    public virtual UserProfileMaster User { get; set; } = null!;
}
