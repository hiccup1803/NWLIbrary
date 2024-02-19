using DbLocalization;
using NHibernate;
using NHibernate.Criterion;
using NW.Core.Entities;
using NW.Core.Entitites;
using NW.Core.Enum;
using NW.Core.Repositories;
using NW.Core.Services;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace NW.Service.Messaging
{
    public class MessageService : BaseService, IMessageService
    {
        IRepository<MessageTopic, int> MessageTopicRepository { get; set; }
        IRepository<Message, int> MessageRepository { get; set; }
        IRepository<MessageTemplate, int> MessageTemplateRepository { get; set; }
        private IMemberRepository MemberRepository { get; set; }

        public MessageService(IRepository<MessageTopic, int> _messageTopicRepository, IRepository<Message, int> _messageRepository, IRepository<MessageTemplate, int> _messageTemplateRepository,
            IMemberRepository _memberRepository,
            IUnitOfWork _unitOfWork, ISession _session) : base(_unitOfWork, _session)
        {
            MessageTopicRepository = _messageTopicRepository;
            MessageRepository = _messageRepository;
            MessageTemplateRepository = _messageTemplateRepository;
            MemberRepository = _memberRepository;
        }


        //MessageTopic

        public MessageTopic MessageTopic(int id)
        {
            return MessageTopicRepository.Get(id);
        }
        public PagingModel<MessageTopic> MessageTopicsForPowerUser(int pageIndex, int pageSize, int powerUserId, int? messageType)
        {
            PagingModel<MessageTopic> pagingModel = new PagingModel<MessageTopic>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    if (messageType.HasValue)
                    {
                        pagingModel.TotalCount = MessageTopicRepository.GetAll().Where(mt => mt.CreatedBy == powerUserId && mt.MessageType == messageType && (mt.SystemName == null || mt.SystemName == string.Empty)).Count();
                        pagingModel.ItemList = Session.QueryOver<MessageTopic>()
                                .Where(mt => mt.CreatedBy == powerUserId && mt.MessageType == messageType && (mt.SystemName == null || mt.SystemName == string.Empty))
                                .OrderBy(mt => mt.CreateDate).Desc
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .List();
                    }
                    else
                    {
                        pagingModel.TotalCount = MessageTopicRepository.GetAll().Where(mt => mt.CreatedBy == powerUserId && (mt.SystemName == null || mt.SystemName == string.Empty)).Count();
                        pagingModel.ItemList = Session.QueryOver<MessageTopic>()
                                .Where(mt => mt.CreatedBy == powerUserId && (mt.SystemName == null || mt.SystemName == string.Empty))
                                .OrderBy(mt => mt.CreateDate).Desc
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .List();
                    }
                }
            }
            return pagingModel;
        }
        public PagingModel<MessageTopic> MessageTopics(int pageIndex, int pageSize, int companyId, int? messageType)
        {
            PagingModel<MessageTopic> pagingModel = new PagingModel<MessageTopic>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    if (messageType.HasValue)
                    {
                        pagingModel.TotalCount = MessageTopicRepository.GetAll().Where(mt => mt.CompanyId == companyId && mt.MessageType == messageType && (mt.SystemName == null || mt.SystemName == string.Empty)).Count();
                        pagingModel.ItemList = Session.QueryOver<MessageTopic>()
                                .Where(mt => mt.CompanyId == companyId && mt.MessageType == messageType && (mt.SystemName == null || mt.SystemName == string.Empty))
                                .OrderBy(mt => mt.CreateDate).Desc
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .List();
                    }
                    else
                    {
                        pagingModel.TotalCount = MessageTopicRepository.GetAll().Where(mt => mt.CompanyId == companyId && (mt.SystemName == null || mt.SystemName == string.Empty)).Count();
                        pagingModel.ItemList = Session.QueryOver<MessageTopic>()
                                .Where(mt => mt.CompanyId == companyId && (mt.SystemName == null || mt.SystemName == string.Empty))
                                .OrderBy(mt => mt.CreateDate).Desc
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .List();
                    }
                }
            }
            return pagingModel;
        }
        public MessageTopic InsertMessageTopic(MessageTopic messageTopic)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    messageTopic = MessageTopicRepository.Insert(messageTopic);
                    unitOfWork.Commit(transaction);
                    return messageTopic;
                }
            }
        }
        public MessageTopic UpdateMessageTopic(MessageTopic messageTopic)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    messageTopic = MessageTopicRepository.Update(messageTopic);
                    unitOfWork.Commit(transaction);
                    return messageTopic;
                }
            }
        }
        //Message
        public Message Message(int id)
        {
            return MessageRepository.Get(id);
        }
        /* public PagingModel<Message> Messages(int pageIndex, int pageSize, int memberId, int? messageType)
         {
             PagingModel<Message> pagingModel = new PagingModel<Message>();
             using (var unitOfWork = UnitOfWork.Current)
             {
                 List<Transaction> result = new List<Transaction>();
                 using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                 {
                     if (messageType.HasValue)
                     {
                         pagingModel.TotalCount = MessageRepository.GetAll().Where(m => m.MemberId == memberId && m.MessageType == messageType).Count();
                         pagingModel.ItemList = Session.QueryOver<Message>()
                                 .Where(m => m.MemberId == memberId && m.MessageType == messageType)
                                 .OrderBy(m => m.CreateDate).Desc
                                 .Skip(pageIndex * pageSize)
                                 .Take(pageSize)
                                 .List();
                     }
                     else
                     {
                         pagingModel.TotalCount = MessageRepository.GetAll().Where(m => m.MemberId == memberId).Count();
                         pagingModel.ItemList = Session.QueryOver<Message>()
                                 .Where(m => m.MemberId == memberId)
                                 .OrderBy(m => m.CreateDate).Desc
                                 .Skip(pageIndex * pageSize)
                                 .Take(pageSize)
                                 .List();
                     }
                 }
             }
             return pagingModel;
         }*/
        public PagingModel<Message> Messages(int pageIndex, int pageSize, int memberId, params int[] messageTypes)
        {
            PagingModel<Message> pagingModel = new PagingModel<Message>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    if (messageTypes != null && messageTypes.Length > 0)
                    {
                        pagingModel.TotalCount = MessageRepository.GetAll().Where(m => m.MemberId == memberId && messageTypes.Contains(m.MessageType) && (m.StartDate == null || m.StartDate <= DateTime.UtcNow) && (m.EndDate == null || m.EndDate > DateTime.UtcNow)).Count();
                        pagingModel.ItemList = Session.QueryOver<Message>()
                                .Where(m => m.MemberId == memberId && (m.StartDate == null || m.StartDate <= DateTime.UtcNow) && (m.EndDate == null || m.EndDate > DateTime.UtcNow))// && messageTypes.Contains(m.MessageType)) - cannot convert to sql so use restriction instead
                                .WhereRestrictionOn(m => m.MessageType)
                                .IsIn(messageTypes)
                                .OrderBy(m => m.CreateDate).Desc
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .List();
                    }
                    else
                    {
                        pagingModel.TotalCount = MessageRepository.GetAll().Where(m => m.MemberId == memberId && (m.StartDate == null || m.StartDate <= DateTime.UtcNow) && (m.EndDate == null || m.EndDate > DateTime.UtcNow)).Count();
                        pagingModel.ItemList = Session.QueryOver<Message>()
                                .Where(m => m.MemberId == memberId && (m.StartDate == null || m.StartDate <= DateTime.UtcNow) && (m.EndDate == null || m.EndDate > DateTime.UtcNow))
                                .OrderBy(m => m.CreateDate).Desc
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .List();
                    }
                }
            }
            return pagingModel;
        }

        public PagingModel<Message> MessagesForMessageTopic(int pageIndex, int pageSize, int messageTopicId)
        {
            PagingModel<Message> pagingModel = new PagingModel<Message>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MessageRepository.GetAll().Where(m => m.MessageTopicId == messageTopicId).Count();
                    pagingModel.ItemList = Session.QueryOver<Message>()
                            .Where(m => m.MessageTopicId == messageTopicId)
                            .OrderBy(m => m.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public Message GetLastPopupMessage(int memberId)
        {
            Message message = null;
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    message = Session.QueryOver<Message>()
                            .Where(m => m.MemberId == memberId && m.ReadDate == null && m.MessageType == (int)MessageType.Popup && (m.StartDate == null || m.StartDate <= DateTime.UtcNow) && (m.EndDate == null || m.EndDate > DateTime.UtcNow))
                            .OrderBy(m => m.CreateDate).Desc
                            .Take(1).List().FirstOrDefault();

                }
            }
            return message;
        }
        public int GetMessageCountForMember(int memberId, int? messageType)
        {
            if (messageType.HasValue)
            {
                return MessageRepository.GetAll().Where(m => m.MemberId == memberId && m.MessageType == messageType).Count();
            }
            else
            {
                return MessageRepository.GetAll().Where(m => m.MemberId == memberId).Count();
            }
        }
        public int GetUnreadMessageCountForMember(int memberId)
        {
            return MessageRepository.GetAll().Where(m => m.MemberId == memberId && m.ReadDate == null && m.MessageType != (int)MessageType.Popup && (m.StartDate == null || m.StartDate <= DateTime.UtcNow) && (m.EndDate == null || m.EndDate > DateTime.UtcNow)).Count();
        }
        /* public int GetUnreadMessageCountForMember(int memberId, int? messageType)
         {
             if (messageType.HasValue)
             {
                 return MessageRepository.GetAll().Where(m => m.MemberId == memberId && m.MessageType == messageType && m.ReadDate == null).Count();
             }
             else
             {
                 return MessageRepository.GetAll().Where(m => m.MemberId == memberId && m.ReadDate == null).Count();
             }
         }*/
        public int GetUnreadMessageCountForMember(int memberId, params int[] messageTypes)
        {
            if (messageTypes != null && messageTypes.Length > 0)
            {
                return MessageRepository.GetAll().Where(m => m.MemberId == memberId && messageTypes.Contains(m.MessageType) && m.ReadDate == null).Count();
            }
            else
            {
                return MessageRepository.GetAll().Where(m => m.MemberId == memberId && m.ReadDate == null).Count();
            }
        }
        public int GetMessageCountForMessageTopic(int messageTopicId)
        {
            return MessageRepository.GetAll().Where(m => m.MessageTopicId == messageTopicId).Count();
        }
        public int GetUnreadMessageCountForMessageTopic(int messageTopicId)
        {
            return MessageRepository.GetAll().Where(m => m.MessageTopicId == messageTopicId && m.ReadDate == null).Count();
        }
        public void InsertMessage(Message message)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    MessageRepository.Insert(message);
                    unitOfWork.Commit(transaction);
                }
            }
        }


        public int InsertMessageADONET_NOTTESTEDFORPERFORMANCE(Message message)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBt"].ConnectionString))
            {
                SqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT INTO Message (MessageTopicId, MemberId, MessageType, [Title], [Content], CreateDate, ReadDate, CreatedBy, StatusType) VALUES(@messageTopicId, @memberId, @messageType, @title, @content, @createDate, @readDate, @createdBy, @statusType)";


                command.Parameters.Add(new SqlParameter("@messageTopicId", message.MessageTopicId));
                command.Parameters.Add(new SqlParameter("@memberId", message.MemberId));
                command.Parameters.Add(new SqlParameter("@messageType", message.MessageType));
                command.Parameters.Add(new SqlParameter("@title", message.Title));
                command.Parameters.Add(new SqlParameter("@content", message.Content));
                command.Parameters.Add(new SqlParameter("@createDate", DateTime.UtcNow));
                command.Parameters.Add(new SqlParameter("@readDate", DBNull.Value));
                command.Parameters.Add(new SqlParameter("@createdBy", message.CreatedBy));
                command.Parameters.Add(new SqlParameter("@statusType", message.StatusType));

                conn.Open();
                int count = (int)command.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();

                return count;
            }
        }
        public void InsertMessages(IList<Message> messages)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session.SetBatchSize(50)))
                {
                    foreach (Message message in messages)
                    {
                        MessageRepository.Insert(message);
                    }
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void UpdateMessage(Message message)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    MessageRepository.Update(message);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void MarkMessageAsRead(int messageId, int memberId)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    Message message = MessageRepository.Get(messageId);
                    if (message.ReadDate == null && message.MemberId == memberId)
                    {
                        message.ReadDate = DateTime.Now;
                        MessageRepository.Update(message);
                        unitOfWork.Commit(transaction);
                    }
                }
            }
        }
        public void MarkMessageAsRead(Message message)
        {
            MarkMessageAsRead(message.Id, message.MemberId);
        }
        public void MarkAllMessagesAsReadForMember(int memberId, int? messageType)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    List<Message> messages = new List<Message>();
                    if (messageType.HasValue)
                    {
                        messages = MessageRepository.GetAll().Where(m => m.MemberId == memberId && m.MessageType == messageType).ToList();
                    }
                    else
                    {
                        messages = MessageRepository.GetAll().Where(m => m.MemberId == memberId).ToList();
                    }

                    foreach (Message message in messages)
                    {
                        if (message.ReadDate == null)
                        {
                            message.ReadDate = DateTime.Now;
                            MessageRepository.Update(message);
                        }
                    }
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void MarkMessageAsUnread(int messageId)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    Message message = MessageRepository.Get(messageId);
                    if (message.ReadDate != null)
                    {
                        message.ReadDate = null;
                        MessageRepository.Update(message);
                        unitOfWork.Commit(transaction);
                    }
                }
            }
        }
        public void MarkMessageAsUnread(Message message)
        {
            MarkMessageAsUnread(message.Id);
        }
        public void MarkAllMessagesAsUnreadForMember(int memberId, int? messageType)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    List<Message> messages = new List<Message>();
                    if (messageType.HasValue)
                    {
                        messages = MessageRepository.GetAll().Where(m => m.MemberId == memberId && m.MessageType == messageType).ToList();
                    }
                    else
                    {
                        messages = MessageRepository.GetAll().Where(m => m.MemberId == memberId).ToList();
                    }

                    foreach (Message message in messages)
                    {
                        if (message.ReadDate != null)
                        {
                            message.ReadDate = null;
                            MessageRepository.Update(message);
                        }
                    }
                    unitOfWork.Commit(transaction);
                }
            }
        }
        //Message Template
        public MessageTemplate MessageTemplate(int id)
        {
            return MessageTemplateRepository.Get(id);
        }
        public IList<MessageTemplate> MessageTemplates(int companyId)
        {
            return MessageTemplateRepository.GetAll().Where(mt => mt.CompanyId == companyId).ToList();
        }
        public PagingModel<MessageTemplate> MessageTemplates(int pageIndex, int pageSize, int companyId)
        {
            PagingModel<MessageTemplate> pagingModel = new PagingModel<MessageTemplate>();
            using (var unitOfWork = UnitOfWork.Current)
            {
                List<Transaction> result = new List<Transaction>();
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    pagingModel.TotalCount = MessageTemplateRepository.GetAll().Where(mt => mt.CompanyId == companyId).Count();
                    pagingModel.ItemList = Session.QueryOver<MessageTemplate>()
                            .Where(mt => mt.CompanyId == companyId)
                            .OrderBy(mt => mt.CreateDate).Desc
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .List();
                }
            }
            return pagingModel;
        }
        public void InsertMessageTemplate(MessageTemplate messageTemplate)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    messageTemplate.CreateDate = DateTime.Now;
                    MessageTemplateRepository.Insert(messageTemplate);
                    unitOfWork.Commit(transaction);
                }
            }
        }
        public void UpdateMessageTemplate(MessageTemplate messageTemplate)
        {
            using (var unitOfWork = UnitOfWork.Current)
            {
                using (ITransaction transaction = unitOfWork.BeginTransaction(Session))
                {
                    MessageTemplateRepository.Update(messageTemplate);
                    unitOfWork.Commit(transaction);
                }
            }
        }

        public bool SendPersonalMessage(int memberId, string title, string content)
        {
            MessageTopic messageTopic = new MessageTopic();
            Core.Entities.Member member = MemberRepository.Get(memberId);

            messageTopic.MessageType = (int)MessageType.Personal;
            messageTopic.Title = title;
            messageTopic.Content = content;
            messageTopic.StatusType = (int)StatusType.Active;
            messageTopic.CreateDate = DateTime.Now;
            messageTopic.CompanyId = member.CompanyId;

            try
            {
                messageTopic = InsertMessageTopic(messageTopic);
            }
            catch (Exception ex)
            {
                return false;
            }


            Message message = new Message();

            message.Title = title;
            message.Content = content;
            message.MessageType = (int)MessageType.Personal;
            message.StatusType = (int)StatusType.Active;
            message.CreateDate = DateTime.Now;

            message.MessageTopicId = messageTopic.Id;
            message.MemberId = memberId;


            message = ReplaceMessageTokens(message, member);

            try
            {
                InsertMessage(message);
            }
            catch (Exception ex)
            {
                return false;

            }
            return true;
        }

        public bool SendSingleMessage(int memberId, string title, string content, int messageType, string systemName, Dictionary<string, string> customTokens)
        {
            MessageTopic messageTopic = new MessageTopic();
            Core.Entities.Member member = MemberRepository.Get(memberId);
            if (!MessageTopicRepository.GetAll().Any(mt => mt.SystemName == systemName))
            {
                messageTopic.MessageType = messageType;
                messageTopic.Title = title;
                messageTopic.Content = content;
                messageTopic.StatusType = (int)StatusType.Active;
                messageTopic.CreateDate = DateTime.Now;
                messageTopic.SystemName = systemName;
                messageTopic.CompanyId = member.CompanyId;

                try
                {
                    messageTopic = InsertMessageTopic(messageTopic);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                messageTopic = MessageTopicRepository.GetAll().FirstOrDefault(mt => mt.SystemName == systemName);
            }

            Message message = new Message();

            message.Title = messageTopic.Title;
            message.Content = messageTopic.Content;
            message.MessageType = messageTopic.MessageType;
            message.StatusType = (int)StatusType.Active;
            message.CreateDate = DateTime.Now;

            message.MessageTopicId = messageTopic.Id;
            message.MemberId = memberId;


            message = ReplaceMessageTokens(message, member);
            message = ReplaceCustomMessageTokens(message, customTokens);

            try
            {
                InsertMessage(message);
            }
            catch (Exception ex)
            {
                return false;

            }
            return true;
        }


        public Message ReplaceMessageTokens(Message message, Core.Entities.Member member)
        {

            message.Title = message.Title.Replace("%%FULLNAME%%", member.FirstName + " " + member.LastName);
            message.Title = message.Title.Replace("%%FIRSTNAME%%", member.FirstName);
            message.Title = message.Title.Replace("%%LASTNAME%%", member.LastName);
            message.Title = message.Title.Replace("%%MEMBERID%%", member.Id.ToString());
            message.Title = message.Title.Replace("%%USERNAME%%", member.Username);

            message.Content = message.Content.Replace("%%FULLNAME%%", member.FirstName + " " + member.LastName);
            message.Content = message.Content.Replace("%%FIRSTNAME%%", member.FirstName);
            message.Content = message.Content.Replace("%%LASTNAME%%", member.LastName);
            message.Content = message.Content.Replace("%%MEMBERID%%", member.Id.ToString());
            message.Content = message.Content.Replace("%%USERNAME%%", member.Username);

            return message;
        }
        public Message ReplaceCustomMessageTokens(Message message, Dictionary<string, string> customTokens)
        {
            foreach (KeyValuePair<string, string> customToken in customTokens)
            {
                message.Title = message.Title.Replace(String.Format("%%{0}%%", customToken.Key), customToken.Value);
                message.Content = message.Content.Replace(String.Format("%%{0}%%", customToken.Key), customToken.Value);
            }
            return message;
        }
    }
}
