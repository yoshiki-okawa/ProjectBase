using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using PB.Frameworks.Common.Unity;
using PB.Frameworks.Types.Interfaces;
using PB.Services.DataContracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PB.Hubs
{
	public class ChatHub : Hub
	{
		// In-memory groups for demo.
		private static Dictionary<string, int> groups = new Dictionary<string, int>();
		private static Dictionary<string, List<string>> connectionGroups = new Dictionary<string, List<string>>();

		public async Task JoinGroup(DCJoinGroupArgs args)
		{
			string group = args.Group;
			await Groups.Add(Context.ConnectionId, group);
			lock (groups)
			{
				if (groups.ContainsKey(group))
					groups[group]++;
				else
					groups[group] = 1;

				Clients.All.addGroup(new DCGroup { Name = group, Count = groups[group] });
			}
			lock (connectionGroups)
			{
				List<string> hs = new List<string>();
				if (!connectionGroups.TryGetValue(Context.ConnectionId, out hs))
				{
					hs = new List<string>();
					connectionGroups[Context.ConnectionId] = hs;
				}
				hs.Add(group);
			}
		}

		public void BroadcastToGroup(DCBroadcastToGroupArgs args)
		{
			var ctx = UnityHelper.Current.Resolve<IContext>();
			Clients.Group(args.Group).addMessage(new DCMessage { Name = ctx.User.Name, Message = args.Message });
		}

		public void LeaveGroup()
		{
			lock (groups)
				lock (connectionGroups)
				{
					List<string> hs = new List<string>();
					if (connectionGroups.TryGetValue(Context.ConnectionId, out hs))
					{
						foreach (string s in hs)
						{
							if (groups.ContainsKey(s))
								groups[s]--;
							Clients.All.addGroup(new DCGroup { Name = s, Count = groups[s] });
							if (groups[s] <= 0)
								groups.Remove(s);
						}
						connectionGroups.Remove(Context.ConnectionId);
					}
				}
		}

		public override Task OnConnected()
		{
			lock (groups)
				Clients.All.addGroups(groups.Select(x => new DCGroup { Name = x.Key, Count = x.Value }).ToArray());
			return base.OnConnected();
		}

		public override Task OnDisconnected()
		{
			lock (groups)
				lock (connectionGroups)
				{
					List<string> hs = new List<string>();
					if (connectionGroups.TryGetValue(Context.ConnectionId, out hs))
					{
						foreach (string s in hs)
						{
							if (groups.ContainsKey(s))
								groups[s]--;
							if (groups[s] <= 0)
								groups.Remove(s);
						}
					}
					connectionGroups.Remove(Context.ConnectionId);
				}

			return base.OnDisconnected();
		}
	}

	public interface IChatHubClient
	{
		void addMessage(DCMessage msg);
		void addGroup(DCGroup msg);
		void addGroups(DCGroup[] msg);
	}
}
