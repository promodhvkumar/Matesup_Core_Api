using Iprogs.Matesup.Models;
using Iprogs.Matesup.CoreAPI.Models;
using AutoMapper;

using Microsoft.EntityFrameworkCore;

using HtmlAgilityPack;

namespace Iprogs.Matesup.CoreAPI.Services
{
    public static class ChatService
    {
        public static List<ChatMessagesModel> GetChatMessagesByChatRoomId(long ChatRoomId, int Paging, IMapper mapper, DevContext _dbContext)
        {
            var _list = new List<ChatMessagesModel>();

            try
            {
                var _chatList = _dbContext.ChatMaster.
                                    Include(_ => _.User).
                                    Include(_ => _.User.Gender).
                                    Include(_ => _.User.Country).
                                    Include(_ => _.User.State).
                                    Include(_ => _.User.City).
                                    Where(_ => _.ChatRoomId == ChatRoomId).
                                    OrderByDescending(_ => _.SentOn).
                                    Skip(Paging).Take(20).
                                    Select(_ => _).
                                    ToList();

                _list = _chatList.Select(_ => mapper.Map<ChatMaster, ChatMessagesModel>(_)).ToList();

                return _list;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatService.GetChatMessagesByChatRoomId", ex);
            }

            return _list;
        }

        public static List<ChatMessagesModel> GetNewChatMessagesByChatRoomId(long ChatRoomId, long LastMessageId, IMapper mapper, DevContext _dbContext)
        {
            var _list = new List<ChatMessagesModel>();

            try
            {
                var _lastChatTime = _dbContext.ChatMaster.
                                        Where(_ => _.ChatRoomId == ChatRoomId
                                            && _.Id == LastMessageId).
                                        Select(_ => _.SentOn).FirstOrDefault();

                if (_lastChatTime != null)
                {

                    var _chatList = _dbContext.ChatMaster.
                                    Include(_ => _.User).
                                    Include(_ => _.User.Gender).
                                    Include(_ => _.User.Country).
                                    Include(_ => _.User.State).
                                    Include(_ => _.User.City).
                                    Where(_ => _.ChatRoomId == ChatRoomId
                                        && _.SentOn > _lastChatTime).
                                    OrderBy(_ => _.SentOn).
                                    Select(_ => _).
                                    ToList();

                    _list = _chatList.Select(_ => mapper.Map<ChatMaster, ChatMessagesModel>(_)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
                //LogService.WriteErrorLog("ChatService.GetNewChatMessagesByChatRoomId", ex);
            }

            return _list;
        }

        public static bool SaveChatMessage(ChatMasterDTO Message, IMapper mapper, DevContext _dbContext)
        {
            try
            {
                if (Message.Message.Length > 204800)
                {
                    return false;
                }

                return ProcessSaveHtmlMessage(Message, mapper, _dbContext);
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatService.SaveChatMessage", ex);
            }

            return false;
        }

        public static bool ConversationExists(long SenderId, long ReceiverId, IMapper mapper, DevContext _dbContext)
        {
            try
            {
                return _dbContext.ChatRoomMaster.
                        Include(_ => _.ChatRoomUserMapping).
                        Where(_ => _.ChatRoomUserMapping.Any()
                            && _.ChatRoomUserMapping.Count() == 2
                            && _.ChatRoomUserMapping.Where(x => x.UserId == SenderId).Any()
                            && _.ChatRoomUserMapping.Where(x => x.UserId == ReceiverId).Any()).
                        Any();
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatService.ConversationExists", ex);
            }

            return false;
        }

        public static ChatRoomMasterDTO GetPrivateChatRoomBySenderReceiverId(long SenderId, long ReceiverId, IMapper mapper, DevContext _dbContext)
        {
            try
            {
                var _chatRoomType = (int)MatesUpConstants.ChatRoomType.Private;

                if (_dbContext.ChatRoomMaster.
                    Include(_ => _.ChatRoomUserMapping).
                    Where(_ => _.ChatRoomUserMapping.Any()
                        && _.ChatRoomType == _chatRoomType
                        && _.ChatRoomUserMapping.Count() == 2
                        && _.ChatRoomUserMapping.Where(x => x.UserId == SenderId).Any()
                        && _.ChatRoomUserMapping.Where(x => x.UserId == ReceiverId).Any()).
                    Any())
                {
                    var _chatRoom = _dbContext.ChatRoomMaster.
                            Include(_ => _.ChatRoomUserMapping).
                            Where(_ => _.ChatRoomUserMapping.Any()
                                && _.ChatRoomUserMapping.Count() == 2
                                && _.ChatRoomType == _chatRoomType
                                && _.ChatRoomUserMapping.Where(x => x.UserId == SenderId).Any()
                                && _.ChatRoomUserMapping.Where(x => x.UserId == ReceiverId).Any()).
                            First();

                    return mapper.Map<ChatRoomMaster, ChatRoomMasterDTO>(_chatRoom);
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatService.GetPrivateChatRoomBySenderReceiverId", ex);
            }

            return null;
        }

        private static bool ProcessSaveHtmlMessage(ChatMasterDTO MasterMessage, IMapper mapper, DevContext _dbContext)
        {
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(MasterMessage.Message);

                var chatAttachments = new List<ChatMasterAttachments>();

                var nodes = GetAllNodes(htmlDoc.DocumentNode);

                if (nodes != null && nodes.Any() && nodes.Where(_ => _.NodeType == HtmlNodeType.Element && _.Name.ToLower() == "img").Any())
                {
                    var imagenodes = nodes.Where(_ => _.NodeType == HtmlNodeType.Element && _.Name.ToLower() == "img").ToList();
                    foreach (var x in imagenodes)
                    {
                        if (x.HasAttributes)
                        {
                            string src = x.GetAttributeValue("src", "");
                            if (!string.IsNullOrWhiteSpace(src))
                            {
                                if (src.Contains("data:") && src.Contains("base64"))
                                {
                                    int dataIndex = src.IndexOf("data:");
                                    int base64Index = src.IndexOf("base64");
                                    string fileType = src.Substring(dataIndex + 5, base64Index - 6);
                                    string base64File = src.Substring(base64Index + 7);

                                    string filename = x.GetAttributeValue("data-filename", "");
                                    string fileExtn = string.Empty;

                                    try
                                    {
                                        fileExtn = "." + filename.Split('.').ToList().LastOrDefault().ToLower();

                                        if (!MatesUpConstants.SupportedImageFileTypes.Contains(fileExtn))
                                        {
                                            fileExtn = string.Empty;
                                        }
                                    }
                                    catch
                                    {

                                    }

                                    var _attachment = new ChatMasterAttachments();
                                    _attachment.AttachmentName = MasterMessage.UserId.ToString() + Guid.NewGuid().ToString();
                                    _attachment.CreatedOn = DateTime.UtcNow;
                                    _attachment.FileExtn = string.IsNullOrWhiteSpace(fileExtn) ? ".jpg" : fileExtn;
                                    _attachment.FileName = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
                                    _attachment.IsActive = true;
                                    _attachment.Attachment = Convert.FromBase64String(base64File);

                                    chatAttachments.Add(_attachment);

                                    htmlDoc.DocumentNode.SelectSingleNode(x.XPath).SetAttributeValue("src", "/Chat/Attachment?id=" + _attachment.AttachmentName);
                                }
                            }
                        }
                    }
                }

                if (nodes != null && nodes.Any()
                    && nodes.Where(_ => _.NodeType == HtmlNodeType.Element
                            && (MatesUpConstants.UnsupportedHTMLTags.Contains(_.Name.ToLower()))).Any())
                {
                    var dumpnodes = nodes.Where(_ => _.NodeType == HtmlNodeType.Element
                                && (MatesUpConstants.UnsupportedHTMLTags.Contains(_.Name.ToLower()))).ToList();

                    foreach (var x in dumpnodes)
                    {
                        var descendants = htmlDoc.DocumentNode.Descendants(x.Depth).ToList();
                        if (descendants.Where(_ => _ == x).Any())
                        {
                            htmlDoc.DocumentNode.SelectSingleNode(x.XPath).Remove();
                        }
                    }
                }

                var _chat = new ChatMaster();

                _chat.ChatRoomId = MasterMessage.ChatRoomId;
                _chat.Message = htmlDoc.DocumentNode.WriteTo();
                _chat.MessageType = 1;
                _chat.SentOn = MasterMessage.SentOn;
                _chat.UserId = MasterMessage.UserId;
                _chat.ChatMasterAttachments = chatAttachments;

                _dbContext.ChatMaster.Add(_chat);

                var _chatRoom = _dbContext.ChatRoomMaster.
                                Where(_ => _.Id == MasterMessage.ChatRoomId).
                                Select(_ => _).
                                FirstOrDefault();

                if (_chatRoom != null)
                {
                    _chatRoom.LastMessageOn = DateTime.UtcNow;
                }
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatService.ProcessSaveHtmlMessage", ex);
            }

            return false;
        }

        private static List<HtmlNode> GetAllNodes(HtmlNode RootNode)
        {
            if (RootNode != null && RootNode.HasChildNodes)
                return RootNode.DescendantsAndSelf().ToList();

            return null;
        }

        private static List<HtmlNode> GetChildNodes(HtmlNode ParentNode)
        {
            if (ParentNode != null && ParentNode.HasChildNodes)
            {
                return ParentNode.ChildNodes.ToList();
            }
            return null;
        }

        public static ChatMasterAttachmentsDTO GetAttachment(string Id, long UserId, IMapper mapper, DevContext _dbContext)
        {
            try
            {
                var _attachment = _dbContext.ChatMasterAttachments.
                                    Include(_ => _.Chat.ChatRoom.ChatRoomUserMapping).
                                    Where(_ => _.AttachmentName == Id
                                        && _.Chat.ChatRoom.ChatRoomUserMapping.Where(x => x.UserId == UserId).Any()).
                                    FirstOrDefault();

                if (_attachment != null)
                {
                    return mapper.Map<ChatMasterAttachments, ChatMasterAttachmentsDTO>(_attachment);
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatService.GetAttachment", ex);
            }

            return null;
        }

        public static List<AnnouncementsModel> GetAnnouncementsByChatRoomId(long ChatRoomId, int Paging, IMapper mapper, DevContext _dbContext)
        {
            var _list = new List<AnnouncementsModel>();

            try
            {
                var _chatList = _dbContext.MegaPhoneMaster.
                                    Include(_ => _.User).
                                    Include(_ => _.User.Gender).
                                    Include(_ => _.User.Country).
                                    Include(_ => _.User.State).
                                    Include(_ => _.User.City).
                                    Include(_ => _.NewChatRoom).
                                    Where(_ => _.ChatRoomId == ChatRoomId).
                                    OrderByDescending(_ => _.SentOn).
                                    Skip(Paging).Take(20).
                                    Select(_ => _).
                                    ToList().
                                    Select(_ => new AnnouncementsModel()
                                    {
                                        Announcement = _.Announcement,
                                        ChatRoomId = _.ChatRoomId,
                                        Id = _.Id,
                                        SentOn = _.SentOn,
                                        UserId = _.UserId,
                                        UserProfileMaster = mapper.Map<UserProfileMaster, UserModalModel>(_.User),
                                        NewRoom = mapper.Map<ChatRoomMaster, RoomModalModel>(_.NewChatRoom)
                                    }).
                                    ToList();

                _list = _chatList;

                return _list;
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatService.GetAnnouncementsByChatRoomId", ex);
            }

            return _list;
        }

        public static List<AnnouncementsModel> GetNewAnnoucementsByChatRoomId(long ChatRoomId, long LastMessageId, IMapper mapper, DevContext _dbContext)
        {
            var _list = new List<AnnouncementsModel>();

            try
            {
                var _lastChatTime = _dbContext.MegaPhoneMaster.
                                        Where(_ => _.ChatRoomId == ChatRoomId
                                            && _.Id == LastMessageId).
                                        Select(_ => _.SentOn).FirstOrDefault();

                if (_lastChatTime != null)
                {
                    var _chatList = _dbContext.MegaPhoneMaster.
                                    Include(_ => _.User).
                                    Include(_ => _.User.Gender).
                                    Include(_ => _.User.Country).
                                    Include(_ => _.User.State).
                                    Include(_ => _.User.City).
                                    Where(_ => _.ChatRoomId == ChatRoomId
                                        && _.SentOn > _lastChatTime).
                                    OrderBy(_ => _.SentOn).
                                    Select(_ => _).
                                    ToList().
                                    Select(_ => new AnnouncementsModel()
                                    {
                                        Announcement = _.Announcement,
                                        ChatRoomId = _.ChatRoomId,
                                        Id = _.Id,
                                        SentOn = _.SentOn,
                                        UserId = _.UserId,
                                        UserProfileMaster = mapper.Map<UserProfileMaster, UserModalModel>(_.User),
                                        NewRoom = mapper.Map<ChatRoomMaster, RoomModalModel>(_.NewChatRoom)
                                    }).
                                    ToList();

                    _list = _chatList;
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatService.GetNewAnnoucementsByChatRoomId", ex);
            }

            return _list;
        }

        public static bool AdminWelcomeMessage(long UserId, IMapper mapper, DevContext _dbContext)
        {
            try
            {
                var _senderId = 7;
                var _chatRoom = ChatService.GetPrivateChatRoomBySenderReceiverId(_senderId, UserId, mapper, _dbContext);

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
                        LastSeen = DateTime.UtcNow
                    });

                    _chatRoom = ChatRoomService.CreatePrivateChatRoom(_chatRoom, mapper, _dbContext);

                    ChatService.SaveChatMessage(new ChatMasterDTO()
                    {
                        ChatRoomId = _chatRoom.Id,
                        UserId = _senderId,
                        MessageType = 1,
                        Message = @"<p>Welcome to MatesUp! <br/> Mingle in public arenas or Create your own Arena, share your room with your friends, enjoy your time here! <br/>"
                            + "You can view/ edit your profile right from your name on top! "
                            + "Please complete your profile for a better experience! <br/>"
                            + "-MatesUp Team"
                            + "</p>",
                        SentOn = DateTime.UtcNow.AddMinutes(2)
                    }
                    , mapper, _dbContext);

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;//LogService.WriteErrorLog("ChatService.AdminWelcomeMessage", ex);
            }

            return false;
        }
    }
}
