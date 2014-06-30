module Application.Controllers
{
	import Services = Application.Services;

	export class LoginController extends BaseController
	{
		constructor($scope: ng.IScope, $rootScope: ng.IScope, accountService: Services.IAccountService)
		{
			super($scope, $rootScope, accountService);
			this.scope.formData = { emailOrUserName: "", password: "", rememberMe: false };
			this.scope.isSubmitting = false;
			this.scope.Submit = () => this.Submit();
			this.scope.serverErrorMsg = "";
		}

		public Submit(): void
		{
			this.scope.serverErrorMsg = "";
			this.scope.isSubmitting = true;
			if (this.scope.loginForm.$valid)
			{
				this.accountService.Login(this.scope.formData, (result: any) =>
				{
					this.scope.isSubmitting = false;
					if (result.Success)
					{
						this.scope.isSubmitted = true;
					}
					else
					{
						this.scope.serverErrorMsg = result.error_description;
						this.scope.loginForm.password.blurred = false;
						this.scope.formData.password = "";
					}
				});
			}
			else
			{
				this.scope.isSubmitting = false;
			}
		}
	}
}