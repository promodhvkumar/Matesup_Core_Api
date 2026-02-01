using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class UserProfileMaster
{
    public long UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string NickName { get; set; } = null!;

    public int Gender { get; set; }

    public int? City { get; set; }

    public int? State { get; set; }

    public int? Country { get; set; }

    public int? RelationshipStatus { get; set; }

    public int? Age { get; set; }

    public DateOnly? DOB { get; set; }

    public string? Description { get; set; }

    public virtual ActiveUsers? ActiveUsers { get; set; }

    public virtual ICollection<ChatMaster> ChatMaster { get; set; } = new List<ChatMaster>();

    public virtual ICollection<ChatRoomBlockedUsers> ChatRoomBlockedUsersBlockedByNavigation { get; set; } = new List<ChatRoomBlockedUsers>();

    public virtual ICollection<ChatRoomBlockedUsers> ChatRoomBlockedUsersBlockedUser { get; set; } = new List<ChatRoomBlockedUsers>();

    public virtual ICollection<ChatRoomMaster> ChatRoomMasterCreatedByNavigation { get; set; } = new List<ChatRoomMaster>();

    public virtual ICollection<ChatRoomMaster> ChatRoomMasterRoomOwner { get; set; } = new List<ChatRoomMaster>();

    public virtual ICollection<ChatRoomUserMapping> ChatRoomUserMapping { get; set; } = new List<ChatRoomUserMapping>();

    public virtual LookupCity? CityNavigation { get; set; }

    public virtual LookupCountry? CountryNavigation { get; set; }

    public virtual ICollection<FollowersMaster> FollowersMasterFollowerUser { get; set; } = new List<FollowersMaster>();

    public virtual ICollection<FollowersMaster> FollowersMasterUser { get; set; } = new List<FollowersMaster>();

    public virtual LookupGender GenderNavigation { get; set; } = null!;

    public virtual ICollection<MegaPhoneMaster> MegaPhoneMaster { get; set; } = new List<MegaPhoneMaster>();

    public virtual LookupRelationshipStatus? RelationshipStatusNavigation { get; set; }

    public virtual LookupState? StateNavigation { get; set; }

    public virtual AspNetUsers User { get; set; } = null!;

    public virtual ICollection<UserInterestedIn> UserInterestedIn { get; set; } = new List<UserInterestedIn>();

    public virtual UserPics? UserPics { get; set; }
}
