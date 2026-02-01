using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class UserInterestedIn
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long InterestedIn { get; set; }

    public virtual UserProfileMaster User { get; set; } = null!;
}
