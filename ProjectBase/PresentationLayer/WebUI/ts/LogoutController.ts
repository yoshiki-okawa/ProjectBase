module Application.Controllers
{
	import Services = Application.Services;

	export class LogoutController extends BaseController
	{
		constructor($scope: ng.IScope, $rootScope: ng.IScope, accountService: Services.IAccountService)
		{
			super($scope, $rootScope, accountService);
			this.scope.Logout = () => this.Logout();
			this.scope.serverErrorMsg = "";
		}

		public Logout(): void
		{
			this.scope.serverErrorMsg = "";
			this.accountService.Logout((result: any) => { if (!result.Success) this.scope.serverErrorMsg = result.ErrorMessage; });
		}
	}
}