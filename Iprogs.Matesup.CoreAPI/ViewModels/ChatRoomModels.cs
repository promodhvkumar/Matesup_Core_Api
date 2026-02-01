namespace Iprogs.Matesup.Models
{
    public class ChatRoomMasterDTO
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

        //public virtual ICollection<ChatMasterDTO> ChatMaster { get; set; } = new List<ChatMasterDTO>();

        //public virtual ICollection<ChatRoomBlockedUsersDTO> ChatRoomBlockedUsers { get; set; } = new List<ChatRoomBlockedUsersDTO>();

        public virtual LookupChatRoomPrivacyDTO ChatRoomPrivacyNavigation { get; set; } = null!;

        public virtual LookupChatRoomTypeDTO ChatRoomTypeNavigation { get; set; } = null!;

        public virtual ICollection<ChatRoomUserMappingDTO> ChatRoomUserMapping { get; set; } = new List<ChatRoomUserMappingDTO>();

        public virtual UserProfileMasterDTO CreatedByNavigation { get; set; } = null!;

        //public virtual ICollection<MegaPhoneMasterDTO> MegaPhoneMasterChatRoom { get; set; } = new List<MegaPhoneMasterDTO>();

        //public virtual ICollection<MegaPhoneMasterDTO> MegaPhoneMasterNewChatRoom { get; set; } = new List<MegaPhoneMasterDTO>();

        public virtual UserProfileMasterDTO RoomOwner { get; set; } = null!;
    }

    public class ChatRoomUserMappingDTO
    {
        public long Id { get; set; }

        public long ChatRoomId { get; set; }

        public long UserId { get; set; }

        public DateTime? LastSeen { get; set; }

        public virtual ChatRoomMasterDTO ChatRoom { get; set; } = null!;

        public virtual UserProfileMasterDTO User { get; set; } = null!;
    }

    public class ChatRoomBlockedUsersDTO
    {
        public long Id { get; set; }

        public long ChatRoomId { get; set; }

        public long BlockedUserId { get; set; }

        public long BlockedBy { get; set; }

        public DateTime BlockedOn { get; set; }

        public virtual UserProfileMasterDTO BlockedByNavigation { get; set; } = null!;

        public virtual UserProfileMasterDTO BlockedUser { get; set; } = null!;

        public virtual ChatRoomMasterDTO ChatRoom { get; set; } = null!;
    }

    public partial class MegaPhoneMasterDTO
    {
        public long Id { get; set; }

        public long ChatRoomId { get; set; }

        public long UserId { get; set; }

        public string Announcement { get; set; } = null!;

        public DateTime SentOn { get; set; }

        public long? NewChatRoomId { get; set; }

        public virtual ChatRoomMasterDTO ChatRoom { get; set; } = null!;

        public virtual ChatRoomMasterDTO? NewChatRoom { get; set; }

        public virtual UserProfileMasterDTO User { get; set; } = null!;
    }
}