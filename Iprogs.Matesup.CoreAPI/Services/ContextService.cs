using AutoMapper;
using Iprogs.Matesup.CoreAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Iprogs.Matesup.CoreAPI.Services
{
    public class ContextService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private SignInManager<IdentityUser<long>> _signInManager;
        private IMapper _mapper;
        private DevContext _dbContext;
        public ContextService(IHttpContextAccessor httpContextAccessor, SignInManager<IdentityUser<long>> signInManager, IMapper mapper, DevContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _mapper = mapper;
            _dbContext = dbContext;

            UserContext = new UserContext();
            FetchUserContext();
        }

        private long _userId;

        public long UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
            }
        }

        public UserContext UserContext { get; set; }

        public void FetchUserContext()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _signInManager.UserManager.GetUserId(_httpContextAccessor.HttpContext.User);

                if (userId != null && Int64.TryParse(userId, out _userId))
                {
                    BuildUserContext(UserId);
                }
            }
        }

        public UserContext GetUserContext()
        {
            if (UserContext != null && UserContext.UserProfile != null)
            {
                return UserContext;
            }
            else
            {
                FetchUserContext();
                return UserContext;
            }
        }
        
        //HttpContext? IHttpContextAccessor.HttpContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void BuildUserContext(long UserId)
        {
            try
            {
                var _userProfile = ChatRoomService.GetMyDetailsById(UserId, _mapper, _dbContext);

                if (_userProfile != null)
                {
                    var _userContext = new UserContext();
                    _userContext.UserId = UserId;

                    _userContext.UserProfile = _userProfile;

                    UserContext = _userContext;

                    //SetUserContext(_userContext);
                }
            }
            catch (Exception ex)
            {
                throw;
                // LogService.WriteErrorLog("ContextService.BuildUserContext", ex);
            }
        }

        public void SetUserContext(UserContext _userContext)
        {
            UserContext = Utilities.CloneUtility.DeepClone<UserContext>(_userContext);
        }
    }
}