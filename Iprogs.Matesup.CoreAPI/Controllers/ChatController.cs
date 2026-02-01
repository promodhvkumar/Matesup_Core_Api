using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using AutoMapper;

using Iprogs.Matesup.CoreAPI.Models;
using Iprogs.Matesup.Models;
using Iprogs.Matesup.CoreAPI.Services;
using System.Buffers.Text;
using Microsoft.AspNetCore.Authorization;

namespace Iprogs.Matesup.CoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IMapper _mapper;
        private readonly DevContext _dbContext;
        private readonly SignInManager<IdentityUser<long>> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ContextService contextService;

        public ChatController(ILogger<ChatController> logger, IMapper mapper, DevContext dbContext, SignInManager<IdentityUser<long>> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;

            contextService = new ContextService(_httpContextAccessor, _signInManager, _mapper, _dbContext);
        }
        
        /// <summary>
        /// Gets the chat attachment, mostly images embedded in the chat
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        //[OutputCache(Duration = 1440, Location = System.Web.UI.OutputCacheLocation.ServerAndClient, NoStore = false, VaryByParam = "*")]
        public IActionResult Attachment(string Id)
        {
            try
            {
                var _attachment = ChatService.GetAttachment(Id, contextService.UserId, _mapper, _dbContext);

                if (_attachment != null)
                {
                    return File(_attachment.Attachment, "image/jpeg", _attachment.AttachmentName + _attachment.FileExtn);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.Attachment");
            }

            return BadRequest();
        }

        /// <summary>
        /// Posts a new chat message to the provided chat arena - private or group chat or public arenas
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseModel NewMessage(ChatMessageModel message)
        {
            try
            {
                UserService.UpdateLastActiveByUserId(contextService.UserId, _mapper, _dbContext);
                if (message != null && message.Message != null)
                {
                    if (!ChatRoomService.IsActiveChatRoomUserByChatRoomId(message.ChatRoomId, contextService.UserId, _mapper, _dbContext))
                    {
                        return new ResponseModel { status = "Failure", responseText = "You're not an active Member of the ChatRoom!" };
                    }

                    if (message.Message.Length > 204800)
                    {
                        return new ResponseModel { status = "Failure", responseText = "Content size Exceeded the Limit!" };
                    }

                    var _message = new ChatMasterDTO();
                    _message.ChatRoomId = message.ChatRoomId;
                    _message.Message = message.Message;
                    _message.MessageType = 1;
                    _message.SentOn = DateTime.UtcNow;
                    _message.UserId = contextService.UserId;

                    var _result = ChatService.SaveChatMessage(_message, _mapper, _dbContext);

                    if (_result)
                    {
                        return new ResponseModel { status = "Success", responseText = "Message sent successfully!" };
                    }
                }
                else
                {
                    return new ResponseModel { status = "Failure", responseText = "Invalid Message!" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.NewMessage");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpPost]
        public ResponseModel NewPrivateMessage(ChatMessageModel message)
        {
            try
            {
                UserService.UpdateLastActiveByUserId(contextService.UserId, _mapper, _dbContext);
                if (message != null && message.Message != null)
                {
                    if (!ChatRoomService.IsActivePrivateChatRoomUserByChatRoomId(message.ChatRoomId, contextService.UserId, _mapper, _dbContext))
                    {
                        return new ResponseModel { status = "Failure", responseText = "You're not an active Member of the ChatRoom!" };
                    }

                    if (message.Message.Length > 204800)
                    {
                        return new ResponseModel { status = "Failure", responseText = "Content size Exceeded the Limit!" };
                    }

                    var _message = new ChatMasterDTO();
                    _message.ChatRoomId = message.ChatRoomId;
                    _message.Message = message.Message;
                    _message.MessageType = 1;
                    _message.SentOn = DateTime.UtcNow;
                    _message.UserId = contextService.UserId;

                    var _result = ChatService.SaveChatMessage(_message, _mapper, _dbContext);

                    if (_result)
                    {
                        return new ResponseModel { status = "Success", responseText = "Message sent successfully!" };
                    }
                }
                else
                {
                    return new ResponseModel { status = "Failure", responseText = "Invalid Message!" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.NewPrivateMessage");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetMyConversations(int Paging, string SearchTerm)
        {
            try
            {
                var _userId = contextService.UserId;

                var _chatRooms = ChatRoomService.GetMyPrivateChatRooms(_userId, Paging, string.IsNullOrWhiteSpace(SearchTerm) ? string.Empty : SearchTerm, _mapper, _dbContext);

                if (_chatRooms != null && _chatRooms.Any())
                {
                    return new ResponseModel { status = "Success", Data = _chatRooms };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetMyConversations");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetConversationwithUser(long UserId)
        {
            try
            {
                var _senderId = contextService.UserId;

                if (_senderId == UserId)
                {
                    return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
                }

                var _chatRoom = ChatService.GetPrivateChatRoomBySenderReceiverId(_senderId, UserId, _mapper, _dbContext);

                if (_chatRoom == null)
                {
                    _chatRoom = new ChatRoomMasterDTO();
                    _chatRoom.ChatRoomName = DateTime.UtcNow.ToString("ddMMyyyyhhmmssfff") + "_" + _senderId.ToString() + "_" + UserId.ToString();
                    _chatRoom.RoomOwnerId = _senderId;
                    _chatRoom.CreatedBy = _senderId;
                    _chatRoom.CreatedOn = DateTime.UtcNow;
                    _chatRoom.ChatRoomPrivacy = (int)MatesUpConstants.ChatRoomPrivacy.Private;
                    _chatRoom.ChatRoomType = (int)MatesUpConstants.ChatRoomType.Private;
                    _chatRoom.IsActive = true;
                    _chatRoom.LastMessageOn = DateTime.UtcNow;

                    _chatRoom.ChatRoomUserMapping = new List<ChatRoomUserMappingDTO>();
                    _chatRoom.ChatRoomUserMapping.Add(new ChatRoomUserMappingDTO()
                    {
                        UserId = _senderId,
                        LastSeen = DateTime.UtcNow
                    });
                    _chatRoom.ChatRoomUserMapping.Add(new ChatRoomUserMappingDTO()
                    {
                        UserId = UserId,
                        LastSeen = null
                    });

                    _chatRoom = ChatRoomService.CreatePrivateChatRoom(_chatRoom, _mapper, _dbContext);

                    return new ResponseModel { status = "Success", Data = _chatRoom.Id };
                }
                else
                {
                    return new ResponseModel { status = "Success", Data = _chatRoom.Id };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetConversationwithUser");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetPrivateChatRoomMessages(long ChatRoomId, int Paging)
        {
            try
            {
                if (ChatRoomService.IsActivePrivateChatRoomUserByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    ChatRoomService.UpdateChatRoomLastseenByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext);
                    var _messages = ChatService.GetChatMessagesByChatRoomId(ChatRoomId, Paging, _mapper, _dbContext);

                    return new ResponseModel { status = "Success", Data = _messages };
                }
                else
                {
                    return new ResponseModel { status = "Failure", responseText = "You're not allowed to this Conversation!" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetPrivateChatRoomMessages");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetChatRoomMessages(long ChatRoomId, int Paging)
        {
            try
            {
                if (ChatRoomService.IsActiveChatRoomUserByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    ChatRoomService.UpdateChatRoomLastseenByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext);
                    var _messages = ChatService.GetChatMessagesByChatRoomId(ChatRoomId, Paging, _mapper, _dbContext);

                    return new ResponseModel { status = "Success", Data = _messages };
                }
                else if (ChatRoomService.CanUserJoinChatRoomByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    if (ChatRoomService.JoinChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                    {
                        ChatRoomService.UpdateChatRoomLastseenByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext);
                    }
                    return new ResponseModel { status = "Success", Data = new List<ChatMessagesModel>() };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetChatRoomMessages");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetNewChatRoomMessages(long ChatRoomId, long Last)
        {
            try
            {
                if (ChatRoomService.IsActiveChatRoomUserByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    var _messages = ChatService.GetNewChatMessagesByChatRoomId(ChatRoomId, Last, _mapper, _dbContext);

                    return new ResponseModel { status = "Success", Data = _messages };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetNewChatRoomMessages");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetNewPrivateChatRoomMessages(long ChatRoomId, long Last)
        {
            try
            {
                if (ChatRoomService.IsActivePrivateChatRoomUserByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    var _messages = ChatService.GetNewChatMessagesByChatRoomId(ChatRoomId, Last, _mapper, _dbContext);

                    return new ResponseModel { status = "Success", Data = _messages };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetNewPrivateChatRoomMessages");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpPost]
        public ResponseModel NewChatRoom(NewChatRoomModel NewChatRoom)
        {
            try
            {
                if (!ChatRoomService.IsValidChatRoomType(NewChatRoom.ChatRoomType, _mapper, _dbContext))
                {
                    return new ResponseModel { status = "Failure", responseText = "Invalid Room Type!" };
                }

                var regexExp = "^[0-9a-zA-Z]+$";

                if (string.IsNullOrWhiteSpace(NewChatRoom.ChatRoomName)
                    || !Regex.Match(NewChatRoom.ChatRoomName, regexExp).Success)
                {
                    return new ResponseModel { status = "Failure", responseText = "Invalid Room Name!" };
                }

                var _ownerId = contextService.UserId;

                var _newChatRoom = new ChatRoomMasterDTO();

                if (NewChatRoom.ChatRoomId == 0)
                {
                    _newChatRoom = ChatRoomService.GetChatRoomByChatRoomName(NewChatRoom.ChatRoomName, _mapper, _dbContext);
                }
                else
                {
                    _newChatRoom = ChatRoomService.GetChatRoomDetailsById(NewChatRoom.ChatRoomId, _mapper, _dbContext);
                }

                if (_newChatRoom != null)
                {
                    if (ChatRoomService.IsChatRoomOwnerByChatRoomId(_newChatRoom.Id, _ownerId, _mapper, _dbContext))
                    {
                        _newChatRoom.Id = NewChatRoom.ChatRoomId;
                    }
                    else
                    {
                        return new ResponseModel { status = "Failure", responseText = "Room name already taken!" };
                    }
                }
                else
                {
                    _newChatRoom = new ChatRoomMasterDTO();
                    _newChatRoom.LastMessageOn = DateTime.UtcNow;

                    _newChatRoom.ChatRoomUserMapping = new List<ChatRoomUserMappingDTO>();
                    _newChatRoom.ChatRoomUserMapping.Add(new ChatRoomUserMappingDTO()
                    {
                        UserId = _ownerId,
                        LastSeen = DateTime.UtcNow
                    });
                }

                _newChatRoom.ChatRoomName = NewChatRoom.ChatRoomName;
                _newChatRoom.Description = NewChatRoom.Description;
                _newChatRoom.RoomOwnerId = _ownerId;
                _newChatRoom.CreatedBy = _ownerId;
                _newChatRoom.CreatedOn = DateTime.UtcNow;
                _newChatRoom.ChatRoomPrivacy = NewChatRoom.ChatRoomPrivacy;
                _newChatRoom.ChatRoomType = NewChatRoom.ChatRoomType;
                _newChatRoom.IsActive = true;
                if (NewChatRoom.ChatRoomType == (int)MatesUpConstants.ChatRoomType.Group)
                {
                    _newChatRoom.PasswordHash = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(NewChatRoom.Password));
                }
                else
                {
                    _newChatRoom.PasswordHash = null;
                }

                if (!string.IsNullOrWhiteSpace(NewChatRoom.Password))
                {
                    _newChatRoom.PasswordChangedOn = DateTime.UtcNow;
                }
                else
                {
                    _newChatRoom.PasswordChangedOn = null;
                }

                _newChatRoom = ChatRoomService.CreateChatRoom(_newChatRoom, _mapper, _dbContext);

                if (_newChatRoom.ChatRoomType == (int)MatesUpConstants.ChatRoomType.Public
                    && _newChatRoom.ChatRoomPrivacy == (int)MatesUpConstants.ChatRoomPrivacy.Public)
                {
                    UserService.UserArenaCreationAnnouncement(contextService.UserId, _newChatRoom.Id, _mapper, _dbContext);
                }

                if (NewChatRoom.ChatRoomId != 0)
                {
                    return new ResponseModel
                    {
                        status = "Success",
                        responseText = "Arena Details has been udpated successfully!"
                    };
                }
                else
                {
                    return new ResponseModel
                    {
                        status = "Success",
                        responseText = "New Arena has been created successfully!"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.NewChatRoom");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetMyOwnChatRooms()
        {
            try
            {
                var _chatRooms = ChatRoomService.GetMyOwnChatRooms(contextService.UserId, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _chatRooms };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetMyOwnChatRooms");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetMyJoinedChatRooms()
        {
            try
            {
                var _chatRooms = ChatRoomService.GetMyJoinedChatRooms(contextService.UserId, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _chatRooms };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetMyJoinedChatRooms");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetPublicChatRooms()
        {
            try
            {
                var _chatRooms = ChatRoomService.GetPublicChatRooms(contextService.UserId, 50, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _chatRooms };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetPublicChatRooms");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetTrendingChatRooms()
        {
            try
            {
                var _chatRooms = ChatRoomService.GetTrendingChatRooms(contextService.UserId, 20, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _chatRooms };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetPublicChatRooms");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetChatRoomDetailsById(long ChatRoomId)
        {
            try
            {
                var _roomDetails = ChatRoomService.GetChatRoomDetailsByIdUserId(ChatRoomId, contextService.UserId, _mapper, _dbContext);

                if (_roomDetails != null)
                {
                    return new ResponseModel { status = "Success", Data = _roomDetails };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetChatRoomDetailsById");
            }

            return new ResponseModel { status = "Failure", responseText="Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetChatRoomDetailsByName(string ChatRoomName)
        {
            try
            {
                var _roomDetails = ChatRoomService.GetChatRoomDetailsByNameUserId(ChatRoomName, contextService.UserId, _mapper, _dbContext);

                if (_roomDetails != null)
                {
                    return new ResponseModel { status = "Success", Data = _roomDetails };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetChatRoomDetailsByName");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpPost]
        public ResponseModel ValidatePasswordById(long ChatRoomId, TextModel password)
        {
            try
            {
                var _roomDetails = ChatRoomService.GetChatRoomDetailsById(ChatRoomId, _mapper, _dbContext);

                if (_roomDetails != null
                    && !string.IsNullOrWhiteSpace(_roomDetails.PasswordHash)
                    && !string.IsNullOrWhiteSpace(password.Content))
                {
                    var _hash = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(password.Content));
                    //var _result = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(_roomDetails.PasswordHash));
                        //new PasswordHasher().VerifyHashedPassword(_roomDetails.PasswordHash, password.Content);
                    if (_hash == _roomDetails.PasswordHash)
                    {
                        ChatRoomService.UpdateChatRoomLastseenByChatRoomId(_roomDetails.Id, contextService.UserId, _mapper, _dbContext);
                        return new ResponseModel
                        {
                            status = "Success",
                            Validation = true,
                        };
                    }
                    else
                    {
                        return new ResponseModel
                        {
                            status = "Success",
                            Validation = false,
                        };
                    }
                }
                else
                {
                    return new ResponseModel
                    {
                        status = "Success",
                        Validation = false,
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.ValidatePasswordById");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel GetChatRoomMembers(long ChatRoomId)
        {
            try
            {
                if (ChatRoomService.IsActiveChatRoomUserByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext)
                        || ChatRoomService.IsChatRoomOwnerByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    var _users = ChatRoomService.GetChatRoomUsersByChatRoomId(ChatRoomId, _mapper, _dbContext);

                    return new ResponseModel { status = "Success", Data = _users };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetChatRoomMembers");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetUserDetailsById(long UserId)
        {
            try
            {
                var _userDetails = ChatRoomService.GetUserDetailsByIdUserId(UserId, contextService.UserId, _mapper, _dbContext);

                if (_userDetails != null)
                {
                    return new ResponseModel { status = "Success", Data = _userDetails };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetUserDetailsById");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred" };
        }

        [HttpGet]
        public ResponseModel ClearChatRoomMessages(long ChatRoomId)
        {
            try
            {
                if (ChatRoomService.IsChatRoomOwnerByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    ChatRoomService.ClearChatRoomMessagesByChatRoomId(ChatRoomId, _mapper, _dbContext);
                    return new ResponseModel { status = "Success" };
                }
                else
                {
                    return new ResponseModel { status = "Failure", responseText = "Not Authorized!" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.ClearChatRoomMessages");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel ClearChatRoomAnnouncements(long ChatRoomId)
        {
            try
            {
                if (ChatRoomService.IsChatRoomOwnerByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    ChatRoomService.ClearChatRoomAnnoucementsByChatRoomId(ChatRoomId, _mapper, _dbContext);
                    return new ResponseModel { status = "Success" };
                }
                else
                {
                    return new ResponseModel { status = "Failure", responseText = "Not Authorized!" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.ClearChatRoomAnnouncements");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel ChatRoomUserAccess(long ChatRoomId)
        {
            try
            {
                var _result = ChatRoomService.ChatRoomUserAccessByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.ChatRoomUserAccess");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetChatRoomAnnouncements(long ChatRoomId, int Paging)
        {
            try
            {
                var _messages = ChatService.GetAnnouncementsByChatRoomId(ChatRoomId, Paging, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _messages };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetChatRoomAnnouncements");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetNewChatRoomAnnouncements(long ChatRoomId, long last)
        {
            try
            {
                var _messages = ChatService.GetNewAnnoucementsByChatRoomId(ChatRoomId, last, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _messages };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetNewChatRoomAnnouncements");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel ChatRoomBlockUser(long ChatRoomId, long UserId)
        {
            try
            {
                if (ChatRoomService.IsChatRoomOwnerByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext) &&
                    (ChatRoomService.BlockChatRoomUser(ChatRoomId, UserId, contextService.UserId, _mapper, _dbContext)))
                {
                    return new ResponseModel { status = "Success" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.ChatRoomBlockUser");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel ChatRoomUnBlockUser(long ChatRoomId, long UserId)
        {
            try
            {
                if (ChatRoomService.IsChatRoomOwnerByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext) &&
                    (ChatRoomService.UnBlockChatRoomUser(ChatRoomId, UserId, contextService.UserId, _mapper, _dbContext)))
                {
                    return new ResponseModel { status = "Success" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.ChatRoomBlockUser");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel PrivateRoomBlockUser(long ChatRoomId)
        {
            try
            {
                if (ChatRoomService.IsActivePrivateChatRoomUserByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    var _member = ChatRoomService.GetChatRoomUsersByChatRoomId(ChatRoomId, _mapper, _dbContext).Where(_ => _.UserId != contextService.UserId).Select(_ => _).FirstOrDefault();

                    if (_member != null && (ChatRoomService.BlockChatRoomUser(ChatRoomId, _member.UserId, contextService.UserId, _mapper, _dbContext)))
                    {
                        return new ResponseModel { status = "Success" };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.PrivateRoomBlockUser");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel PrivateRoomUnBlockUser(long ChatRoomId)
        {
            try
            {
                if (ChatRoomService.IsActivePrivateChatRoomUserByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    var _member = ChatRoomService.GetChatRoomUsersByChatRoomId(ChatRoomId, _mapper, _dbContext).Where(_ => _.UserId != contextService.UserId).Select(_ => _).FirstOrDefault();

                    if (_member != null && (ChatRoomService.UnBlockChatRoomUser(ChatRoomId, _member.UserId, contextService.UserId, _mapper, _dbContext)))
                    {
                        return new ResponseModel { status = "Success" };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.ChatRoomBlockUser");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel CheckUserJoinChatRoom(long ChatRoomId)
        {
            try
            {
                var _room = ChatRoomService.GetChatRoomDetailsByIdUserId(ChatRoomId, contextService.UserId, _mapper, _dbContext);

                if (_room.RoomType == MatesUpConstants.MatesUpArenaId || _room.RoomType == (int)MatesUpConstants.ChatRoomType.Private
                        || _room.NeedPassword)
                {
                    return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
                }

                if (ChatRoomService.IsActiveChatRoomUserByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    return new ResponseModel { status = "Success", Validation = true };
                }
                else if (!ChatRoomService.IsBlockedChatRoomUserByChatRoomId(ChatRoomId, contextService.UserId, _mapper, _dbContext)
                    && ChatRoomService.AddChatRoomUser(ChatRoomId, contextService.UserId, _mapper, _dbContext))
                {
                    return new ResponseModel { status = "Success", Validation = true };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.CheckUserJoinChatRoom");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };

        }

        [HttpGet]
        public ResponseModel GetUserChatRoomsList(long UserId, int Paging)
        {
            try
            {
                var _rooms = ChatRoomService.GetUserChatRooms(UserId, contextService.UserId, Paging, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _rooms };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetUserChatRoomsList");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpPost]
        public ResponseModel GetUsersList(int paging, UserSearchModel search)
        {
            try
            {
                var _currentUser = contextService.UserContext.UserProfile;

                var _users = ChatRoomService.GetUsersList(_currentUser, search, paging, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _users };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetUsersList");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpPost]
        public ResponseModel GetRoomsList(int paging, RoomSearchModel search)
        {
            try
            {
                var _rooms = ChatRoomService.GetRoomsList(search, paging, contextService.UserId, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _rooms };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatController.GetRoomsList");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }
    }
}
