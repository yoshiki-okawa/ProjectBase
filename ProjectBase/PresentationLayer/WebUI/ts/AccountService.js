var Application;
(function (Application) {
    (function (Services) {
        var AccountService = (function () {
            function AccountService($rootScope, $http, $location) {
                this.rootScope = $rootScope;
                this.rootScope.isLoggedIn = false;
                this.http = $http;
                this.location = $location;
            }
            AccountService.prototype.Login = function (formData, callback) {
                var _this = this;
                this.http({
                    method: "POST",
                    url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/token",
                    data: $.param({ grant_type: "password", username: formData.emailOrUserName, password: formData.password, rememberMe: formData.rememberMe }),
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                }).success(function (data, status) {
                    if (data.access_token === "dummy token") {
                        _this.rootScope.isLoggedIn = true;
                        _this.rootScope.userName = data.user_name;
                        if (_this.location.path() == "/login")
                            _this.location.path("/");
                    }
                    callback(data);
                }).error(function (error) {
                    callback(error);
                });
            };

            AccountService.prototype.Logout = function (callback) {
                var _this = this;
                this.http({
                    method: "POST",
                    url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/logout"
                }).success(function (data, status) {
                    _this.rootScope.isLoggedIn = false;
                    callback(data);
                    _this.location.path("/");
                }).error(function (error) {
                    callback(error);
                });
            };

            AccountService.prototype.Signup = function (formData, callback) {
                this.http({
                    method: "POST",
                    url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/register",
                    data: JSON.stringify({ UserName: formData.userName, Email: formData.email, Password: formData.password })
                }).success(function (data, status) {
                    callback(data);
                }).error(function (error) {
                    callback(error);
                });
            };

            AccountService.prototype.Verify = function (routePrms, callback) {
                this.http({
                    method: "POST",
                    url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/verify",
                    data: JSON.stringify({ Id: routePrms.id, VerificationCode: routePrms.verificationCode })
                }).success(function (data, status) {
                    callback(data);
                }).error(function (error) {
                    callback(error);
                });
            };

            AccountService.prototype.CheckUser = function (json, callback) {
                this.http({
                    method: "POST",
                    url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/checkUser",
                    data: JSON.stringify(json)
                }).success(function (data, status) {
                    callback(data);
                }).error(function (error) {
                    callback(error);
                });
            };

            AccountService.prototype.RequestPasswordReset = function (formData, callback) {
                this.http({
                    method: "POST",
                    url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/requestPasswordReset",
                    data: JSON.stringify({ EmailOrUserName: formData.emailOrUserName })
                }).success(function (data, status) {
                    callback(data);
                }).error(function (error) {
                    callback(error);
                });
            };

            AccountService.prototype.ResetPassword = function (formData, callback) {
                this.http({
                    method: "POST",
                    url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/resetPassword",
                    data: JSON.stringify({ Password: formData.password, Id: formData.id, VerificationCode: formData.verificationCode })
                }).success(function (data, status) {
                    callback(data);
                }).error(function (error) {
                    callback(error);
                });
            };

            AccountService.prototype.LoadUserDetails = function (callback) {
                this.http({
                    method: "POST",
                    url: this.location.protocol() + "://" + this.location.host() + ":" + this.location.port() + "/PBService/api/account/loadUserDetails"
                }).success(function (data, status) {
                    callback(data);
                }).error(function (error) {
                    callback(error);
                });
            };
            return AccountService;
        })();
        Services.AccountService = AccountService;
    })(Application.Services || (Application.Services = {}));
    var Services = Application.Services;
})(Application || (Application = {}));
//# sourceMappingURL=AccountService.js.map
