using AutoMapper;
using Iprogs.Matesup.CoreAPI.Models;
using Iprogs.Matesup.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Runtime.CompilerServices;

namespace Iprogs.Matesup.CoreAPI.Services
{
    public static class DBContextService
    {
        public static List<T> GetProcResults<T>(string ProcName, string[] parameters, IMapper mapper, DevContext dbContext)
        {
            var sqlparams = string.Join("','", parameters);

            FormattableString query = FormattableStringFactory.Create("exec " + ProcName + " '" + sqlparams.Trim('\'') + "'");

            var qry = query.Format;

            return dbContext.Database.SqlQuery<T>(query).ToList();

            //FormattableString query = FormattableStringFactory.Create(ProcName, parameters);

            //return dbContext.Database.SqlQuery<T>(query).ToList();

            //T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj ?? string.Empty);

            //return result;
        }

        public static List<T> GetTableFunctionResults<T>(string FuncName, string[] parameters, IMapper mapper, DevContext dbContext)
        {
            var sqlparams = string.Join("','", parameters);

            FormattableString query = FormattableStringFactory.Create("select * from " + FuncName + " ('" + sqlparams.Trim('\'') + "')");

            var qry = query.Format;

            return dbContext.Database.SqlQuery<T>(query).ToList();

            //return results;

            //var obj = dbContext.Database.ExecuteSqlRaw(string.Format("Select * from {0} ", query)).ToJson(Newtonsoft.Json.Formatting.None);

            //T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj ?? string.Empty);

            //return result;
        }

        public static T GetScalarFunctionResults<T>(string FuncName, string[] parameters, IMapper mapper, DevContext dbContext)
        {
            var sqlparams = string.Join("','", parameters);

            FormattableString query = FormattableStringFactory.Create("select " + FuncName + " ('" + sqlparams.Trim('\'') + "')");

            var qry = query.Format;

            return dbContext.Database.SqlQuery<T>(query).ToList().FirstOrDefault();

            //var obj = dbContext.Database.ExecuteSqlRaw(string.Format("Select {0} ", FuncName), parameters).ToJson(Newtonsoft.Json.Formatting.None);

            //T result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj ?? string.Empty);

            //return result;
        }

        public static List<GetActiveChatRoomsListModel> GetActiveChatRoomsList(long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveChatRoomsListModel>("GetActiveChatRoomsList", new string[] { UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetChatRoomsDetailsModel> GetChatRoomsDetails(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetChatRoomsDetailsModel>("GetChatRoomsDetails", new string[] { ChatRoomId.ToString(), UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetMyPrivateChatRoomsListModel> GetMyPrivateChatRoomsList(long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetMyPrivateChatRoomsListModel>("GetMyPrivateChatRoomsList", new string[] { UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetActiveChatRoomsListModel> GetMyownChatRoomsList(long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveChatRoomsListModel>("GetMyownChatRoomsList", new string[] { UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetActiveChatRoomsListModel> GetMyJoinedChatRoomsList(long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveChatRoomsListModel>("GetMyJoinedChatRoomsList", new string[] { UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetActiveChatRoomsListModel> GetPublicChatRoomsList(long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveChatRoomsListModel>("GetPublicChatRoomsList", new string[] { UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetActiveChatRoomsListModel> GetTrendingChatRoomsList(long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveChatRoomsListModel>("GetTrendingChatRoomsList", new string[] { UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetActiveChatRoomsListModel> GetUserChatRoomsList(long UserId, long CurrentUserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveChatRoomsListModel>("GetUserChatRoomsList", new string[] { UserId.ToString(), CurrentUserId.ToString() }, mapper, dbContext);
        }

        public static List<GetActiveChatRoomsListModel> GetPrivateChatRoomsDetails(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveChatRoomsListModel>("GetPrivateChatRoomsDetails", new string[] { ChatRoomId.ToString(), UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetActiveChatRoomsListModel> GetChatRoomsDetailsByName(string ChatRoomName, long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveChatRoomsListModel>("GetChatRoomsDetailsByName", new string[] { ChatRoomName, UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetActiveUsersListModel> GetActiveUsersList(int CityId, int StateId, int CountryId, long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveUsersListModel>("GetActiveUsersList", new string[] { CityId.ToString(), StateId.ToString(), CountryId.ToString(), UserId.ToString() }, mapper, dbContext);
        }

        public static List<GetUserDetailsModel> GetUserDetails(long UserId, long CurrentUserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetUserDetailsModel>("GetUserDetails", new string[] { UserId.ToString(), CurrentUserId.ToString() }, mapper, dbContext);
        }

        public static List<GetActiveChatRoomsListModel> GetSearchChatRoomsList(long UserId, IMapper mapper, DevContext dbContext)
        {
            return GetTableFunctionResults<GetActiveChatRoomsListModel>("GetSearchChatRoomsList", new string[] { UserId.ToString() }, mapper, dbContext);
        }

        public static List<int> MatesUpArenaAnnouncement(long UserId, string Announcement, long? ChatRoomId, IMapper mapper, DevContext dbContext)
        {
            return GetProcResults<int>("MatesUpArenaAnnouncement", new string[] { UserId.ToString(), Announcement, ChatRoomId.HasValue ? ChatRoomId.Value.ToString() : "0" }, mapper, dbContext);
        }

        public static List<int> ChatRoomAnnouncement(long ChatRoomId, long UserId, string Announcement, long? NewChatRoomId, IMapper mapper, DevContext dbContext)
        {
            return GetProcResults<int>("ChatRoomAnnouncement", new string[] { ChatRoomId.ToString(), UserId.ToString(), Announcement, NewChatRoomId.HasValue ? NewChatRoomId.Value.ToString() : "0" }, mapper, dbContext);
        }
    }

    #region Stored Procedure Models

    public class GetActiveChatRoomsListModel
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

        //public virtual LookupChatRoomPrivacy ChatRoomPrivacyNavigation { get; set; } = null!;

        //public virtual LookupChatRoomType ChatRoomTypeNavigation { get; set; } = null!;

        //public virtual UserProfileMaster CreatedByNavigation { get; set; } = null!;

        //public virtual UserProfileMaster RoomOwner { get; set; } = null!;

        public int? TotalUsers { get; set; }
        public int? ActiveUsers { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? NeedPassword { get; set; }
        public bool? PasswordProtected { get; set; }
        public bool? AlreadyMember { get; set; }
        public bool? IsRoomOwner { get; set; }
        public bool? Moderator { get; set; }
    }

    public class GetChatRoomsDetailsModel : GetActiveChatRoomsListModel
    {

    }

    public class GetMyPrivateChatRoomsListModel
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

        public int? NewMessagesCount { get; set; }
        public string RoomDisplayName { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? YouBlocked { get; set; }
    }

    public class GetActiveUsersListModel
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

        public DateTime? LastActive { get; set; }
        public int? LastSeenRank { get; set; }
        public int? CityRank { get; set; }
        public int? StateRank { get; set; }
        public int? CountryRank { get; set; }
        public int? Followers { get; set; }
        public int? Following { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? YouBlocked { get; set; }
        public bool? EmailConfirmed { get; set; }
        public bool? IsFollowing { get; set; }
    }

    public class GetUserDetailsModel
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

        public DateTime? LastActive { get; set; }
        public int? Followers { get; set; }
        public int? Following { get; set; }
        public bool? IsBlocked { get; set; }
        public bool? YouBlocked { get; set; }
        public bool? EmailConfirmed { get; set; }
        public bool? IsFollowing { get; set; }
    }

    #endregion Stored Procedure Models

}