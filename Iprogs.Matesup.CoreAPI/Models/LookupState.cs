using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class LookupState
{
    public int ID { get; set; }

    public string? Name { get; set; }

    public int CountryID { get; set; }

    public virtual LookupCountry Country { get; set; } = null!;

    public virtual ICollection<LookupCity> LookupCity { get; set; } = new List<LookupCity>();

    public virtual ICollection<UserProfileMaster> UserProfileMaster { get; set; } = new List<UserProfileMaster>();
}
