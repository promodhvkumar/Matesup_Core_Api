using System.Linq;
using Microsoft.EntityFrameworkCore;

using Iprogs.Matesup.Models;
using Iprogs.Matesup.CoreAPI.Models;
using AutoMapper;
using Iprogs.Matesup.CoreAPI.Utilities;

namespace Iprogs.Matesup.CoreAPI.Services
{
    public static class UserService
    {
        #region ProfileDetails

        public static UserProfileMasterDTO GetUserProfileByUserId(long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _profile = dbContext.UserProfileMaster.
                                Include(_ => _.GenderNavigation).
                                Include(_ => _.CountryNavigation).
                                Include(_ => _.StateNavigation).
                                Include(_ => _.CityNavigation).
                                Where(_ => _.UserId == UserId).
                                FirstOrDefault();

                if (_profile != null)
                {
                    return mapper.Map<UserProfileMaster, UserProfileMasterDTO>(_profile);
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.GetUserProfileByUserId", ex);
            }

            return null;
        }

        public static bool SaveUserProfile(UserProfileMasterDTO UserProfile, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _exisingProfile = dbContext.UserProfileMaster.Where(_ => _.UserId == UserProfile.UserId).FirstOrDefault();

                if (_exisingProfile != null)
                {
                    _exisingProfile.FirstName = UserProfile.FirstName;
                    _exisingProfile.LastName = UserProfile.LastName;
                    _exisingProfile.NickName = !CheckNickName(UserProfile.NickName, UserProfile.UserId, mapper, dbContext) ? UserProfile.NickName : ("User" + UserProfile.UserId + DateTime.UtcNow.ToString("hhmmss"));
                    _exisingProfile.Gender = UserProfile.Gender;
                    _exisingProfile.DOB = UserProfile.DOB.HasValue ? UserProfile.DOB : null;
                    _exisingProfile.Description = UserProfile.Description;
                    if (UserProfile.DOB.HasValue)
                    {
                        _exisingProfile.Age = (int)DateTime.UtcNow.Date.Subtract(DateTimeUtility.ToDateTime(UserProfile.DOB.Value)).TotalDays / 365;
                    }
                    _exisingProfile.Country = UserProfile.Country;
                    _exisingProfile.State = UserProfile.State;
                    _exisingProfile.City = UserProfile.City;
                }
                else
                {
                    var _newProfile = new UserProfileMaster();
                    _newProfile.UserId = UserProfile.UserId;
                    _newProfile.FirstName = "User" + UserProfile.UserId + DateTime.UtcNow.ToString("hhmmss");
                    _newProfile.NickName = _newProfile.FirstName;
                    _newProfile.Gender = 4;
                    _newProfile.Country = 101;      //India
                    _newProfile.State = 1659;       //Tamil Nadu
                    _newProfile.City = 19210;       //Chennai

                    dbContext.UserProfileMaster.Add(_newProfile);
                }

                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.SaveUserProfile", ex);
            }

            return false;
        }

        public static UserPicsDTO GetUserProfilePicByUserId(long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _exisingProfile = dbContext.UserPics.Where(_ => _.UserId == UserId).FirstOrDefault();

                if (_exisingProfile != null)
                {
                    return mapper.Map<UserPics, UserPicsDTO>(_exisingProfile);
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.GetUserProfilePicByUserId", ex);
            }

            return null;
        }

        public static bool SaveProfilePic(long UserId, byte[] ProfilePic, string FileName, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _exisingProfile = dbContext.UserPics.Where(_ => _.UserId == UserId).FirstOrDefault();

                if (_exisingProfile != null)
                {
                    _exisingProfile.ProfilePic = ProfilePic;
                    _exisingProfile.ProfilePicName = FileName;
                }
                else
                {
                    var _newProfile = new UserPics();
                    _newProfile.UserId = UserId;
                    _newProfile.ProfilePic = ProfilePic;
                    _newProfile.ProfilePicName = FileName;

                    dbContext.UserPics.Add(_newProfile);
                }

                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.SaveProfilePic", ex);
            }

            return false;
        }

        public static bool CheckNickName(string NickName, long CurrentUserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _exists = dbContext.UserProfileMaster.Where(_ => _.NickName.ToLower() == NickName.ToLower()
                        && _.UserId != CurrentUserId).Any();

                return _exists;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.CheckNickName", ex);
            }

            return false;
        }

        public static void UpdateLastLoginByUserId(long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                if (dbContext.ActiveUsers.Where(_ => _.UserId == UserId).Any())
                {
                    dbContext.ActiveUsers.Where(_ => _.UserId == UserId).FirstOrDefault().LastLogin = DateTime.UtcNow;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.ActiveUsers.Add(new ActiveUsers()
                    {
                        UserId = UserId,
                        LastActive = DateTime.UtcNow,
                        LastLogin = DateTime.UtcNow,
                        LastNotification = DateTime.UtcNow
                    });
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.UpdateLastLoginByUserId", ex);
            }
        }

        public static bool UpdateLastActiveByUserId(long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                var _user = dbContext.ActiveUsers.Where(_ => _.UserId == UserId).Select(_ => _).FirstOrDefault();
                if (_user != null)
                {
                    _user.LastActive = DateTime.UtcNow;
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.ActiveUsers.Add(new ActiveUsers()
                    {
                        UserId = UserId,
                        LastActive = DateTime.UtcNow,
                        LastLogin = DateTime.UtcNow,
                        LastNotification = DateTime.UtcNow
                    });
                    dbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.UpdateLastActiveByUserId", ex);
            }

            return false;
        }

        public static bool UpdateLastNotificationByUserId(long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                if (dbContext.ActiveUsers.Where(_ => _.UserId == UserId).Any())
                {
                    var _notification = dbContext.ActiveUsers.Where(_ => _.UserId == UserId).FirstOrDefault();
                    if (_notification != null)
                    {
                        _notification.LastActive = DateTime.UtcNow;
                        _notification.LastNotification = DateTime.UtcNow;
                    }
                    dbContext.SaveChanges();
                }
                else
                {
                    dbContext.ActiveUsers.Add(new ActiveUsers()
                    {
                        UserId = UserId,
                        LastActive = DateTime.UtcNow,
                        LastLogin = DateTime.UtcNow,
                        LastNotification = DateTime.UtcNow
                    });
                    dbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.UpdateLastNotificationByUserId", ex);
            }

            return false;
        }

        public static void UserArenaEntryAnnouncement(long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                DBContextService.MatesUpArenaAnnouncement(UserId, "has entered the MatesUp Arena!", 1, mapper, dbContext);
                //dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.UserArenaEntry", ex);
            }
        }

        public static void UserArenaCreationAnnouncement(long UserId, long ChatRoomId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                DBContextService.MatesUpArenaAnnouncement(UserId, "has created a public Arena!", ChatRoomId, mapper, dbContext);
                //dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.UserArenaEntry", ex);
            }
        }

        public static void UserChatRoomEntryAnnouncement(long ChatRoomId, long UserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                DBContextService.ChatRoomAnnouncement(ChatRoomId, UserId, "has entered the Arena!", null, mapper, dbContext);
                //dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.UserChatRoomEntryAnnouncement", ex);
            }
        }

        public static List<UserModalModel> GetFollowersByUserId(long UserId, long CurrentUserId, IMapper mapper, DevContext dbContext)
        {
            var _results = new List<UserModalModel>();
            try
            {
                var _followers = dbContext.FollowersMaster.
                                    Include(_ => _.User).
                                    Include(_ => _.User.CityNavigation).
                                    Include(_ => _.User.CountryNavigation).
                                    Include(_ => _.User.GenderNavigation).
                                    Include(_ => _.User.StateNavigation).
                                    Include(_ => _.User.ActiveUsers).
                                    Include(_ => _.User.User.EmailConfirmed).
                                    Where(_ => _.UserId == UserId).
                                    OrderByDescending(_ => _.FollowedOn).
                                    Select(_ => new
                                    {
                                        _.Id,
                                        _.UserId,
                                        _.FollowerUserId,
                                        UserDetails = _.User
                                    }).
                                    ToList();

                if (_followers.Any())
                {
                    _followers.ForEach(_ =>
                    {
                        if (_.UserDetails != null)
                        {
                            _results.Add(new UserModalModel()
                            {
                                UserId = _.UserDetails.UserId,
                                FirstName = _.UserDetails.FirstName,
                                LastName = _.UserDetails.LastName,
                                NickName = _.UserDetails.NickName,
                                Description = _.UserDetails.Description,
                                Gender = _.UserDetails.Gender,
                                CityNavigation = mapper.Map<LookupCity, LookupCityDTO>(_.UserDetails.CityNavigation),
                                CountryNavigation = mapper.Map<LookupCountry, LookupCountryDTO>(_.UserDetails.CountryNavigation),
                                StateNavigation = mapper.Map<LookupState, LookupStateDTO>(_.UserDetails.StateNavigation),
                                GenderNavigation = mapper.Map<LookupGender, LookupGenderDTO>(_.UserDetails.GenderNavigation),
                                Online = _.UserDetails.ActiveUsers.LastActive > DateTime.UtcNow.AddMinutes(-15),
                                VerifiedUser = _.UserDetails.User.EmailConfirmed
                            });
                        }
                    });

                    return _results;
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.GetFollowersByUserId", ex);
            }

            return null;

        }

        public static List<UserModalModel> GetFollowingsByUserId(long UserId, long CurrentUserId, IMapper mapper, DevContext dbContext)
        {
            var _results = new List<UserModalModel>();
            try
            {
                var _followers = dbContext.FollowersMaster.
                        Include(_ => _.User).
                                    Include(_ => _.User.CityNavigation).
                                    Include(_ => _.User.CountryNavigation).
                                    Include(_ => _.User.GenderNavigation).
                                    Include(_ => _.User.StateNavigation).
                                    Include(_ => _.User.ActiveUsers).
                                    Include(_ => _.User.User.EmailConfirmed).
                                    Where(_ => _.FollowerUserId == UserId).
                                    OrderByDescending(_ => _.FollowedOn).
                                    Select(_ => new
                                    {
                                        _.Id,
                                        _.UserId,
                                        _.FollowerUserId,
                                        UserDetails = _.FollowerUser
                                    }).
                                    ToList();

                if (_followers.Any())
                {
                    _followers.ForEach(_ =>
                    {
                        if (_.UserDetails != null)
                        {
                            _results.Add(new UserModalModel()
                            {
                                UserId = _.UserDetails.UserId,
                                FirstName = _.UserDetails.FirstName,
                                LastName = _.UserDetails.LastName,
                                NickName = _.UserDetails.NickName,
                                Description = _.UserDetails.Description,
                                Gender = _.UserDetails.Gender,
                                CityNavigation = mapper.Map<LookupCity, LookupCityDTO>(_.UserDetails.CityNavigation),
                                CountryNavigation = mapper.Map<LookupCountry, LookupCountryDTO>(_.UserDetails.CountryNavigation),
                                StateNavigation = mapper.Map<LookupState, LookupStateDTO>(_.UserDetails.StateNavigation),
                                GenderNavigation = mapper.Map<LookupGender, LookupGenderDTO>(_.UserDetails.GenderNavigation),
                                VerifiedUser = _.UserDetails.User.EmailConfirmed,
                                Online = _.UserDetails.ActiveUsers.LastActive >= DateTime.UtcNow.AddMinutes(-15) ? true : false
                            });
                        }
                    });

                    return _results;
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.GetFollowingsByUserId", ex);
            }

            return null;

        }

        public static bool FollowUser(long UserId, long FollowerUserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                if (!dbContext.FollowersMaster.Where(_ => _.UserId == UserId && _.FollowerUserId == FollowerUserId).Any())
                {
                    dbContext.FollowersMaster.Add(new FollowersMaster()
                    {
                        FollowedOn = DateTime.UtcNow,
                        FollowerUserId = FollowerUserId,
                        UserId = UserId
                    });
                }
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("UserService.FollowUser", ex);
            }

            return false;
        }

        public static bool UnFollowUser(long UserId, long FollowerUserId, IMapper mapper, DevContext dbContext)
        {
            try
            {
                if (dbContext.FollowersMaster.Where(_ => _.UserId == UserId && _.FollowerUserId == FollowerUserId).Any())
                {
                    var _items = dbContext.FollowersMaster.Where(_ => _.UserId == UserId && _.FollowerUserId == FollowerUserId).Select(_ => _).ToList();

                    dbContext.FollowersMaster.RemoveRange(_items);
                }
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;//throw;//LogService.WriteErrorLog("UserService.UnFollowUser", ex);
            }

            return false;
        }

        #endregion ProfileDetails
    }
}