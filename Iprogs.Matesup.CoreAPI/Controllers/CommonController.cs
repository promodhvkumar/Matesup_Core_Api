using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Iprogs.Matesup.CoreAPI.Services;
using Iprogs.Matesup.CoreAPI.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Data;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Iprogs.Matesup.Models;

namespace Iprogs.Matesup.CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ILogger<CommonController> _logger;
        private readonly IMapper _mapper;
        private readonly DevContext _dbContext;

        public CommonController(ILogger<CommonController> logger, IMapper mapper, DevContext dbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get Country Lookup List
        /// </summary>
        /// <returns></returns>
        [Route("GetLookupCountry")]
        [HttpGet]
        public List<LookupCountryDTO> GetLookupCountry()
        {
            try
            {
                return CommonService.GetLookupCountry(_mapper, _dbContext);
                //return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Common.GetLookupCountry");
            }

            return new List<LookupCountryDTO>();
        }

        /// <summary>
        /// Get State Lookup List by the Country Id
        /// </summary>
        /// <returns></returns>
        [Route("GetLookupState")]
        [HttpGet]
        public List<LookupStateDTO> GetLookupState(int CountryId)
        {
            try
            {
                return CommonService.GetLookupStateByCountryId(CountryId, _mapper, _dbContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Common.GetLookupState");
            }

            return new List<LookupStateDTO>();
        }

        /// <summary>
        /// Get City Lookup List by the State Id
        /// </summary>
        /// <returns></returns>
        [Route("GetLookupCity")]
        [HttpGet]
        public List<LookupCityDTO> GetLookupCity(int StateId)
        {
            try
            {
                return CommonService.GetLookupCityByStateId(StateId, _mapper, _dbContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Common.GetLookupCity");
            }

            return new List<LookupCityDTO>();
        }

        /// <summary>
        /// Get RoomType Lookup
        /// </summary>
        /// <returns></returns>
        [Route("GetLookupRoomType")]
        [HttpGet]
        public List<LookupChatRoomTypeDTO> GetLookupRoomType()
        {
            try
            {
                return CommonService.GetLookupRoomType(_mapper, _dbContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Common.GetLookupRoomType");
            }

            return new List<LookupChatRoomTypeDTO>();
        }

        /// <summary>
        /// Get Room Privacy Lookup
        /// </summary>
        /// <returns></returns>
        [Route("GetLookupRoomPrivacy")]
        [HttpGet]
        public List<LookupChatRoomPrivacyDTO> GetLookupRoomPrivacy()
        {
            try
            {
                return CommonService.GetLookupRoomPrivacy(_mapper, _dbContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Common.GetLookupRoomPrivacy");
            }

            return new List<LookupChatRoomPrivacyDTO>();
        }
    }
}