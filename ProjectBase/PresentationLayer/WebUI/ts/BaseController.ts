module Application.Controllers
{
	import Services = Application.Services;

	export class BaseController
	{
		scope: any;
		rootScope: any;
		accountService: Services.IAccountService;

		constructor($scope: ng.IScope, $rootScope: ng.IScope, accountService: Services.IAccountService)
		{
			this.scope = $scope;
			this.rootScope = $rootScope;
			this.accountService = accountService;
			this.scope.AutoLogin = () => this.AutoLogin();
			this.scope.OnBlur = (elem: any) => this.OnBlur(elem);
			this.scope.OnFocus = (elem: any) => this.OnFocus(elem);
		}

		public AutoLogin(callBack?: Function): void
		{
			if (!this.rootScope.isAutoLoginTried && !this.rootScope.isAutoLoggingIn)
			{
				this.rootScope.isAutoLoggingIn = true;
				this.accountService.Login({ email: "", password: "", rememberMe: true }, (result: any) => { this.rootScope.isAutoLoginTried = true; this.rootScope.isAutoLoggingIn = false; if (callBack) callBack(result); });
			}
		}

		public OnBlur(elem: any): void
		{
			elem.blurred = true;
			elem.modelValueChanged = elem.lastModelValue !== elem.$modelValue;
			elem.lastModelValue = elem.$modelValue;
		}

		public OnFocus(elem: any): void
		{
			elem.blurred = false;
		}
	}
}