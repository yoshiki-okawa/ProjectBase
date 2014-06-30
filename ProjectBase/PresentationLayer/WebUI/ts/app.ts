var appModule = angular.module("myApp", ["ngRoute", "ngCookies"]);

appModule.factory("accountService", ["$rootScope", "$http", "$location",
	($rootScope : ng.IScope, $http: ng.IHttpService, $location: ng.ILocationService)
		=> new Application.Services.AccountService($rootScope, $http, $location)]);

appModule.controller("homeController", ["$scope", "$rootScope", "$cookies", "accountService",
	($scope: ng.IScope, $rootScope: ng.IScope, $cookies: ng.cookies.ICookiesService, accountService: Application.Services.IAccountService)
		=> new Application.Controllers.HomeController($scope, $rootScope, $cookies, accountService)]);

appModule.controller("loginController", ["$scope", "$rootScope", "accountService",
	($scope: ng.IScope, $rootScope: ng.IScope, accountService: Application.Services.IAccountService)
		=> new Application.Controllers.LoginController($scope, $rootScope, accountService)]);

appModule.controller("logoutController", ["$scope", "$rootScope", "accountService",
	($scope: ng.IScope, $rootScope: ng.IScope, accountService: Application.Services.IAccountService)
		=> new Application.Controllers.LogoutController($scope, $rootScope, accountService)]);

appModule.controller("signupController", ["$scope", "$rootScope", "accountService", '$routeParams',
	($scope: ng.IScope, $rootScope: ng.IScope, accountService: Application.Services.IAccountService, $routeParams: ng.route.IRouteParamsService)
		=> new Application.Controllers.SignupController($scope, $rootScope, accountService, $routeParams)]);

appModule.controller("resetPasswordController", ["$scope", "$rootScope", "accountService", '$routeParams',
	($scope: ng.IScope, $rootScope: ng.IScope, accountService: Application.Services.IAccountService, $routeParams: ng.route.IRouteParamsService)
		=> new Application.Controllers.ResetPasswordController($scope, $rootScope, accountService, $routeParams)]);

appModule.controller("accountController", ["$scope", "$rootScope", "accountService",
	($scope: ng.IScope, $rootScope: ng.IScope, accountService: Application.Services.IAccountService)
		=> new Application.Controllers.AccountController($scope, $rootScope, accountService)]);

appModule.config(["$routeProvider", "$locationProvider",
	($routeProvider: ng.route.IRouteProvider, $locationProvider: ng.ILocationProvider) =>
	{
		$routeProvider
			.when("/", {
				templateUrl: "Views/home.html",
				controller: "homeController"
			})
			.when("/login", {
				templateUrl: "Views/login.html",
				controller: "loginController"
			})
			.when("/logout", {
				templateUrl: "Views/logout.html",
				controller: "logoutController"
			})
			.when("/signup", {
				templateUrl: "Views/signup.html",
				controller: "signupController"
			})
			.when("/verify/:id/:verificationCode", {
				templateUrl: "Views/verify.html",
				controller: "signupController"
			})
			.when("/resetPassword/:id?/:verificationCode*?", {
				templateUrl: "Views/resetPassword.html",
				controller: "resetPasswordController"
			})
			.when("/account", {
				templateUrl: "Views/account.html",
				controller: "accountController"
			})
			.otherwise({
				redirectTo: "/"
			});
	}
]);

appModule.run(["$http", "$cookies",
	($http: ng.IHttpService, $cookies: ng.cookies.ICookiesService) =>
	{
		var token: string = $cookies["access_token"];
		if (token && token.length > 0)
			$http.defaults.headers.common.Authorization = "Bearer " + token;
	}]);
