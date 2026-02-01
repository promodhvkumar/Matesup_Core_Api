using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class ChatRoomMaster
{
    public long Id { get; set; }

    public string? ChatRoomName { get; set; }

    public int ChatRoomType { get; set; }

    public int ChatRoomPrivacy { get; set; }

    public long CreatedBy { get; set; }

    public long RoomOwnerId { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public DateTime LastMessageOn { get; set; }

    public string? Description { get; set; }

    public string? PasswordHash { get; set; }

    public DateTime? PasswordChangedOn { get; set; }

    public virtual ICollection<ChatMaster> ChatMaster { get; set; } = new List<ChatMaster>();

    public virtual ICollection<ChatRoomBlockedUsers> ChatRoomBlockedUsers { get; set; } = new List<ChatRoomBlockedUsers>();

    public virtual LookupChatRoomPrivacy ChatRoomPrivacyNavigation { get; set; } = null!;

    public virtual LookupChatRoomType ChatRoomTypeNavigation { get; set; } = null!;

    public virtual ICollection<ChatRoomUserMapping> ChatRoomUserMapping { get; set; } = new List<ChatRoomUserMapping>();

    public virtual UserProfileMaster CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<MegaPhoneMaster> MegaPhoneMasterChatRoom { get; set; } = new List<MegaPhoneMaster>();

    public virtual ICollection<MegaPhoneMaster> MegaPhoneMasterNewChatRoom { get; set; } = new List<MegaPhoneMaster>();

    public virtual UserProfileMaster RoomOwner { get; set; } = null!;
}
