module Application.Controllers
{
	import Services = Application.Services;

	export class AccountController extends BaseController
	{
		constructor($scope: ng.IScope, $rootScope: ng.IScope, accountService: Services.IAccountService)
		{
			super($scope, $rootScope, accountService);
			this.scope.LoadUserDetails = () => this.LoadUserDetails();
			this.scope.userName = "";
			this.scope.email = "";
		}

		public LoadUserDetails(): void
		{
			if (!this.rootScope.isLoggedIn)
				this.AutoLogin(() => this.LoadUserDetailsInternal());
			else
				this.LoadUserDetailsInternal();
		}
		private LoadUserDetailsInternal(): void
		{
			if (this.rootScope.isLoggedIn)
			{
				this.accountService.LoadUserDetails((result: any) => { this.scope.userName = result.UserName; this.scope.email = result.Email });
			}
		}
	}
}