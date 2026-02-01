using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class LookupChatRoomType
{
    public int Id { get; set; }

    public string ChatRoomType { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    public bool NeedPassword { get; set; }

    public virtual ICollection<ChatRoomMaster> ChatRoomMaster { get; set; } = new List<ChatRoomMaster>();
}
