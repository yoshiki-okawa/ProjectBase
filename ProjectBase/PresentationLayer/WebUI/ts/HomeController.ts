module Application.Controllers
{
	import Services = Application.Services;

	export class HomeController extends BaseController
	{
		constructor($scope: ng.IScope, $rootScope: ng.IScope, $cookies: ng.cookies.ICookiesService, accountService: Services.IAccountService)
		{
			super($scope, $rootScope, accountService);
			this.scope.JoinGroup = (group?: string) => this.JoinGroup(group);
			this.scope.BroadcastToGroup = () => this.BroadcastToGroup();
			this.scope.LeaveGroup = () => this.LeaveGroup();
			this.scope.messages = [];
			this.scope.formData = { joinGroupForm: {}, broadcastForm: {} };
			this.scope.groups = {};
			this.scope.group = "";
			this.scope.serverErrorMsgs = [];
			if (!this.rootScope.hubInitialized)
			{
				this.rootScope.hubInitialized = false;
				$.signalR.chatHub.client.addMessage = (msg: DCMessage) => { this.scope.messages.push({ tick: new Date().valueOf(), dateStr: new Date().toLocaleString(), name: msg.Name, msg: msg.Message }); this.scope.$apply(); };
				$.signalR.chatHub.client.addGroup = (group: DCGroup) => { if (group.Count > 0) this.scope.groups[group.Name] = group.Count; else delete this.scope.groups[group.Name]; this.scope.$apply(); };
				$.signalR.chatHub.client.addGroups = (groups: DCGroup[]) => { for (var i = 0; i < groups.length; i++) this.scope.groups[groups[i].Name] = groups[i].Count; this.scope.$apply(); };

				var token: string = $cookies["access_token"];
				if (token && token.length > 0)
					$.signalR.hub.qs = { "Bearer": token };
				$.signalR.hub.start().done(() => { this.rootScope.hubInitialized = true; this.rootScope.$apply(); });
			}
		}

		public JoinGroup(group?: string): void
		{
			if (!group)
				group = this.scope.formData.joinGroupForm.group;

			var args: DCJoinGroupArgs = { Group: group };
			$.signalR.chatHub.server.joinGroup(args).done((error: any) =>
			{
				if (error !== undefined && error.length > 0)
					this.scope.serverErrorMsgs = error;
				else
					this.scope.group = group;
				this.scope.$apply();
			});
		}

		public BroadcastToGroup(): void
		{
			var args: DCBroadcastToGroupArgs = { Group: this.scope.group, Message: this.scope.formData.broadcastForm.msg };
			$.signalR.chatHub.server.broadcastToGroup(args);
			this.scope.formData.broadcastForm.msg = "";
		}

		public LeaveGroup(): void
		{
			$.signalR.chatHub.server.leaveGroup().done(() => { this.scope.group = ""; while (this.scope.messages.length > 0) this.scope.messages.pop(); this.scope.$apply(); });
		}
	}
}