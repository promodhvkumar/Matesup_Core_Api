using System.Runtime.Serialization;

using Iprogs.Matesup.Models;

namespace Iprogs.Matesup.CoreAPI.Models
{
    public class ChatMessageModel
    {
        public long ChatRoomId { get; set; }
        //[AllowHtml]
        public string Message { get; set; }
    }

    public class NewChatRoomModel
    {
        public long ChatRoomId { get; set; }
        public string ChatRoomName { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public int ChatRoomPrivacy { get; set; }
        public int ChatRoomType { get; set; }
    }

    public class TextModel
    {
        public string Content { get; set; }
    }

    public class UserModalModel
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public int Gender { get; set; }
        public virtual LookupStateDTO? StateNavigation { get; set; }
        public virtual LookupCityDTO? CityNavigation { get; set; }
        public virtual LookupCountryDTO? CountryNavigation { get; set; }
        public virtual LookupGenderDTO GenderNavigation { get; set; } = null!;
        public string? Description { get; set; }
        public bool VerifiedUser { get; set; }
        public bool Online { get; set; }
        public bool IsBlocked { get; set; }
        public bool YouBlocked { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public bool IsFollowing { get; set; }
        public DateOnly? DOB { get; set; }
    }

    public class RoomModalModel
    {
        public long Id { get; set; }
        public string ChatRoomName { get; set; }
        public long RoomOwnerId { get; set; }
        public string Description { get; set; }
        public bool NeedPassword { get; set; }
        public bool PasswordProtected { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalUsers { get; set; }
        public int RoomPrivacy { get; set; }
        public int RoomType { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsRoomOwner { get; set; }
        public bool Moderator { get; set; }
        public bool AlreadyMember { get; set; }

        public ChatRoomAccessModel UserAccess { get; set; }
    }

    public class ChatMessagesModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ChatRoomId { get; set; }
        public string Message { get; set; }
        public DateTime SentOn { get; set; }
        public bool IsNew { get; set; }
        public virtual UserModalModel UserProfileMaster { get; set; }
    }

    public class AnnouncementsModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long ChatRoomId { get; set; }
        public string Announcement { get; set; }
        public DateTime SentOn { get; set; }
        public bool IsNew { get; set; }
        public virtual UserModalModel UserProfileMaster { get; set; }
        public virtual RoomModalModel NewRoom { get; set; }
    }

    public class ChatRoomAccessModel
    {
        public long ChatRoomId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsRoomOwner { get; set; }
        public bool IsModerator { get; set; }
        public bool IsActiveUser { get; set; }
    }

    public class PrivateChatRoomModel
    {
        public long ChatRoomId { get; set; }
        public DateTime? LastMessageOn { get; set; }
        public int NewMessageCount { get; set; }
        public string RoomName { get; set; }
        public bool IsBlocked { get; set; }
        public bool YouBlocked { get; set; }
    }

    public class UserSearchModel
    {
        public string SearchTerm { get; set; }
        public int Gender { get; set; }
        public bool OnlyOnline { get; set; }
        public bool OnlyVerifiedUsers { get; set; }
    }

    public class RoomSearchModel
    {
        public string SearchTerm { get; set; }
        public bool SkipAlreadyOwned { get; set; }
        public bool SkipAlreadyJoined { get; set; }
    }

    public class UserContext
    {
        public long UserId { get; set; }
        public UserModalModel UserProfile { get; set; }
    }

    public class ResponseModel
    {
        public string status { get; set; }
        public string responseText { get; set; }
        public object Data { get; set; }
        public bool Validation { get; set; }
    }

    public class NotificationModel
    {
        public int NewMessageCount { get; set; }
        public int ChatsCount { get; set; }
        public List<PrivateChatRoomModel>? TaggedChats { get; set; }
    }
}