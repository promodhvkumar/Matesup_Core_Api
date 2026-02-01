using Iprogs.Matesup.CoreAPI.Models;

namespace Iprogs.Matesup.Models
{
    public class ChatMasterDTO
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long ChatRoomId { get; set; }

        public string Message { get; set; } = null!;

        public DateTime SentOn { get; set; }

        public int MessageType { get; set; }

        public long? ReplyTo { get; set; }

        public virtual ICollection<ChatMasterAttachmentsDTO> ChatMasterAttachments { get; set; } = new List<ChatMasterAttachmentsDTO>();

        public virtual ChatRoomMasterDTO ChatRoom { get; set; } = null!;

        //public virtual ICollection<ChatMasterDTO> InverseReplyToNavigation { get; set; } = new List<ChatMasterDTO>();

        public virtual ChatMasterDTO? ReplyToNavigation { get; set; }

        public virtual UserProfileMasterDTO User { get; set; } = null!;
    }

    public class ChatMasterAttachmentsDTO
    {
        public long Id { get; set; }

        public string AttachmentName { get; set; } = null!;

        public string? FileName { get; set; }

        public string? FileExtn { get; set; }

        public byte[] Attachment { get; set; } = null!;

        public long ChatId { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }

        public virtual ChatMasterDTO Chat { get; set; } = null!;
    }
}
