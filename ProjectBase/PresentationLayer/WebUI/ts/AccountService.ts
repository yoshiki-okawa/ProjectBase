module Application.Services
{
	export interface IAccountService
	{
		Login(formData: any, callback: Function);
		Logout(callback: Function);
		Signup(formData: any, callback: Function);
		Verify(routePrms: any, callback: Function);
		CheckUser(json: any, callback: Function);
		RequestPasswordReset(formData: any, callback: Function);
		ResetPassword(formData: any, callback: Function);
		LoadUserDetails(callback: Function);
	}

	export class AccountService implements IAccountService
	{
		rootScope: any;
		http: ng.IHttpService;
		location: ng.ILocationService;

		constructor($rootScope: ng.IScope, $http: ng.IHttpService, $location: ng.ILocationService)
		{
			this.rootScope = $rootScope;
			this.rootScope.isLoggedIn = false;
			this.http = $http;
			this.location = $location;
		}

		Login(formData: any, callback: Function) : void
		{
			this.http({
				method: "POST",
				url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/token",
				data: $.param({ grant_type: "password", username: formData.emailOrUserName, password: formData.password, rememberMe: formData.rememberMe }),
				headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
			}).success((data : any, status) =>
			{
				if (data.access_token === "dummy token")
				{
					this.rootScope.isLoggedIn = true;
					this.rootScope.userName = data.user_name;
					if (this.location.path() == "/login")
						this.location.path("/");
				}
				callback(data);
			}).error(error =>
			{
				callback(error);
			});
		}

		Logout(callback: Function): void
		{
			this.http({
				method: "POST",
				url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/logout"
			}).success((data, status) =>
			{
				this.rootScope.isLoggedIn = false;
				callback(data);
				this.location.path("/");
			}).error(error =>
			{
				callback(error);
			});
		}

		Signup(formData: any, callback: Function): void
		{
			this.http({
				method: "POST",
				url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/register",
				data: JSON.stringify({ UserName: formData.userName, Email: formData.email, Password: formData.password })
			}).success((data, status) =>
			{
				callback(data);
			}).error(error =>
			{
				callback(error);
			});
		}

		Verify(routePrms: any, callback: Function): void
		{
			this.http({
				method: "POST",
				url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/verify",
				data: JSON.stringify({ Id: routePrms.id, VerificationCode: routePrms.verificationCode })
			}).success((data, status) =>
			{
				callback(data);
			}).error(error =>
			{
				callback(error);
			});
		}

		CheckUser(json: any, callback: Function): void
		{
			this.http({
				method: "POST",
				url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/checkUser",
				data: JSON.stringify(json)
			}).success((data, status) =>
			{
				callback(data);
			}).error(error =>
			{
				callback(error);
			});
		}

		RequestPasswordReset(formData: any, callback: Function): void
		{
			this.http({
				method: "POST",
				url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/requestPasswordReset",
				data: JSON.stringify({ EmailOrUserName: formData.emailOrUserName })
			}).success((data, status) =>
			{
				callback(data);
			}).error(error =>
			{
				callback(error);
			});
		}

		ResetPassword(formData: any, callback: Function): void
		{
			this.http({
				method: "POST",
				url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/resetPassword",
				data: JSON.stringify({ Password: formData.password, Id: formData.id, VerificationCode: formData.verificationCode })
			}).success((data, status) =>
			{
				callback(data);
			}).error(error =>
			{
				callback(error);
			});
		}

		LoadUserDetails(callback: Function): void
		{
			this.http({
				method: "POST",
				url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/loadUserDetails",
			}).success((data: any, status) =>
			{
				callback(data);
			}).error(error =>
			{
				callback(error);
			});
		}
	}
}