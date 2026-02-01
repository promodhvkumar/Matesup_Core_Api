using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class LookupCity
{
    public int ID { get; set; }

    public string? Name { get; set; }

    public int StateID { get; set; }

    public virtual LookupState State { get; set; } = null!;

    public virtual ICollection<UserProfileMaster> UserProfileMaster { get; set; } = new List<UserProfileMaster>();
}
