using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class AspNetUserLogins
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }

    public long UserId { get; set; }

    public virtual AspNetUsers User { get; set; } = null!;
}
