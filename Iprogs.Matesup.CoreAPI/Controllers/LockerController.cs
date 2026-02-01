using AutoMapper;
using Iprogs.Matesup.CoreAPI.Models;
using Iprogs.Matesup.CoreAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iprogs.Matesup.CoreAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LockerController : ControllerBase
    {
        private readonly ILogger<LockerController> _logger;
        private readonly IMapper _mapper;
        private readonly DevContext _dbContext;

        public LockerController(ILogger<LockerController> logger, IMapper mapper, DevContext dbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult ProfilePic(long Id)
        {
            try
            {
                var _userProfile = UserService.GetUserProfilePicByUserId(Id, _mapper, _dbContext);

                if (_userProfile != null && _userProfile.ProfilePic != null && _userProfile.ProfilePic.Length > 0)
                {
                    return File(_userProfile.ProfilePic, "image/jpeg", _userProfile.UserId + _userProfile.ProfilePicName);
                }
                else
                {
                    return Redirect("/matesup.jpg");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LockerController.ProfilePic");
            }

            return BadRequest();
        }
    }
}
