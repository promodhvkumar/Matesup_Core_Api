using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using NLog;

using Iprogs.Matesup.CoreAPI.Services;
using Iprogs.Matesup.Models;
using Iprogs.Matesup.CoreAPI.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace Iprogs.Matesup.CoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly DevContext _dbContext;
        private readonly SignInManager<IdentityUser<long>> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ContextService contextService;

        public HomeController(ILogger<HomeController> logger, IMapper mapper, DevContext dbContext, SignInManager<IdentityUser<long>> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;

            contextService = new ContextService(_httpContextAccessor, _signInManager, _mapper, _dbContext);
        }

        [AllowAnonymous]
        [HttpGet]
        public ResponseModel Connectivity()
        {
            return new ResponseModel { status = "Success" };
        }

        [HttpGet]
        public ResponseModel GetUserContext()
        {
            try
            {
                if (contextService.UserContext != null && contextService.UserContext.UserProfile != null)
                {
                    return new ResponseModel { status = "Success", Data = contextService.UserContext };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetUserContext");
            }

            return new ResponseModel { status = "Failure", responseText="Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetMyChatRooms()
        {
            try
            {
                var _rooms = ChatRoomService.GetMyActiveChatRoomsByUserId(contextService.UserId, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _rooms };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetMyChatRooms");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetActiveUsersList(int Gender)
        {
            try
            {
                var _currentUser = contextService.UserContext.UserProfile;

                var _users = ChatRoomService.GetActiveUsersList(_currentUser, Gender, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _users };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetActiveUsersList");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetArenaMessages(int Paging)
        {
            try
            {
                var _messages = ChatService.GetChatMessagesByChatRoomId(MatesUpConstants.MatesUpArenaId, Paging, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _messages };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetArenaMessages");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetNewArenaMessages(long last)
        {
            try
            {
                var _messages = ChatService.GetNewChatMessagesByChatRoomId(MatesUpConstants.MatesUpArenaId, last, _mapper, _dbContext);

                return new ResponseModel { Data = _messages };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetArenaMessages");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpPost]
        public ResponseModel ArenaMessage(ChatMessageModel Message)
        {
            try
            {
                if (Message != null && Message.Message != null)
                {
                    if (Message.Message.Length > 204800)
                    {
                        return new ResponseModel { status = "Failure", responseText = "Content size Exceeded the Limit!" };
                    }

                    if (ChatRoomService.IsActiveChatRoomUserByChatRoomId(MatesUpConstants.MatesUpArenaId, contextService.UserId, _mapper, _dbContext))
                    {
                        var _message = new ChatMasterDTO();
                        _message.UserId = contextService.UserId;
                        _message.ChatRoomId = MatesUpConstants.MatesUpArenaId;
                        _message.Message = Message.Message;
                        _message.MessageType = 1;
                        _message.SentOn = DateTime.UtcNow;

                        if (ChatService.SaveChatMessage(_message, _mapper, _dbContext))
                        {
                            return new ResponseModel { status = "Success", responseText = "Message Sent Successfully!" };
                        }
                    }
                    else
                    {
                        return new ResponseModel { status = "Failure", responseText = "You're not allowed to send messages to this Chat Room" };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.ArenaMessage");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel ClearArenaMessages()
        {
            try
            {
                if (ChatRoomService.IsChatRoomOwnerByChatRoomId(MatesUpConstants.MatesUpArenaId, contextService.UserId, _mapper, _dbContext))
                {
                    if (ChatRoomService.ClearChatRoomMessagesByChatRoomId(MatesUpConstants.MatesUpArenaId, _mapper, _dbContext))
                    {
                        return new ResponseModel { status = "Success", responseText = "Messages cleared Successfully!" };
                    }
                }
                else
                {
                    return new ResponseModel { status = "Failure", responseText = "User not authorised!" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.ClearArenaMessages");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel ClearArenaAnnouncements()
        {
            try
            {
                if (ChatRoomService.IsChatRoomOwnerByChatRoomId(MatesUpConstants.MatesUpArenaId, contextService.UserId, _mapper, _dbContext))
                {
                    if (ChatRoomService.ClearChatRoomAnnoucementsByChatRoomId(MatesUpConstants.MatesUpArenaId, _mapper, _dbContext))
                    {
                        return new ResponseModel { status = "Success", responseText = "Messages cleared Successfully!" };
                    }
                }
                else
                {
                    return new ResponseModel { status = "Failure", responseText = "User not authorised!" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.ClearArenaMessages");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetArenaUserAccess()
        {
            try
            {
                var _result = ChatRoomService.ChatRoomUserAccessByChatRoomId(MatesUpConstants.MatesUpArenaId, contextService.UserId, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetArenaUserAccess");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetArenaMembers()
        {
            try
            {
                if (ChatRoomService.IsChatRoomOwnerByChatRoomId(MatesUpConstants.MatesUpArenaId, contextService.UserId, _mapper, _dbContext)
                        || ChatRoomService.IsActiveChatRoomUserByChatRoomId(MatesUpConstants.MatesUpArenaId, contextService.UserId, _mapper, _dbContext))
                {
                    var _result = ChatRoomService.GetChatRoomUsersByChatRoomId(MatesUpConstants.MatesUpArenaId, _mapper, _dbContext);
                    return new ResponseModel { status = "Success", Data = JsonConvert.SerializeObject(_result) };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetArenaUserAccess");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetNotifications()
        {
            try
            {
                var _chats = ChatRoomService.GetMessageNotifications(contextService.UserId, 10, _mapper, _dbContext);
                int _newMessagesCount = 0;
                int _chatsCount = 0;

                if (_chats != null && _chats.Any())
                {
                    _newMessagesCount = _chats.Sum(_ => _.NewMessageCount);
                    _chatsCount = _chats.Count();
                }

                var _taggedChats = ChatRoomService.GetMyPrivateChatRooms(contextService.UserId, 10, string.Empty, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = new NotificationModel { NewMessageCount = _newMessagesCount, ChatsCount = _chatsCount, TaggedChats = _taggedChats } };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetNotifications");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel ClearNotifications()
        {
            try
            {
                if (UserService.UpdateLastNotificationByUserId(contextService.UserId, _mapper, _dbContext))
                {
                    return new ResponseModel { status = "Success" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetNotifications");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetArenaAnnouncements(int Paging)
        {
            try
            {
                var _messages = ChatService.GetAnnouncementsByChatRoomId(MatesUpConstants.MatesUpArenaId, Paging, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _messages };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetArenaAnnouncements");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel GetNewArenaAnnouncements(long last)
        {
            try
            {
                var _messages = ChatService.GetNewAnnoucementsByChatRoomId(MatesUpConstants.MatesUpArenaId, last, _mapper, _dbContext);

                return new ResponseModel { status = "Success", Data = _messages };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetNewArenaAnnouncements");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }

        [HttpGet]
        public ResponseModel UpdateUserActiveStatus()
        {
            try
            {
                if (UserService.UpdateLastActiveByUserId(contextService.UserId, _mapper, _dbContext))
                {
                    return new ResponseModel { status = "Success" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HomeController.GetNotifications");
            }

            return new ResponseModel { status = "Failure", responseText = "Error Occurred!" };
        }
    }
}
