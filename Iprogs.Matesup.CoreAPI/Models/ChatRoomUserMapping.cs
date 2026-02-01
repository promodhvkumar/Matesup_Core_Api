using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class ChatRoomUserMapping
{
    public long Id { get; set; }

    public long ChatRoomId { get; set; }

    public long UserId { get; set; }

    public DateTime? LastSeen { get; set; }

    public virtual ChatRoomMaster ChatRoom { get; set; } = null!;

    public virtual UserProfileMaster User { get; set; } = null!;
}
