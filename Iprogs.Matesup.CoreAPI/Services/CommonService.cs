using Iprogs.Matesup.Models;

using Iprogs.Matesup.CoreAPI.Models;
using Microsoft.EntityFrameworkCore;

using AutoMapper;

namespace Iprogs.Matesup.CoreAPI.Services
{
    internal static class CommonService
    {
        #region Lookups

        public static List<LookupCountryDTO> GetLookupCountry(IMapper mapper, DevContext _dbContext)
        {
            try
            {
                var _list = _dbContext.LookupCountry.Select(_ => _).ToList();

                return _list.Select(_ => mapper.Map<LookupCountry, LookupCountryDTO>(_)).ToList();
            }
            catch (Exception)
            {
                throw;
                //LogService.WriteErrorLog("CommonService.GetLookupCountry", ex);
            }

            //return null;
        }

        public static List<LookupStateDTO> GetLookupStateByCountryId(int CountryId, IMapper mapper, DevContext _dbContext)
        {
            try
            {
                var _list = _dbContext.LookupState.
                                    Where(_ => _.CountryID == CountryId).
                                    Select(_ => _).ToList();

                return _list.Select(_ => mapper.Map<LookupState, LookupStateDTO>(_)).ToList();
            }
            catch (Exception ex)
            {
                throw;
                //LogService.WriteErrorLog("CommonService.GetLookupStateByCountryId", ex);
            }

            //return null;
        }

        public static List<LookupCityDTO> GetLookupCityByStateId(int StateId, IMapper mapper, DevContext _dbContext)
        {
            try
            {
                var _list = _dbContext.LookupCity.
                                    Where(_ => _.StateID == StateId).
                                    Select(_ => _).ToList();

                return _list.Select(_ => mapper.Map<LookupCity, LookupCityDTO>(_)).ToList();
            }
            catch (Exception ex)
            {
                throw;
                //LogService.WriteErrorLog("CommonService.GetLookupCityByStateId", ex);
            }

            //return null;
        }

        public static List<LookupChatRoomTypeDTO> GetLookupRoomType(IMapper mapper, DevContext _dbContext)
        {
            try
            {
                var _list = _dbContext.LookupChatRoomType.
                                    Where(_ => _.Id != 1 && _.Id != 2 //Matesup Arena or Private Messages
                                        && _.IsActive).
                                    Select(_ => _).ToList();

                return _list.Select(_ => mapper.Map<LookupChatRoomType, LookupChatRoomTypeDTO>(_)).ToList();
            }
            catch (Exception ex)
            {
                throw;
                //LogService.WriteErrorLog("CommonService.GetLookupRoomType", ex);
            }

            //return null;
        }

        public static List<LookupChatRoomPrivacyDTO> GetLookupRoomPrivacy(IMapper mapper, DevContext _dbContext)
        {
            try
            {
                var _list = _dbContext.LookupChatRoomPrivacy.
                                    Where(_ => _.IsActive).
                                    Select(_ => _).ToList();

                return _list.Select(_ => mapper.Map<LookupChatRoomPrivacy, LookupChatRoomPrivacyDTO>(_)).ToList();
            }
            catch (Exception ex)
            {
                throw;
                //LogService.WriteErrorLog("CommonService.GetLookupRoomPrivacy", ex);
            }

            //return null;
        }

        #endregion Lookups
    }
}
