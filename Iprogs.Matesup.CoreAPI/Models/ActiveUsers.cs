using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class ActiveUsers
{
    public long UserId { get; set; }

    public DateTime LastActive { get; set; }

    public DateTime LastNotification { get; set; }

    public DateTime LastLogin { get; set; }

    public virtual UserProfileMaster User { get; set; } = null!;
}
