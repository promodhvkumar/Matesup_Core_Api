using AutoMapper;
using Iprogs.Matesup.CoreAPI.Models;
using Iprogs.Matesup.Models;

using System.Collections;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Iprogs.Matesup.CoreAPI.Utilities;

namespace Iprogs.Matesup.CoreAPI.Services
{
    public static class ChatRoomService
    {
        public static List<UserModalModel> GetChatRoomUsersByChatRoomId(long ChatRoomId, IMapper mapper, DevContext dbContext)
        {
            var _list = new List<UserModalModel>();
            try
            {
                var _users = dbContext.ChatRoomUserMapping.
                                    Include(_ => _.User).
                                    Include(_ => _.User.Gender).
                                    Include(_ => _.User.Country).
                                    Include(_ => _.User.State).
                                    Include(_ => _.User.City).
                                    Include(_ => _.User.RelationshipStatus).
                                    Include(_ => _.User.ActiveUsers).
                                    Include(_ => _.User.User).
                                    Include(_ => _.ChatRoom.ChatRoomBlockedUsers).
                                    Where(_ => _.ChatRoomId == ChatRoomId).
                                    Select(_ => _).
                                    ToList().
                                    Select(_ => new UserModalModel()
                                    {
                                        UserId = _.UserId,
                                        FirstName = _.User.FirstName,
                                        LastName = _.User.LastName,
                                        NickName = _.User.NickName,
                                        Description = _.User.Description,
                                        CityNavigation = mapper.Map<LookupCity, LookupCityDTO>(_.User.CityNavigation),
                                        CountryNavigation = mapper.Map<LookupCountry, LookupCountryDTO>(_.User.CountryNavigation),
                                        StateNavigation = mapper.Map<LookupState, LookupStateDTO>(_.User.StateNavigation),
                                        GenderNavigation = mapper.Map<LookupGender, LookupGenderDTO>(_.User.GenderNavigation),
                                        VerifiedUser = _.User.User.EmailConfirmed,
                                        Online = _.User.ActiveUsers.LastActive >= DateTime.UtcNow.AddMinutes(-15) ? true : false,
                                        IsBlocked = _.ChatRoom.ChatRoomBlockedUsers.Any(x => x.BlockedUserId == _.UserId)
                                    }).
                                    ToList();

                _list = _users;
                //.Select(_ => Mapper.Map<UserProfileMaster, UserModalModel>(_.UserProfileMaster)).ToList();
            }
            catch (Exception ex)
            {
                throw;
                //LogService.WriteErrorLog("ChatRoomService.GetChatRoomUsersByChatRoomId", ex);
            }

            return _list;
        }

        public static ChatRoomMasterDTO GetChatRoomByChatRoomName(string ChatRoomName, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _rooms = dbContext.ChatRoomMaster.
                                    Where(_ => _.ChatRoomName.ToLower() == ChatRoomName.ToLower()).
                                    Select(_ => _).
                                    ToList();
                if (_rooms.Any())
                {
                    return _rooms.Select(_ => mapper.Map<ChatRoomMaster, ChatRoomMasterDTO>(_)).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
                //LogService.WriteErrorLog("ChatRoomService.GetChatRoomByChatRoomName", ex);
            }

            return null;
        }

        public static List<RoomModalModel> GetMyActiveChatRoomsByUserId(long UserId, IMapper mapper, DevContext dbContext)
        {
            var _list = new List<RoomModalModel>();
            try
            {
                var _rooms = DBContextService.GetActiveChatRoomsList(UserId, mapper, dbContext).
                                    Take(20).
                                    Select(_ => _).
                                    ToList().
                                    Select(_ => new RoomModalModel()
                                    {
                                        ActiveUsers = _.ActiveUsers.HasValue ? _.ActiveUsers.Value : 0,
                                        ChatRoomName = _.ChatRoomName,
                                        Description = _.Description,
                                        RoomPrivacy = _.ChatRoomPrivacy,
                                        RoomType = _.ChatRoomType,
                                        Id = _.Id,
                                        NeedPassword = _.NeedPassword.HasValue ? _.NeedPassword.Value : false,
                                        PasswordProtected = _.PasswordProtected.HasValue ? _.PasswordProtected.Value : false,
                                        RoomOwnerId = _.RoomOwnerId,
                                        TotalUsers = _.TotalUsers.HasValue ? _.TotalUsers.Value : 0,
                                        AlreadyMember = _.AlreadyMember.HasValue ? _.AlreadyMember.Value : false,
                                        IsBlocked = _.IsBlocked.HasValue ? _.IsBlocked.Value : false,
                                        Moderator = _.Moderator.HasValue ? _.Moderator.Value : false,
                                        IsRoomOwner = _.IsRoomOwner.HasValue ? _.IsRoomOwner.Value : false
                                    }).
                                    OrderByDescending(_ => _.ActiveUsers).
                                    ToList();

                _list = _rooms;
            }
            catch (Exception ex)
            {
                throw;
                //LogService.WriteErrorLog("ChatRoomService.GetMyActiveChatRoomsByUserId", ex);
            }

            return _list;
        }

        public static List<PrivateChatRoomModel> GetMyPrivateChatRooms(long UserId, int Paging, string SearchTerm, IMapper mapper, DevContext dbContext)
        {
            var _result = new List<PrivateChatRoomModel>();

            try
            {
                var _chatRooms = DBContextService.GetMyPrivateChatRoomsList(UserId, mapper, dbContext).
                                Where(_ => SearchTerm != null ? _.RoomDisplayName.Contains(SearchTerm) : true).
                                OrderByDescending(_ => _.NewMessagesCount).
                                ThenByDescending(_ => _.LastMessageOn).
                                Select(_ => _).
                                Take(Paging).
                                ToList().
                                Select(_ => new PrivateChatRoomModel()
                                {
                                    ChatRoomId = _.Id,
                                    LastMessageOn = _.LastMessageOn,
                                    NewMessageCount = _.NewMessagesCount.HasValue ? _.NewMessagesCount.Value : 0,
                                    RoomName = _.RoomDisplayName,
                                    IsBlocked = _.IsBlocked.HasValue ? _.IsBlocked.Value : false,
                                    YouBlocked = _.YouBlocked.HasValue ? _.YouBlocked.Value : false
                                }).
                                ToList();

                _result = _chatRooms.ToList();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetMyPrivateChatRooms", ex);
            }

            return _result;
        }

        public static ChatRoomMasterDTO CreatePrivateChatRoom(ChatRoomMasterDTO NewChatRoom, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _newChatRoom = new ChatRoomMaster();

                _newChatRoom = mapper.Map<ChatRoomMasterDTO, ChatRoomMaster>(NewChatRoom);

                dbContext.ChatRoomMaster.Add(_newChatRoom);

                dbContext.SaveChanges();

                return mapper.Map<ChatRoomMaster, ChatRoomMasterDTO>(_newChatRoom);
            }
            catch (Exception ex)
            {
                throw;
                //LogService.WriteErrorLog("ChatRoomService.CreatePrivateChatRoom", ex);
            }

            return null;
        }

        public static ChatRoomMasterDTO CreateChatRoom(ChatRoomMasterDTO NewChatRoom, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _newChatRoom = new ChatRoomMaster();

                if (NewChatRoom.Id > 0)
                {
                    var _existingRoom = dbContext.ChatRoomMaster.Where(_ => _.Id == NewChatRoom.Id).Select(_ => _).FirstOrDefault();

                    if (_existingRoom != null)
                    {
                        //_existingRoom = Mapper.Map<ChatRoomMasterDTO, ChatRoomMaster>(NewChatRoom);
                        _existingRoom.ChatRoomName = NewChatRoom.ChatRoomName;
                        _existingRoom.ChatRoomPrivacy = NewChatRoom.ChatRoomPrivacy;
                        _existingRoom.ChatRoomType = NewChatRoom.ChatRoomType;
                        _existingRoom.Description = NewChatRoom.Description;
                        _existingRoom.PasswordChangedOn = NewChatRoom.PasswordChangedOn;
                        _existingRoom.PasswordHash = NewChatRoom.PasswordHash;

                        dbContext.SaveChanges();

                        _newChatRoom = _existingRoom;
                    }
                }
                else
                {
                    _newChatRoom = mapper.Map<ChatRoomMasterDTO, ChatRoomMaster>(NewChatRoom);

                    dbContext.ChatRoomMaster.Add(_newChatRoom);
                    dbContext.SaveChanges();
                }

                return mapper.Map<ChatRoomMaster, ChatRoomMasterDTO>(_newChatRoom);
            }
            catch (Exception ex)
            {
                throw;
                //LogService.WriteErrorLog("ChatRoomService.CreateChatRoom", ex);
            }

            return null;
        }

        public static List<RoomModalModel> GetMyOwnChatRooms(long UserId, IMapper mapper, DevContext dbContext)
        {
            var _result = new List<RoomModalModel>();

            try
            {
                var _chatRooms = DBContextService.GetMyownChatRoomsList(UserId, mapper, dbContext).
                                Where(_ => _.RoomOwnerId == UserId && _.IsActive).
                                Select(_ => _).
                                ToList().
                                Select(_ => new RoomModalModel()
                                {
                                    ActiveUsers = _.ActiveUsers.HasValue ? _.ActiveUsers.Value : 0,
                                    ChatRoomName = _.ChatRoomName,
                                    Description = _.Description,
                                    RoomPrivacy = _.ChatRoomPrivacy,
                                    RoomType = _.ChatRoomType,
                                    Id = _.Id,
                                    NeedPassword = _.NeedPassword.HasValue ? _.NeedPassword.Value : false,
                                    PasswordProtected = _.PasswordProtected.HasValue ? _.PasswordProtected.Value : false,
                                    RoomOwnerId = _.RoomOwnerId,
                                    TotalUsers = _.TotalUsers.HasValue ? _.TotalUsers.Value : 0,
                                    AlreadyMember = _.AlreadyMember.HasValue ? _.AlreadyMember.Value : false,
                                    IsBlocked = _.IsBlocked.HasValue ? _.IsBlocked.Value : false,
                                    Moderator = _.Moderator.HasValue ? _.Moderator.Value : false,
                                    IsRoomOwner = _.IsRoomOwner.HasValue ? _.IsRoomOwner.Value : false
                                }).
                                ToList();

                    _result = _chatRooms;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetMyOwnChatRooms", ex);
            }

            return _result;
        }

        public static List<RoomModalModel> GetMyJoinedChatRooms(long UserId, IMapper mapper, DevContext dbContext)
        {
            var _result = new List<RoomModalModel>();

            try
            {
                var _chatRooms = DBContextService.GetMyJoinedChatRoomsList(UserId, mapper, dbContext).
                                Where(_ => _.RoomOwnerId != UserId && _.IsActive).
                                Select(_ => _).
                                ToList().
                                Select(_ => new RoomModalModel()
                                {
                                    ActiveUsers = _.ActiveUsers.HasValue ? _.ActiveUsers.Value : 0,
                                    ChatRoomName = _.ChatRoomName,
                                    Description = _.Description,
                                    RoomPrivacy = _.ChatRoomPrivacy,
                                    RoomType = _.ChatRoomType,
                                    Id = _.Id,
                                    NeedPassword = _.NeedPassword.HasValue ? _.NeedPassword.Value : false,
                                    PasswordProtected = _.PasswordProtected.HasValue ? _.PasswordProtected.Value : false,
                                    RoomOwnerId = _.RoomOwnerId,
                                    TotalUsers = _.TotalUsers.HasValue ? _.TotalUsers.Value : 0,
                                    AlreadyMember = _.AlreadyMember.HasValue ? _.AlreadyMember.Value : false,
                                    IsBlocked = _.IsBlocked.HasValue ? _.IsBlocked.Value : false,
                                    Moderator = _.Moderator.HasValue ? _.Moderator.Value : false,
                                    IsRoomOwner = _.IsRoomOwner.HasValue ? _.IsRoomOwner.Value : false
                                }).
                                ToList();

                    _result = _chatRooms;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetMyJoinedChatRooms", ex);
            }

            return _result;
        }

        public static List<RoomModalModel> GetPublicChatRooms(long UserId, int paging, IMapper mapper, DevContext dbContext)
        {
            var _result = new List<RoomModalModel>();

            try
            {
                var _chatRooms = DBContextService.GetPublicChatRoomsList(UserId, mapper, dbContext).
                                Where(_ => _.IsActive && !_.IsBlocked.Value).
                                Select(_ => _).
                                OrderByDescending(_ => _.ActiveUsers).
                                ThenByDescending(_ => _.TotalUsers).
                                Take(paging).
                                ToList().
                                Select(_ => new RoomModalModel()
                                {
                                    ActiveUsers = _.ActiveUsers.HasValue ? _.ActiveUsers.Value : 0,
                                    ChatRoomName = _.ChatRoomName,
                                    Description = _.Description,
                                    RoomPrivacy = _.ChatRoomPrivacy,
                                    RoomType = _.ChatRoomType,
                                    Id = _.Id,
                                    NeedPassword = _.NeedPassword.HasValue ? _.NeedPassword.Value : false,
                                    PasswordProtected = _.PasswordProtected.HasValue ? _.PasswordProtected.Value : false,
                                    RoomOwnerId = _.RoomOwnerId,
                                    TotalUsers = _.TotalUsers.HasValue ? _.TotalUsers.Value : 0,
                                    AlreadyMember = _.AlreadyMember.HasValue ? _.AlreadyMember.Value : false,
                                    IsBlocked = _.IsBlocked.HasValue ? _.IsBlocked.Value : false,
                                    Moderator = _.Moderator.HasValue ? _.Moderator.Value : false,
                                    IsRoomOwner = _.IsRoomOwner.HasValue ? _.IsRoomOwner.Value : false
                                }).
                                ToList();

                    _result = _chatRooms;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetPublicChatRooms", ex);
            }

            return _result;
        }

        public static List<RoomModalModel> GetTrendingChatRooms(long UserId, int paging, IMapper mapper, DevContext dbContext)
        {
            var _result = new List<RoomModalModel>();

            try
            {
                var _chatRooms = DBContextService.GetTrendingChatRoomsList(UserId, mapper, dbContext).
                                Where(_ => _.IsActive && !_.IsBlocked.Value).
                                Select(_ => _).
                                OrderByDescending(_ => _.ActiveUsers).
                                ThenByDescending(_ => _.TotalUsers).
                                Take(paging).
                                ToList().
                                Select(_ => new RoomModalModel()
                                {
                                    ActiveUsers = _.ActiveUsers.HasValue ? _.ActiveUsers.Value : 0,
                                    ChatRoomName = _.ChatRoomName,
                                    Description = _.Description,
                                    RoomPrivacy = _.ChatRoomPrivacy,
                                    RoomType = _.ChatRoomType,
                                    Id = _.Id,
                                    NeedPassword = _.NeedPassword.HasValue ? _.NeedPassword.Value : false,
                                    PasswordProtected = _.PasswordProtected.HasValue ? _.PasswordProtected.Value : false,
                                    RoomOwnerId = _.RoomOwnerId,
                                    TotalUsers = _.TotalUsers.HasValue ? _.TotalUsers.Value : 0,
                                    AlreadyMember = _.AlreadyMember.HasValue ? _.AlreadyMember.Value : false,
                                    IsBlocked = _.IsBlocked.HasValue ? _.IsBlocked.Value : false,
                                    Moderator = _.Moderator.HasValue ? _.Moderator.Value : false,
                                    IsRoomOwner = _.IsRoomOwner.HasValue ? _.IsRoomOwner.Value : false
                                }).
                                ToList();

                    _result = _chatRooms;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetPublicChatRooms", ex);
            }

            return _result;
        }

        public static List<RoomModalModel> GetUserChatRooms(long UserId, long CurrentUserId, int paging, IMapper mapper, DevContext dbContext)
        {
            var _result = new List<RoomModalModel>();

            try
            {
                var _chatRooms = DBContextService.GetUserChatRoomsList(UserId, CurrentUserId, mapper, dbContext).
                                Where(_ => _.IsActive && !_.IsBlocked.Value).
                                Select(_ => _).
                                OrderByDescending(_ => _.ActiveUsers).
                                ThenByDescending(_ => _.TotalUsers).
                                Take(paging).
                                ToList().
                                Select(_ => new RoomModalModel()
                                {
                                    ActiveUsers = _.ActiveUsers.HasValue ? _.ActiveUsers.Value : 0,
                                    ChatRoomName = _.ChatRoomName,
                                    Description = _.Description,
                                    RoomPrivacy = _.ChatRoomPrivacy,
                                    RoomType = _.ChatRoomType,
                                    Id = _.Id,
                                    NeedPassword = _.NeedPassword.HasValue ? _.NeedPassword.Value : false,
                                    PasswordProtected = _.PasswordProtected.HasValue ? _.PasswordProtected.Value : false,
                                    RoomOwnerId = _.RoomOwnerId,
                                    TotalUsers = _.TotalUsers.HasValue ? _.TotalUsers.Value : 0,
                                    AlreadyMember = _.AlreadyMember.HasValue ? _.AlreadyMember.Value : false,
                                    IsBlocked = _.IsBlocked.HasValue ? _.IsBlocked.Value : false,
                                    Moderator = _.Moderator.HasValue ? _.Moderator.Value : false,
                                    IsRoomOwner = _.IsRoomOwner.HasValue ? _.IsRoomOwner.Value : false
                                }).
                                ToList();

                    _result = _chatRooms;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetUserChatRooms", ex);
            }

            return _result;
        }

        public static bool IsActiveChatRoomByChatRoomId(long ChatRoomId, IMapper mapper, DevContext dbContext)
        {
            return true;
        }

        public static bool IsActiveChatRoomByChatRoomName(string ChatRoomName, IMapper mapper, DevContext dbContext)
        {
            return true;
        }

        public static bool IsActivePrivateChatRoomUserByChatRoomId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _details = GetPrivateChatRoomDetailsByIdUserId(ChatRoomId, UserId, mapper, dbContext);

                if (_details != null)
                {
                    return !_details.IsBlocked && _details.AlreadyMember && !_details.NeedPassword;
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.IsActivePrivateChatRoomUserByChatRoomId", ex);
            }

            return false;
        }

        public static bool IsActiveChatRoomUserByChatRoomId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _details = GetChatRoomDetailsByIdUserId(ChatRoomId, UserId, mapper, dbContext);

                if (_details != null)
                {
                    if (_details.RoomOwnerId == UserId)
                    {
                        return true;
                    }

                    return !_details.IsBlocked && _details.AlreadyMember && !_details.NeedPassword;
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.IsActiveChatRoomUserByChatRoomId", ex);
            }

            return false;
        }

        public static bool IsActiveChatRoomUserByChatRoomName(string ChatRoomName, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                if (!IsBlockedChatRoomUserByChatRoomName(ChatRoomName, UserId, mapper, dbContext))
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }

            return true;
        }

        public static bool IsBlockedChatRoomUserByChatRoomId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                return dbContext.ChatRoomBlockedUsers.Where(_ => _.ChatRoomId == ChatRoomId && _.BlockedUserId == UserId).Any();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.IsBlockedChatRoomUserByChatRoomId", ex);
            }

            return false;
        }

        public static bool IsBlockedChatRoomUserByChatRoomName(string ChatRoomName, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                return dbContext.ChatRoomBlockedUsers.
                            Where(_ => _.ChatRoom.ChatRoomName.ToLower() == ChatRoomName.ToLower()
                                && _.BlockedUserId == UserId).
                            Any();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.IsBlockedChatRoomUserByChatRoomName", ex);
            }

            return false;
        }

        public static bool CanUserJoinChatRoomByChatRoomId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            if (IsPublicChatRoomByChatRoomId(ChatRoomId, mapper, dbContext) && !IsBlockedChatRoomUserByChatRoomId(ChatRoomId, UserId, mapper, dbContext))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsPublicChatRoomByChatRoomId(long ChatRoomId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _chatRoom = dbContext.ChatRoomMaster.Where(_ => _.Id == ChatRoomId).FirstOrDefault();

                if (_chatRoom != null)
                {
                    if (MatesUpConstants.PublicChatRoomPrivacy.Contains(_chatRoom.ChatRoomPrivacy)
                        && MatesUpConstants.PublicChatRoomTypes.Contains(_chatRoom.ChatRoomType)
                        && _chatRoom.PasswordHash == null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.IsPublicChatRoomByChatRoomId", ex);
            }

            return false;
        }

        public static bool JoinChatRoomId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {

            try
            {
                dbContext.ChatRoomUserMapping.Add(new ChatRoomUserMapping()
                    {
                        ChatRoomId = ChatRoomId,
                        UserId = UserId,
                        LastSeen = DateTime.UtcNow
                    });

                    dbContext.SaveChanges();

                    return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.JoinChatRoomId", ex);
            }

            return false;
        }

        public static bool IsChatRoomOwnerByChatRoomId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                return dbContext.ChatRoomMaster.Where(_ => _.Id == ChatRoomId && _.RoomOwnerId == UserId).Any();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.IsChatRoomOwnerByChatRoomId", ex);
            }

            return false;
        }

        public static ChatRoomMasterDTO GetChatRoomDetailsById(long ChatRoomId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _rooms = dbContext.ChatRoomMaster.
                                    Include(_ => _.ChatRoomType).
                                    Include(_ => _.ChatRoomPrivacy).
                                    Where(_ => _.Id == ChatRoomId).
                                    Select(_ => _).
                                    ToList();
                    if (_rooms.Any())
                    {
                        return _rooms.Select(_ => mapper.Map<ChatRoomMaster, ChatRoomMasterDTO>(_)).FirstOrDefault();
                    }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetChatRoomDetailsById", ex);
            }

            return null;
        }

        public static RoomModalModel GetPrivateChatRoomDetailsByIdUserId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _room = DBContextService.GetPrivateChatRoomsDetails(ChatRoomId, UserId, mapper, dbContext).
                                    Where(_ => _.Id == ChatRoomId).
                                    Select(_ => _).
                                    FirstOrDefault();

                if (_room != null)
                {
                    return new RoomModalModel()
                    {
                        ActiveUsers = _room.ActiveUsers.HasValue ? _room.ActiveUsers.Value : 0,
                        AlreadyMember = _room.AlreadyMember.HasValue ? _room.AlreadyMember.Value : false,
                        ChatRoomName = _room.ChatRoomName,
                        Description = _room.Description,
                        RoomPrivacy = _room.ChatRoomPrivacy,
                        RoomType = _room.ChatRoomType,
                        Id = _room.Id,
                        IsBlocked = _room.IsBlocked.HasValue ? _room.IsBlocked.Value : false,
                        Moderator = _room.Moderator.HasValue ? _room.Moderator.Value : false,
                        NeedPassword = _room.NeedPassword.HasValue ? _room.NeedPassword.Value : false,
                        PasswordProtected = _room.PasswordProtected.HasValue ? _room.PasswordProtected.Value : false,
                        IsRoomOwner = _room.IsRoomOwner.HasValue ? _room.IsRoomOwner.Value : false,
                        RoomOwnerId = _room.RoomOwnerId,
                        TotalUsers = _room.TotalUsers.HasValue ? _room.TotalUsers.Value : 0
                    };
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetChatRoomDetailsByIdUserId", ex);
            }

            return null;
        }

        public static RoomModalModel GetChatRoomDetailsByIdUserId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _room = DBContextService.GetChatRoomsDetails(ChatRoomId, UserId, mapper, dbContext).
                                    Where(_ => _.Id == ChatRoomId).
                                    Select(_ => _).
                                    FirstOrDefault();

                    if (_room != null)
                    {
                        return new RoomModalModel()
                        {
                            ActiveUsers = _room.ActiveUsers.HasValue ? _room.ActiveUsers.Value : 0,
                            AlreadyMember = _room.AlreadyMember.HasValue ? _room.AlreadyMember.Value : false,
                            ChatRoomName = _room.ChatRoomName,
                            Description = _room.Description,
                            RoomPrivacy = _room.ChatRoomPrivacy,
                            RoomType = _room.ChatRoomType,
                            Id = _room.Id,
                            IsBlocked = _room.IsBlocked.HasValue ? _room.IsBlocked.Value : false,
                            Moderator = _room.Moderator.HasValue ? _room.Moderator.Value : false,
                            NeedPassword = _room.NeedPassword.HasValue ? _room.NeedPassword.Value : false,
                            PasswordProtected = _room.PasswordProtected.HasValue ? _room.PasswordProtected.Value : false,
                            IsRoomOwner = _room.IsRoomOwner.HasValue ? _room.IsRoomOwner.Value : false,
                            RoomOwnerId = _room.RoomOwnerId,
                            TotalUsers = _room.TotalUsers.HasValue ? _room.TotalUsers.Value : 0
                        };
                    }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetChatRoomDetailsByIdUserId", ex);
            }

            return null;
        }

        public static ChatRoomMasterDTO GetChatRoomDetailsByName(string ChatRoomName, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _rooms = dbContext.ChatRoomMaster.
                                    Include(_ => _.ChatRoomType).
                                    Include(_ => _.ChatRoomPrivacy).
                                    Where(_ => _.ChatRoomName.ToLower() == ChatRoomName.ToLower()).
                                    Select(_ => _).
                                    ToList();

                    if (_rooms.Any())
                    {
                        return _rooms.Select(_ => mapper.Map<ChatRoomMaster, ChatRoomMasterDTO>(_)).FirstOrDefault();
                    }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetChatRoomDetailsByName", ex);
            }

            return null;
        }

        public static RoomModalModel GetChatRoomDetailsByNameUserId(string ChatRoomName, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _room = DBContextService.GetChatRoomsDetailsByName(ChatRoomName, UserId, mapper, dbContext).
                                    Select(_ => _).
                                    FirstOrDefault();

                    if (_room != null)
                    {
                        return new RoomModalModel()
                        {
                            ActiveUsers = _room.ActiveUsers.HasValue ? _room.ActiveUsers.Value : 0,
                            AlreadyMember = _room.AlreadyMember.HasValue ? _room.AlreadyMember.Value : false,
                            ChatRoomName = _room.ChatRoomName,
                            Description = _room.Description,
                            RoomPrivacy = _room.ChatRoomPrivacy,
                            RoomType = _room.ChatRoomType,
                            Id = _room.Id,
                            IsBlocked = _room.IsBlocked.HasValue ? _room.IsBlocked.Value : false,
                            Moderator = _room.Moderator.HasValue ? _room.Moderator.Value : false,
                            NeedPassword = _room.NeedPassword.HasValue ? _room.NeedPassword.Value : false,
                            PasswordProtected = _room.PasswordProtected.HasValue ? _room.PasswordProtected.Value : false,
                            IsRoomOwner = _room.IsRoomOwner.HasValue ? _room.IsRoomOwner.Value : false,
                            RoomOwnerId = _room.RoomOwnerId,
                            TotalUsers = _room.TotalUsers.HasValue ? _room.TotalUsers.Value : 0
                        };
                    }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetChatRoomDetailsByNameUserId", ex);
            }

            return null;
        }

        public static List<UserModalModel> GetActiveUsersList(IMapper mapper, DevContext dbContext)
        {
            var _list = new List<UserModalModel>();
            try
            {
                var _users = dbContext.ActiveUsers.
                                    Include(_ => _.User).
                                    Include(_ => _.User.Gender).
                                    Include(_ => _.User.Country).
                                    Include(_ => _.User.State).
                                    Include(_ => _.User.City).
                                    Include(_ => _.User.RelationshipStatus).
                                    OrderByDescending(_ => _.LastActive).
                                    Select(_ => _).
                                    Take(20).
                                    ToList();

                    _list = _users.Select(_ => mapper.Map<UserProfileMaster, UserModalModel>(_.User)).ToList();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetActiveUsersList", ex);
            }

            return _list;
        }

        public static List<UserModalModel> GetActiveUsersList(UserModalModel UserProfile, int Gender, IMapper mapper, DevContext dbContext)
        {
            var _list = new List<UserModalModel>();
            try
            {
                var _users = DBContextService.GetActiveUsersList(
                                        UserProfile.CityNavigation != null ? UserProfile.CityNavigation.ID : 19210,
                                        UserProfile.StateNavigation != null ? UserProfile.StateNavigation.ID : 1659,
                                        UserProfile.CountryNavigation != null ? UserProfile.CountryNavigation.ID : 101,
                                        UserProfile.UserId,
                                        mapper,
                                        dbContext).
                                    Join(dbContext.UserProfileMaster.
                                    Include(_ => _.GenderNavigation).
                                    Include(_ => _.CountryNavigation).
                                    Include(_ => _.StateNavigation).
                                    Include(_ => _.CityNavigation).
                                    Include(_ => _.RelationshipStatusNavigation), x => x.UserId, y => y.UserId, (x, y) => new { x, y }).
                                    Where(_ => Gender != 0 ? (_.y.Gender == Gender) : true).
                                    OrderBy(_ => _.x.CountryRank).
                                    ThenBy(_ => _.x.StateRank).
                                    ThenBy(_ => _.x.CityRank).
                                    ThenBy(_ => _.x.LastSeenRank).
                                    Select(_ => _).
                                    Distinct().
                                    Take(20).
                                    ToList().
                                    Select(_ => new UserModalModel()
                                    {
                                        UserId = _.x.UserId,
                                        FirstName = _.x.FirstName,
                                        LastName = _.x.LastName ?? "",
                                        NickName = _.x.NickName,
                                        Description = _.x.Description ?? "",
                                        Gender = _.y.Gender,
                                        CityNavigation = _.y.CityNavigation != null ? mapper.Map<LookupCity, LookupCityDTO>(_.y.CityNavigation) : null,
                                        CountryNavigation = _.y.CountryNavigation != null ? mapper.Map<LookupCountry, LookupCountryDTO>(_.y.CountryNavigation) : null,
                                        StateNavigation = _.y.StateNavigation != null ? mapper.Map<LookupState, LookupStateDTO>(_.y.StateNavigation) : null,
                                        GenderNavigation = mapper.Map<LookupGender, LookupGenderDTO>(_.y.GenderNavigation),
                                        VerifiedUser = _.x.EmailConfirmed.HasValue ? _.x.EmailConfirmed.Value : false,
                                        Online = _.x.LastActive >= DateTime.UtcNow.AddMinutes(-15) ? true : false,
                                        Followers = _.x.Followers.HasValue ? _.x.Followers.Value : 0,
                                        Following = _.x.Following.HasValue ? _.x.Following.Value : 0,
                                        IsBlocked = _.x.IsBlocked.HasValue ? _.x.IsBlocked.Value : false,
                                        YouBlocked = _.x.YouBlocked.HasValue ? _.x.YouBlocked.Value : false,
                                        IsFollowing = _.x.IsFollowing.HasValue ? _.x.IsFollowing.Value : false
                                    }).
                                    ToList();

                    _list = _users;
                    //.Select(_ => Mapper.Map<UserProfileMaster, UserModalModel>(_)).ToList();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetActiveUsersList", ex);
            }

            return _list;
        }

        public static UserModalModel GetUserDetailsByIdUserId(long UserId, long CurrentUserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _user = DBContextService.GetUserDetails(UserId, CurrentUserId, mapper, dbContext).
                                    Join(dbContext.UserProfileMaster.Include(_ => _.GenderNavigation).
                                    Include(_ => _.CountryNavigation).
                                    Include(_ => _.StateNavigation).
                                    Include(_ => _.CityNavigation).
                                    Include(_ => _.RelationshipStatusNavigation), x => x.UserId, y => y.UserId, (x, y) => new { x, y }).
                                    Select(_ => _).
                                    FirstOrDefault();

                    if (_user != null)
                    {
                        return new UserModalModel()
                        {
                            UserId = _user.y.UserId,
                            FirstName = _user.y.FirstName,
                            LastName = _user.y.LastName,
                            NickName = _user.y.NickName,
                            Description = _user.y.Description,
                            Gender = _user.y.Gender,
                            CityNavigation = mapper.Map<LookupCity, LookupCityDTO>(_user.y.CityNavigation),
                            CountryNavigation = mapper.Map<LookupCountry, LookupCountryDTO>(_user.y.CountryNavigation),
                            StateNavigation = mapper.Map<LookupState, LookupStateDTO>(_user.y.StateNavigation),
                            GenderNavigation = mapper.Map<LookupGender, LookupGenderDTO>(_user.y.GenderNavigation),
                            VerifiedUser = _user.x.EmailConfirmed.HasValue ? _user.x.EmailConfirmed.Value : false,
                            Online = _user.x.LastActive >= DateTime.UtcNow.AddMinutes(-15) ? true : false,
                            Followers = _user.x.Followers.HasValue ? _user.x.Followers.Value : 0,
                            Following = _user.x.Following.HasValue ? _user.x.Following.Value : 0,
                            IsBlocked = _user.x.IsBlocked.HasValue ? _user.x.IsBlocked.Value : false,
                            YouBlocked = _user.x.YouBlocked.HasValue ? _user.x.YouBlocked.Value : false,
                            IsFollowing = _user.x.IsFollowing.HasValue ? _user.x.IsFollowing.Value : false
                        };
                    }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetUserDetailsByIdUserId", ex);
            }

            return null;
        }

        public static UserModalModel GetMyDetailsById(long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _user = DBContextService.GetUserDetails(UserId, UserId, mapper, dbContext).
                                    Join(dbContext.UserProfileMaster.Include(_ => _.GenderNavigation).
                                    Include(_ => _.CountryNavigation).
                                    Include(_ => _.StateNavigation).
                                    Include(_ => _.CityNavigation).
                                    Include(_ => _.RelationshipStatusNavigation), x => x.UserId, y => y.UserId, (x, y) => new { x, y }).
                                    Select(_ => _).
                                    FirstOrDefault();

                    if (_user != null)
                    {
                        return new UserModalModel()
                        {
                            UserId = _user.y.UserId,
                            FirstName = _user.y.FirstName,
                            LastName = _user.y.LastName,
                            NickName = _user.y.NickName,
                            Description = _user.y.Description,
                            Gender = _user.y.Gender,
                            DOB = _user.y.DOB.HasValue ? _user.y.DOB.Value : DateTimeUtility.DateOnlyFromDateTime(DateTime.MinValue.Date),
                            CityNavigation = mapper.Map<LookupCity, LookupCityDTO>(_user.y.CityNavigation),
                            CountryNavigation = mapper.Map<LookupCountry, LookupCountryDTO>(_user.y.CountryNavigation),
                            StateNavigation = mapper.Map<LookupState, LookupStateDTO>(_user.y.StateNavigation),
                            GenderNavigation = mapper.Map<LookupGender, LookupGenderDTO>(_user.y.GenderNavigation),
                            VerifiedUser = _user.x.EmailConfirmed.HasValue ? _user.x.EmailConfirmed.Value : false,
                            Online = _user.x.LastActive >= DateTime.UtcNow.AddMinutes(-15) ? true : false,
                            Followers = _user.x.Followers.HasValue ? _user.x.Followers.Value : 0,
                            Following = _user.x.Following.HasValue ? _user.x.Following.Value : 0,
                            IsBlocked = _user.x.IsBlocked.HasValue ? _user.x.IsBlocked.Value : false,
                            YouBlocked = _user.x.YouBlocked.HasValue ? _user.x.YouBlocked.Value : false,
                            IsFollowing = _user.x.IsFollowing.HasValue ? _user.x.IsFollowing.Value : false
                        };
                    }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetMyDetailsById", ex);
            }

            return null;
        }

        public static bool IsValidChatRoomType(int ChatRoomType, IMapper mapper, DevContext dbContext)
        {
            if (ChatRoomType != (int)MatesUpConstants.ChatRoomType.MatesUpArena
                && ChatRoomType != (int)MatesUpConstants.ChatRoomType.Private)
                return true;
            return false;
        }

        public static bool ClearChatRoomMessagesByChatRoomId(long ChatRoomId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _messages = dbContext.ChatMaster.Where(_ => _.ChatRoomId == ChatRoomId).Select(_ => _);

                    dbContext.ChatMaster.RemoveRange(_messages);
                    dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.ClearChatRoomMessagesByChatRoomId", ex);
            }

            return false;
        }

        public static bool ClearChatRoomAnnoucementsByChatRoomId(long ChatRoomId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _messages = dbContext.MegaPhoneMaster.Where(_ => _.ChatRoomId == ChatRoomId).Select(_ => _);

                    dbContext.MegaPhoneMaster.RemoveRange(_messages);
                    dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.ClearChatRoomAnnoucementsByChatRoomId", ex);
            }

            return false;
        }

        public static ChatRoomAccessModel ChatRoomUserAccessByChatRoomId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            var _result = new ChatRoomAccessModel();

            _result.ChatRoomId = ChatRoomId;
            _result.IsActiveUser = IsActiveChatRoomUserByChatRoomId(ChatRoomId, UserId, mapper, dbContext);
            _result.IsRoomOwner = IsChatRoomOwnerByChatRoomId(ChatRoomId, UserId, mapper, dbContext);

            return _result;
        }

        public static void UpdateChatRoomLastseenByChatRoomId(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _lastSeen = dbContext.ChatRoomUserMapping.Where(_ => _.ChatRoomId == ChatRoomId && _.UserId == UserId).Select(_ => _).FirstOrDefault();

                    if (_lastSeen != null)
                    {
                        _lastSeen.LastSeen = DateTime.UtcNow;
                    }
                    else
                    {
                        _lastSeen = new ChatRoomUserMapping()
                        {
                            ChatRoomId = ChatRoomId,
                            LastSeen = DateTime.UtcNow,
                            UserId = UserId
                        };
                        dbContext.ChatRoomUserMapping.Add(_lastSeen);
                    }

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.ClearChatRoomMessagesByChatRoomId", ex);
            }
        }

        public static List<PrivateChatRoomModel> GetMessageNotifications(long UserId, int Paging, IMapper mapper, DevContext dbContext)
        {
            var _result = new List<PrivateChatRoomModel>();

            try
            {
                DateTime lastNotificationTime = DateTime.UtcNow.AddDays(-1);
                    var _userActivity = dbContext.ActiveUsers.Where(_ => _.UserId == UserId).FirstOrDefault();

                    if (_userActivity != null)
                    {
                        lastNotificationTime = _userActivity.LastNotification != null ? _userActivity.LastNotification :
                                    (_userActivity.LastActive != null ? _userActivity.LastActive :
                                    (_userActivity.LastLogin != null ? _userActivity.LastLogin : DateTime.UtcNow.AddDays(-1)));
                    }

                    var _chatRooms = DBContextService.GetMyPrivateChatRoomsList(UserId, mapper, dbContext).
                                Where(_ => _.NewMessagesCount > 0
                                    && _.LastMessageOn > lastNotificationTime).
                                OrderByDescending(_ => _.LastMessageOn).
                                Select(_ => _).
                                Take(Paging).
                                ToList().
                                Select(_ => new PrivateChatRoomModel()
                                {
                                    ChatRoomId = _.Id,
                                    LastMessageOn = _.LastMessageOn,
                                    NewMessageCount = _.NewMessagesCount.HasValue ? _.NewMessagesCount.Value : 0,
                                    RoomName = _.RoomDisplayName
                                }).
                                ToList();

                    _result = _chatRooms.ToList();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetMessageNotifications", ex);
            }

            return _result;
        }

        public static bool BlockChatRoomUser(long ChatRoomId, long UserId, long BlockedBy, IMapper mapper, DevContext dbContext)
        {
            try
            {
                if (!dbContext.ChatRoomBlockedUsers.Where(_ => _.ChatRoomId == ChatRoomId && _.BlockedUserId == UserId).Select(_ => _).Any())
                    {
                        dbContext.ChatRoomBlockedUsers.Add(new ChatRoomBlockedUsers()
                        {
                            BlockedBy = BlockedBy,
                            BlockedUserId = UserId,
                            BlockedOn = DateTime.UtcNow,
                            ChatRoomId = ChatRoomId
                        });

                        dbContext.SaveChanges();
                    }

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.BlockChatRoomUser", ex);
            }

            return false;
        }

        public static bool UnBlockChatRoomUser(long ChatRoomId, long UserId, long BlockedBy, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _blocked = dbContext.ChatRoomBlockedUsers.Where(_ => _.ChatRoomId == ChatRoomId && _.BlockedUserId == UserId).Select(_ => _).FirstOrDefault();

                    if (_blocked != null)
                    {
                        dbContext.ChatRoomBlockedUsers.Remove(_blocked);
                        dbContext.SaveChanges();
                    }

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.UnBlockChatRoomUser", ex);
            }

            return false;
        }

        public static bool AddChatRoomUser(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _blocked = dbContext.ChatRoomUserMapping.Where(_ => _.ChatRoomId == ChatRoomId && _.UserId == UserId).Select(_ => _).FirstOrDefault();

                    if (_blocked == null)
                    {
                        dbContext.ChatRoomUserMapping.Add(new ChatRoomUserMapping()
                        {
                            ChatRoomId = ChatRoomId,
                            LastSeen = DateTime.UtcNow,
                            UserId = UserId
                        });
                        dbContext.SaveChanges();

                    return true;
                    }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.AddChatRoomUser", ex);
            }

            return false;
        }

        public static List<UserModalModel> GetUsersList(UserModalModel UserProfile, UserSearchModel search, int Paging, IMapper mapper, DevContext dbContext)
        {
            var _list = new List<UserModalModel>();
            try
            {
                var _onlineTime = DateTime.UtcNow.AddMinutes(-15);

                    var _users = DBContextService.GetActiveUsersList(
                                        UserProfile.CityNavigation != null ? UserProfile.CityNavigation.ID : 19210,
                                        UserProfile.StateNavigation != null ? UserProfile.StateNavigation.ID : 1659,
                                        UserProfile.CountryNavigation != null ? UserProfile.CountryNavigation.ID : 101,
                                        UserProfile.UserId,
                                        mapper,
                                        dbContext).
                                    Where(_ => (search.Gender != 0 ? _.Gender == search.Gender : true)
                                        && (search.OnlyVerifiedUsers ? (_.EmailConfirmed.HasValue ? _.EmailConfirmed.Value : false) : true)
                                        && (search.OnlyOnline ? _.LastActive >= _onlineTime : true)
                                        && (search.SearchTerm != null && search.SearchTerm != "" ? _.NickName.Contains(search.SearchTerm) : true)).
                                    Join(dbContext.UserProfileMaster.Include(_ => _.GenderNavigation).
                                    Include(_ => _.CountryNavigation).
                                    Include(_ => _.StateNavigation).
                                    Include(_ => _.CityNavigation).
                                    Include(_ => _.RelationshipStatusNavigation), x => x.UserId, y => y.UserId, (x, y) => new { x, y }).
                                    OrderBy(_ => _.x.CountryRank).
                                    ThenBy(_ => _.x.StateRank).
                                    ThenBy(_ => _.x.CityRank).
                                    ThenBy(_ => _.x.LastSeenRank).
                                    Select(_ => _).
                                    Take(Paging).
                                    ToList().
                                    Select(_ => new UserModalModel()
                                    {
                                        UserId = _.x.UserId,
                                        FirstName = _.x.FirstName,
                                        LastName = _.x.LastName,
                                        NickName = _.x.NickName,
                                        Description = _.x.Description,
                                        Gender = _.y.Gender,
                                        CityNavigation = mapper.Map<LookupCity, LookupCityDTO>(_.y.CityNavigation),
                                        CountryNavigation = mapper.Map<LookupCountry, LookupCountryDTO>(_.y.CountryNavigation),
                                        StateNavigation = mapper.Map<LookupState, LookupStateDTO>(_.y.StateNavigation),
                                        GenderNavigation = mapper.Map<LookupGender, LookupGenderDTO>(_.y.GenderNavigation),
                                        VerifiedUser = _.x.EmailConfirmed.HasValue ? _.x.EmailConfirmed.Value : false,
                                        Online = _.x.LastActive >= DateTime.UtcNow.AddMinutes(-15) ? true : false,
                                        Followers = _.x.Followers.HasValue ? _.x.Followers.Value : 0,
                                        Following = _.x.Following.HasValue ? _.x.Following.Value : 0,
                                        IsBlocked = _.x.IsBlocked.HasValue ? _.x.IsBlocked.Value : false,
                                        YouBlocked = _.x.YouBlocked.HasValue ? _.x.YouBlocked.Value : false,
                                        IsFollowing = _.x.IsFollowing.HasValue ? _.x.IsFollowing.Value : false
                                    }).
                                    ToList();

                    _list = _users;
                    //.Select(_ => Mapper.Map<UserProfileMaster, UserModalModel>(_)).ToList();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetActiveUsersList", ex);
            }

            return _list;
        }

        public static List<RoomModalModel> GetRoomsList(RoomSearchModel search, int paging, long UserId, IMapper mapper, DevContext dbContext)
        {
            var _result = new List<RoomModalModel>();

            try
            {
                var _chatRooms = DBContextService.GetSearchChatRoomsList(UserId, mapper, dbContext).
                                Where(_ => _.IsActive && !_.IsBlocked.Value
                                    && (search.SearchTerm != null && search.SearchTerm != "" ? _.ChatRoomName.Contains(search.SearchTerm) : true)
                                    && (search.SkipAlreadyJoined ? (_.AlreadyMember.HasValue ? !_.AlreadyMember.Value : true) : true)
                                    && (search.SkipAlreadyOwned ? (_.IsRoomOwner.HasValue ? !_.IsRoomOwner.Value : true) : true)).
                                Select(_ => _).
                                OrderByDescending(_ => _.ActiveUsers).
                                ThenByDescending(_ => _.TotalUsers).
                                Take(paging).
                                ToList().
                                Select(_ => new RoomModalModel()
                                {
                                    ActiveUsers = _.ActiveUsers.HasValue ? _.ActiveUsers.Value : 0,
                                    ChatRoomName = _.ChatRoomName,
                                    Description = _.Description,
                                    RoomPrivacy = _.ChatRoomPrivacy,
                                    RoomType = _.ChatRoomType,
                                    Id = _.Id,
                                    NeedPassword = _.NeedPassword.HasValue ? _.NeedPassword.Value : false,
                                    PasswordProtected = _.PasswordProtected.HasValue ? _.PasswordProtected.Value : false,
                                    RoomOwnerId = _.RoomOwnerId,
                                    TotalUsers = _.TotalUsers.HasValue ? _.TotalUsers.Value : 0,
                                    AlreadyMember = _.AlreadyMember.HasValue ? _.AlreadyMember.Value : false,
                                    IsBlocked = _.IsBlocked.HasValue ? _.IsBlocked.Value : false,
                                    Moderator = _.Moderator.HasValue ? _.Moderator.Value : false,
                                    IsRoomOwner = _.IsRoomOwner.HasValue ? _.IsRoomOwner.Value : false
                                }).
                                ToList();

                    _result = _chatRooms;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatRoomService.GetRoomsList", ex);
            }

            return _result;
        }
    }
}