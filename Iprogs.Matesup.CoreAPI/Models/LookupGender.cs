using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class LookupGender
{
    public int Id { get; set; }

    public string Gender { get; set; } = null!;

    public string GenderIcon { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<UserProfileMaster> UserProfileMaster { get; set; } = new List<UserProfileMaster>();
}
