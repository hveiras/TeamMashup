using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamMashup.Core.Domain;
using TeamMashup.Membership;
using TeamMashup.Models.Internal;

namespace TeamMashup.Server
{
    public class ChatHub : Hub
    {
        public void JoinChat()
        {
            var onlineUsersDictionary = JoinChat(WebSecurity.CurrentUserId, Context.ConnectionId);
            var ids = onlineUsersDictionary.Select(x => x.Key).ToList();

            using (var context = new DatabaseContext())
            {
                var onlineUsers = (from u in context.Users.FilterByIds(ids)
                                   select new UserChatModel
                                   {
                                       Id = u.Id,
                                       Name = u.Name
                                   }).ToList();

                Clients.All.refreshOnlineUsers(onlineUsers);
            }
        }

        public void LeaveChat()
        {
            var onlineUsersDictionary = LeaveChat(WebSecurity.CurrentUserId);
            var ids = onlineUsersDictionary.Select(x => x.Key).ToList();

            using (var context = new DatabaseContext())
            {
                var onlineUsers = context.Users.FilterByIds(ids)
                                         .Select(x => new UserChatModel
                                         {
                                             Id = x.Id,
                                             Name = x.Name
                                         }).ToList();

                Clients.All.refreshOnlineUsers(onlineUsers);
            }
        }

        public void Send(long callerId, long receiverId, string message)
        {
            var onlineUsers = GetOnlineUsers();

            using (var context = new DatabaseContext())
            {
                User caller;
                if (context.Users.TryGetById(callerId, out caller))
                {
                    if (onlineUsers.ContainsKey(receiverId))
                    {
                        var connectionId = onlineUsers[receiverId];

                        Clients.Client(connectionId).showMessage(caller.Id, caller.FirstName, message);
                    }
                }
            }

        }

        private IDictionary<long, string> JoinChat(long userId, string connectionId)
        {
            var onlineUsers = GetOnlineUsers();

            if (!onlineUsers.ContainsKey(userId))
            {
                onlineUsers.Add(new KeyValuePair<long, string>(userId, connectionId));
            }
            else
            {
                onlineUsers[userId] = connectionId;
            }

            SetOnlineUsers(onlineUsers);

            return onlineUsers;
        }

        private IDictionary<long, string> LeaveChat(long userId)
        {
            var onlineUsers = GetOnlineUsers();

            if (!onlineUsers.ContainsKey(userId))
                return onlineUsers;

            onlineUsers.Remove(userId);

            SetOnlineUsers(onlineUsers);

            return onlineUsers;
        }

        private IDictionary<long, string> GetOnlineUsers()
        {
            return (IDictionary<long, string>)(HttpContext.Current.Application["ChatUsers"] ?? new Dictionary<long, string>());
        }

        private void SetOnlineUsers(IDictionary<long, string> data)
        {
            try
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application["ChatUsers"] = data;
            }
            finally
            {
                HttpContext.Current.Application.UnLock();
            }
        }
    }
}