using AutoMapper;
using Iprogs.Matesup.CoreAPI.Models;
using Iprogs.Matesup.CoreAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Iprogs.Matesup.CoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomController : ControllerBase
    {
        private readonly ILogger<ChatRoomController> _logger;
        private readonly IMapper _mapper;
        private readonly DevContext _dbContext;
        private readonly SignInManager<IdentityUser<long>> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ContextService contextService;

        public ChatRoomController(ILogger<ChatRoomController> logger, IMapper mapper, DevContext dbContext, SignInManager<IdentityUser<long>> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;

            contextService = new ContextService(_httpContextAccessor, _signInManager, _mapper, _dbContext);
        }

        //[AllowAnonymous]
        [HttpGet]
        public ResponseModel GetActiveChatRoomsList()
        {
            //var list = ChatRoomService.GetMyActiveChatRoomsByUserId(1, _mapper, _dbContext);
            //return Ok(list);

            //UserService.UserArenaEntryAnnouncement(1, _mapper, _dbContext);

            var context = contextService.GetUserContext();

            return new ResponseModel { status = "Success", Data = ChatRoomService.GetActiveUsersList(contextService.UserContext.UserProfile, 1, _mapper, _dbContext) };
        }
    }
}
