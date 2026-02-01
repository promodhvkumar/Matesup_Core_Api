using System;
using System.Collections.Generic;

namespace Iprogs.Matesup.CoreAPI.Models;

public partial class ChatMasterAttachments
{
    public long Id { get; set; }

    public string AttachmentName { get; set; } = null!;

    public string? FileName { get; set; }

    public string? FileExtn { get; set; }

    public byte[] Attachment { get; set; } = null!;

    public long ChatId { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsActive { get; set; }

    public virtual ChatMaster Chat { get; set; } = null!;
}
