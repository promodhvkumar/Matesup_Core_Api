namespace Iprogs.Matesup.Models
{
    
    
    public class UserProfileMasterDTO
    {
        public long UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string NickName { get; set; } = null!;

        public int Gender { get; set; }

        public int? City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        public int? RelationshipStatus { get; set; }

        public int? Age { get; set; }

        public DateOnly? DOB { get; set; }

        public string? Description { get; set; }

        //public virtual ActiveUsers? ActiveUsers { get; set; }

        //public virtual ICollection<ChatMasterDTO> ChatMaster { get; set; } = new List<ChatMasterDTO>();

        //public virtual ICollection<ChatRoomBlockedUsersDTO> ChatRoomBlockedUsersBlockedByNavigation { get; set; } = new List<ChatRoomBlockedUsersDTO>();

        //public virtual ICollection<ChatRoomBlockedUsersDTO> ChatRoomBlockedUsersBlockedUser { get; set; } = new List<ChatRoomBlockedUsersDTO>();

        //public virtual ICollection<ChatRoomMasterDTO> ChatRoomMasterCreatedByNavigation { get; set; } = new List<ChatRoomMasterDTO>();

        //public virtual ICollection<ChatRoomMasterDTO> ChatRoomMasterRoomOwner { get; set; } = new List<ChatRoomMasterDTO>();

        //public virtual ICollection<ChatRoomUserMappingDTO> ChatRoomUserMapping { get; set; } = new List<ChatRoomUserMappingDTO>();

        public virtual LookupCityDTO? CityNavigation { get; set; }

        public virtual LookupCountryDTO? CountryNavigation { get; set; }

        //public virtual ICollection<FollowersMasterDTO> FollowersMasterFollowerUser { get; set; } = new List<FollowersMasterDTO>();

        //public virtual ICollection<FollowersMaster> FollowersMasterUser { get; set; } = new List<FollowersMaster>();

        public virtual LookupGenderDTO GenderNavigation { get; set; } = null!;

        //public virtual ICollection<MegaPhoneMasterDTO> MegaPhoneMaster { get; set; } = new List<MegaPhoneMasterDTO>();

        //public virtual LookupRelationshipStatus? RelationshipStatusNavigation { get; set; }

        public virtual LookupStateDTO? StateNavigation { get; set; }

        //public virtual AspNetUsers User { get; set; } = null!;

        //public virtual ICollection<UserInterestedIn> UserInterestedIn { get; set; } = new List<UserInterestedIn>();

        public virtual UserPicsDTO? UserPics { get; set; }
    }

    
    
    public class UserPicsDTO
    {

        public long UserId { get; set; }

        public byte[]? ProfilePic { get; set; }

        public string? ProfilePicName { get; set; }

        public byte[]? CoverPic { get; set; }

        public string? CoverPicName { get; set; }

        public virtual UserProfileMasterDTO User { get; set; } = null!;
    }
}
