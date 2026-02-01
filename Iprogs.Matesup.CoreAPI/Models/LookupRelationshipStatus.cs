using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class LookupRelationshipStatus
{
    public int Id { get; set; }

    public string RelationshipStatus { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<UserProfileMaster> UserProfileMaster { get; set; } = new List<UserProfileMaster>();
}
