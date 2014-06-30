module Application.Controllers
{
	import Services = Application.Services;

	export class SignupController extends BaseController
	{
		routeParams: any;

		constructor($scope: ng.IScope, $rootScope: ng.IScope, accountService: Services.IAccountService, $routeParams: ng.route.IRouteParamsService)
		{
			super($scope, $rootScope, accountService);

			this.routeParams = $routeParams;
			this.scope.formData = { userName: "", email: "", password: "", rememberMe: false };
			this.scope.isSubmitting = false;
			this.scope.Submit = () => this.Submit();
			this.scope.Verify = () => this.Verify();
			this.scope.OnUserNameBlur = (elem: any) => this.OnUserNameBlur(elem);
			this.scope.OnEmailBlur = (elem: any) => this.OnEmailBlur(elem);
			this.scope.OnUserNameFocus = (elem: any) => this.OnUserNameFocus(elem);
			this.scope.OnEmailFocus = (elem: any) => this.OnEmailFocus(elem);
			this.scope.userNameCheck = { isCheckPending: true, isChecking: false };
			this.scope.emailCheck = { isCheckPending: true, isChecking: false };
			this.scope.isVerified = false;
			this.scope.verifyErrorMsg = "";
			this.scope.serverErrorMsg = "";
		}

		public Submit(): void
		{
			this.scope.serverErrorMsg = "";
			this.scope.isSubmitting = true;
			if (this.scope.signupForm.$valid && !this.scope.userNameCheck.isChecking && !this.scope.emailCheck.isChecking)
			{
				this.accountService.Signup(this.scope.formData, (result: any) =>
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

		public Verify(): void
		{
			this.scope.verifyErrorMsg = "";
			this.accountService.Verify(this.routeParams, (result: any) =>
			{
				if (result.Success)
					this.scope.isVerified = false;
				else
					this.scope.verifyErrorMsg = result.ErrorMessage;
			});
		}

		public OnUserNameBlur(elem: any): void
		{
			this.OnBlur(elem);
			this.OnUserNameOrEmailBlur(elem, this.scope.userNameCheck, "userNameUniqueness", { UserName: this.scope.formData.userName });
		}

		public OnEmailBlur(elem: any): void
		{
			this.OnBlur(elem);
			this.OnUserNameOrEmailBlur(elem, this.scope.emailCheck, "emailUniqueness", { Email: this.scope.formData.email });
		}

		public OnUserNameOrEmailBlur(elem: any, check: any, validityKey: string, json: any): void
		{
			if (elem.$valid && !elem.modelValueChanged && check.isCheckPending)
			{
				// If model value is NOT changed and if check is still pending, then show error without making web request again.
				elem.$setValidity(validityKey, false);
			}
			else if (elem.$valid && elem.modelValueChanged)
			{
				// Whenever model value is changed, check user.
				check.isChecking = true;
				this.accountService.CheckUser(json, (result: any) =>
				{
					if (result.Success)
					{
						check.isCheckPending = false;
						elem.$setValidity(validityKey, true);
					}
					else
					{
						elem.$setValidity(validityKey, false);
					}
					check.isChecking = false;
				});
			}
		}

		public OnPasswordBlur(elem: any): void
		{
			this.OnBlur(elem);

			if (elem.$valid && elem.$modelValue.Length > 0 && (elem.$modelValue == this.scope.formData.userName || elem.$modelValue == this.scope.formData.email))
				elem.$setValidity("notSameAsUserNameOrEmail", false);
		}

		public OnUserNameFocus(elem: any): void
		{
			this.OnFocus(elem);

			elem.$setValidity("userNameUniqueness", true);
		}

		public OnEmailFocus(elem: any): void
		{
			this.OnFocus(elem);

			elem.$setValidity("emailUniqueness", true);
		}

		public OnPasswordFocus(elem: any): void
		{
			this.OnFocus(elem);

			elem.$setValidity("notSameAsUserNameOrEmail", true);
		}
	}
}