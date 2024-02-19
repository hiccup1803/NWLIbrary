using NW.Core.Entities;
using NW.Core.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Services
{
    public interface IMessageService
    {
        //MessageTopic
        
        MessageTopic MessageTopic(int id);
        PagingModel<MessageTopic> MessageTopicsForPowerUser(int pageIndex, int pageSize, int powerUserId, int? messageType);
        PagingModel<MessageTopic> MessageTopics(int pageIndex, int pageSize, int companyId, int? messageType);
        MessageTopic InsertMessageTopic(MessageTopic messageTopic);
        MessageTopic UpdateMessageTopic(MessageTopic messageTopic);
        
        //Message
        Message Message(int id);
        //PagingModel<Message> Messages(int pageIndex, int pageSize, int memberId, int? messageType);
        PagingModel<Message> Messages(int pageIndex, int pageSize, int memberId, int[] messageTypes);
        Message GetLastPopupMessage(int memberId);
        PagingModel<Message> MessagesForMessageTopic(int pageIndex, int pageSize, int messageTopicId);
        int GetMessageCountForMember(int memberId, int? messageType);
        int GetUnreadMessageCountForMember(int memberId);
        //int GetUnreadMessageCountForMember(int memberId, int? messageType);
        int GetUnreadMessageCountForMember(int memberId, int[] messageTypes);
        int GetMessageCountForMessageTopic(int messageTopicId);
        int GetUnreadMessageCountForMessageTopic(int messageTopicId);
        void InsertMessage(Message message);
        int InsertMessageADONET_NOTTESTEDFORPERFORMANCE(Message message);
        void InsertMessages(IList<Message> messages);
        void UpdateMessage(Message message);
        void MarkMessageAsRead(int messageId, int memberId);
        void MarkMessageAsRead(Message message);
        void MarkAllMessagesAsReadForMember(int memberId, int? messageType);
        void MarkMessageAsUnread(int messageId);
        void MarkMessageAsUnread(Message message);
        void MarkAllMessagesAsUnreadForMember(int memberId, int? messageType);
        //Message Template
        MessageTemplate MessageTemplate(int id);
        IList<MessageTemplate> MessageTemplates(int companyId);
        PagingModel<MessageTemplate> MessageTemplates(int pageIndex, int pageSize, int companyId);
        void InsertMessageTemplate(MessageTemplate messageTemplate);
        void UpdateMessageTemplate(MessageTemplate messageTemplate);
        bool SendPersonalMessage(int memberId, string title, string content);
        bool SendSingleMessage(int memberId, string title, string content, int messageType, string systemName, Dictionary<string, string> customTokens);
        Message ReplaceMessageTokens(Message message, Member member);
        Message ReplaceCustomMessageTokens(Message message, Dictionary<string, string> customTokens);
    }
}
