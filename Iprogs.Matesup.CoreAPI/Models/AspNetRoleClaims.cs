using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class AspNetRoleClaims
{
    public int Id { get; set; }

    public long RoleId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual AspNetRoles Role { get; set; } = null!;
}
