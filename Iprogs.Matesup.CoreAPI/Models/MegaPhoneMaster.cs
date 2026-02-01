using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class MegaPhoneMaster
{
    public long Id { get; set; }

    public long ChatRoomId { get; set; }

    public long UserId { get; set; }

    public string Announcement { get; set; } = null!;

    public DateTime SentOn { get; set; }

    public long? NewChatRoomId { get; set; }

    public virtual ChatRoomMaster ChatRoom { get; set; } = null!;

    public virtual ChatRoomMaster? NewChatRoom { get; set; }

    public virtual UserProfileMaster User { get; set; } = null!;
}
