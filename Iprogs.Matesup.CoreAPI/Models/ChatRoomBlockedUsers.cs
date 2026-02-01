using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class ChatRoomBlockedUsers
{
    public long Id { get; set; }

    public long ChatRoomId { get; set; }

    public long BlockedUserId { get; set; }

    public long BlockedBy { get; set; }

    public DateTime BlockedOn { get; set; }

    public virtual UserProfileMaster BlockedByNavigation { get; set; } = null!;

    public virtual UserProfileMaster BlockedUser { get; set; } = null!;

    public virtual ChatRoomMaster ChatRoom { get; set; } = null!;
}
