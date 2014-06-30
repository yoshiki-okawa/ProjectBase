module Application.Controllers
{
	import Services = Application.Services;

	export class ResetPasswordController extends BaseController
	{
		routeParams: any;

		constructor($scope: ng.IScope, $rootScope: ng.IScope, accountService: Services.IAccountService, $routeParams: ng.route.IRouteParamsService)
		{
			super($scope, $rootScope, accountService);
			this.routeParams = $routeParams;

			if (this.routeParams.id)
			{
				this.scope.isRequestPasswordReset = false;
				this.scope.formData = { password: "", id: this.routeParams.id, verificationCode: this.routeParams.verificationCode };
			}
			else
			{
				this.scope.isRequestPasswordReset = true;
				this.scope.formData = { emailOrUserName: "" };
			}
			this.scope.isSubmitting = false;
			this.scope.Submit = () => this.Submit();
			this.scope.serverErrorMsg = "";
		}

		public Submit(): void
		{
			this.scope.serverErrorMsg = "";
			this.scope.isSubmitting = true;
			if (this.scope.resetPasswordForm.$valid)
			{
				var func = this.scope.isRequestPasswordReset ?
					(formData: any, callback: Function) => this.accountService.RequestPasswordReset(formData, callback) :
					(formData: any, callback: Function) => this.accountService.ResetPassword(formData, callback);

				func(this.scope.formData, (result: any) =>
				{
					this.scope.isSubmitting = false;
					if (result.Success)
						this.scope.isSubmitted = true;
					else
						this.scope.serverErrorMsg = result.ErrorMessage;
				});
			}
			else
			{
				this.scope.isSubmitting = false;
			}
		}
	}
}