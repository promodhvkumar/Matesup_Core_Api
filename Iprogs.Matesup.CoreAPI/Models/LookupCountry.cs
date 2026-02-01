using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class LookupCountry
{
    public int ID { get; set; }

    public string? Name { get; set; }

    public string CountryCode { get; set; } = null!;

    public virtual ICollection<LookupState> LookupState { get; set; } = new List<LookupState>();

    public virtual ICollection<UserProfileMaster> UserProfileMaster { get; set; } = new List<UserProfileMaster>();
}
