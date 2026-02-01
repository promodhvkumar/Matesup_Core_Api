using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class ChatMaster
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long ChatRoomId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime SentOn { get; set; }

    public int MessageType { get; set; }

    public long? ReplyTo { get; set; }

    public virtual ICollection<ChatMasterAttachments> ChatMasterAttachments { get; set; } = new List<ChatMasterAttachments>();

    public virtual ChatRoomMaster ChatRoom { get; set; } = null!;

    public virtual ICollection<ChatMaster> InverseReplyToNavigation { get; set; } = new List<ChatMaster>();

    public virtual ChatMaster? ReplyToNavigation { get; set; }

    public virtual UserProfileMaster User { get; set; } = null!;
}
