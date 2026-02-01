
namespace Iprogs.Matesup.Models
{
    
    
    public class LookupGenderDTO
    {

        public int Id { get; set; }

        public string Gender { get; set; } = null!;

        public string GenderIcon { get; set; } = null!;

        public bool IsActive { get; set; }

        //public virtual ICollection<UserProfileMasterDTO> UserProfileMaster { get; set; } = new List<UserProfileMasterDTO>();
    }

    
    
    public class LookupCountryDTO
    {

        public int ID { get; set; }

        public string? Name { get; set; }

        public string CountryCode { get; set; } = null!;

        //public virtual ICollection<LookupStateDTO> LookupState { get; set; } = new List<LookupStateDTO>();

        //public virtual ICollection<UserProfileMasterDTO> UserProfileMaster { get; set; } = new List<UserProfileMasterDTO>();
    }

    
    
    public partial class LookupStateDTO
    {

        public int ID { get; set; }

        public string? Name { get; set; }

        public int CountryID { get; set; }

        //public virtual LookupCountryDTO Country { get; set; } = null!;

        //public virtual ICollection<LookupCityDTO> LookupCity { get; set; } = new List<LookupCityDTO>();

        //public virtual ICollection<UserProfileMasterDTO> UserProfileMaster { get; set; } = new List<UserProfileMasterDTO>();
    }

    
    
    public partial class LookupCityDTO
    {

        public int ID { get; set; }

        public string? Name { get; set; }

        public int StateID { get; set; }

        //public virtual LookupStateDTO State { get; set; } = null!;

        //public virtual ICollection<UserProfileMasterDTO> UserProfileMaster { get; set; } = new List<UserProfileMasterDTO>();
    }

    
    
    public class LookupChatRoomPrivacyDTO
    {

        public int Id { get; set; }

        public string ChatRoomPrivacy { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? Description { get; set; }

        //public virtual ICollection<ChatRoomMasterDTO> ChatRoomMaster { get; set; } = new List<ChatRoomMasterDTO>();
    }

    
    
    public class LookupChatRoomTypeDTO
    {

        public int Id { get; set; }

        public string ChatRoomType { get; set; } = null!;

        public bool IsActive { get; set; }

        public string? Description { get; set; }

        public bool NeedPassword { get; set; }

        //public virtual ICollection<ChatRoomMasterDTO> ChatRoomMaster { get; set; } = new List<ChatRoomMasterDTO>();
    }
}