var appModule = angular.module("myApp", ["ngRoute", "ngCookies"]);

appModule.factory("accountService", [
    "$rootScope", "$http", "$location",
    function ($rootScope, $http, $location) {
        return new Application.Services.AccountService($rootScope, $http, $location);
    }]);

appModule.controller("homeController", [
    "$scope", "$rootScope", "$cookies", "accountService",
    function ($scope, $rootScope, $cookies, accountService) {
        return new Application.Controllers.HomeController($scope, $rootScope, $cookies, accountService);
    }]);

appModule.controller("loginController", [
    "$scope", "$rootScope", "accountService",
    function ($scope, $rootScope, accountService) {
        return new Application.Controllers.LoginController($scope, $rootScope, accountService);
    }]);

appModule.controller("logoutController", [
    "$scope", "$rootScope", "accountService",
    function ($scope, $rootScope, accountService) {
        return new Application.Controllers.LogoutController($scope, $rootScope, accountService);
    }]);

appModule.controller("signupController", [
    "$scope", "$rootScope", "accountService", '$routeParams',
    function ($scope, $rootScope, accountService, $routeParams) {
        return new Application.Controllers.SignupController($scope, $rootScope, accountService, $routeParams);
    }]);

appModule.controller("resetPasswordController", [
    "$scope", "$rootScope", "accountService", '$routeParams',
    function ($scope, $rootScope, accountService, $routeParams) {
        return new Application.Controllers.ResetPasswordController($scope, $rootScope, accountService, $routeParams);
    }]);

appModule.controller("accountController", [
    "$scope", "$rootScope", "accountService",
    function ($scope, $rootScope, accountService) {
        return new Application.Controllers.AccountController($scope, $rootScope, accountService);
    }]);

appModule.config([
    "$routeProvider", "$locationProvider",
    function ($routeProvider, $locationProvider) {
        $routeProvider.when("/", {
            templateUrl: "Views/home.html",
            controller: "homeController"
        }).when("/login", {
            templateUrl: "Views/login.html",
            controller: "loginController"
        }).when("/logout", {
            templateUrl: "Views/logout.html",
            controller: "logoutController"
        }).when("/signup", {
            templateUrl: "Views/signup.html",
            controller: "signupController"
        }).when("/verify/:id/:verificationCode", {
            templateUrl: "Views/verify.html",
            controller: "signupController"
        }).when("/resetPassword/:id?/:verificationCode*?", {
            templateUrl: "Views/resetPassword.html",
            controller: "resetPasswordController"
        }).when("/account", {
            templateUrl: "Views/account.html",
            controller: "accountController"
        }).otherwise({
            redirectTo: "/"
        });
    }
]);

appModule.run([
    "$http", "$cookies",
    function ($http, $cookies) {
        var token = $cookies["access_token"];
        if (token && token.length > 0)
            $http.defaults.headers.common.Authorization = "Bearer " + token;
    }]);
var Application;
(function (Application) {
    (function (Controllers) {
        var BaseController = (function () {
            function BaseController($scope, $rootScope, accountService) {
                var _this = this;
                this.scope = $scope;
                this.rootScope = $rootScope;
                this.accountService = accountService;
                this.scope.AutoLogin = function () {
                    return _this.AutoLogin();
                };
                this.scope.OnBlur = function (elem) {
                    return _this.OnBlur(elem);
                };
                this.scope.OnFocus = function (elem) {
                    return _this.OnFocus(elem);
                };
            }
            BaseController.prototype.AutoLogin = function (callBack) {
                var _this = this;
                if (!this.rootScope.isAutoLoginTried && !this.rootScope.isAutoLoggingIn) {
                    this.rootScope.isAutoLoggingIn = true;
                    this.accountService.Login({ email: "", password: "", rememberMe: true }, function (result) {
                        _this.rootScope.isAutoLoginTried = true;
                        _this.rootScope.isAutoLoggingIn = false;
                        if (callBack)
                            callBack(result);
                    });
                }
            };

            BaseController.prototype.OnBlur = function (elem) {
                elem.blurred = true;
                elem.modelValueChanged = elem.lastModelValue !== elem.$modelValue;
                elem.lastModelValue = elem.$modelValue;
            };

            BaseController.prototype.OnFocus = function (elem) {
                elem.blurred = false;
            };
            return BaseController;
        })();
        Controllers.BaseController = BaseController;
    })(Application.Controllers || (Application.Controllers = {}));
    var Controllers = Application.Controllers;
})(Application || (Application = {}));
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Application;
(function (Application) {
    (function (Controllers) {
        var AccountController = (function (_super) {
            __extends(AccountController, _super);
            function AccountController($scope, $rootScope, accountService) {
                var _this = this;
                _super.call(this, $scope, $rootScope, accountService);
                this.scope.LoadUserDetails = function () {
                    return _this.LoadUserDetails();
                };
                this.scope.userName = "";
                this.scope.email = "";
            }
            AccountController.prototype.LoadUserDetails = function () {
                var _this = this;
                if (!this.rootScope.isLoggedIn)
                    this.AutoLogin(function () {
                        return _this.LoadUserDetailsInternal();
                    });
                else
                    this.LoadUserDetailsInternal();
            };
            AccountController.prototype.LoadUserDetailsInternal = function () {
                var _this = this;
                if (this.rootScope.isLoggedIn) {
                    this.accountService.LoadUserDetails(function (result) {
                        _this.scope.userName = result.UserName;
                        _this.scope.email = result.Email;
                    });
                }
            };
            return AccountController;
        })(Controllers.BaseController);
        Controllers.AccountController = AccountController;
    })(Application.Controllers || (Application.Controllers = {}));
    var Controllers = Application.Controllers;
})(Application || (Application = {}));
var Application;
(function (Application) {
    (function (Controllers) {
        var HomeController = (function (_super) {
            __extends(HomeController, _super);
            function HomeController($scope, $rootScope, $cookies, accountService) {
                var _this = this;
                _super.call(this, $scope, $rootScope, accountService);
                this.scope.JoinGroup = function (group) {
                    return _this.JoinGroup(group);
                };
                this.scope.BroadcastToGroup = function () {
                    return _this.BroadcastToGroup();
                };
                this.scope.LeaveGroup = function () {
                    return _this.LeaveGroup();
                };
                this.scope.messages = [];
                this.scope.formData = { joinGroupForm: {}, broadcastForm: {} };
                this.scope.groups = {};
                this.scope.group = "";
                this.scope.serverErrorMsgs = [];
                if (!this.rootScope.hubInitialized) {
                    this.rootScope.hubInitialized = false;
                    $.signalR.chatHub.client.addMessage = function (msg) {
                        _this.scope.messages.push({ tick: new Date().valueOf(), dateStr: new Date().toLocaleString(), name: msg.Name, msg: msg.Message });
                        _this.scope.$apply();
                    };
                    $.signalR.chatHub.client.addGroup = function (group) {
                        if (group.Count > 0)
                            _this.scope.groups[group.Name] = group.Count;
                        else
                            delete _this.scope.groups[group.Name];
                        _this.scope.$apply();
                    };
                    $.signalR.chatHub.client.addGroups = function (groups) {
                        for (var i = 0; i < groups.length; i++)
                            _this.scope.groups[groups[i].Name] = groups[i].Count;
                        _this.scope.$apply();
                    };

                    var token = $cookies["access_token"];
                    if (token && token.length > 0)
                        $.signalR.hub.qs = { "Bearer": token };
                    $.signalR.hub.start().done(function () {
                        _this.rootScope.hubInitialized = true;
                        _this.rootScope.$apply();
                    });
                }
            }
            HomeController.prototype.JoinGroup = function (group) {
                var _this = this;
                if (!group)
                    group = this.scope.formData.joinGroupForm.group;

                var args = { Group: group };
                $.signalR.chatHub.server.joinGroup(args).done(function (error) {
                    if (error !== undefined && error.length > 0)
                        _this.scope.serverErrorMsgs = error;
                    else
                        _this.scope.group = group;
                    _this.scope.$apply();
                });
            };

            HomeController.prototype.BroadcastToGroup = function () {
                var args = { Group: this.scope.group, Message: this.scope.formData.broadcastForm.msg };
                $.signalR.chatHub.server.broadcastToGroup(args);
                this.scope.formData.broadcastForm.msg = "";
            };

            HomeController.prototype.LeaveGroup = function () {
                var _this = this;
                $.signalR.chatHub.server.leaveGroup().done(function () {
                    _this.scope.group = "";
                    while (_this.scope.messages.length > 0)
                        _this.scope.messages.pop();
                    _this.scope.$apply();
                });
            };
            return HomeController;
        })(Controllers.BaseController);
        Controllers.HomeController = HomeController;
    })(Application.Controllers || (Application.Controllers = {}));
    var Controllers = Application.Controllers;
})(Application || (Application = {}));
var Application;
(function (Application) {
    (function (Controllers) {
        var ResetPasswordController = (function (_super) {
            __extends(ResetPasswordController, _super);
            function ResetPasswordController($scope, $rootScope, accountService, $routeParams) {
                var _this = this;
                _super.call(this, $scope, $rootScope, accountService);
                this.routeParams = $routeParams;

                if (this.routeParams.id) {
                    this.scope.isRequestPasswordReset = false;
                    this.scope.formData = { password: "", id: this.routeParams.id, verificationCode: this.routeParams.verificationCode };
                } else {
                    this.scope.isRequestPasswordReset = true;
                    this.scope.formData = { emailOrUserName: "" };
                }
                this.scope.isSubmitting = false;
                this.scope.Submit = function () {
                    return _this.Submit();
                };
                this.scope.serverErrorMsg = "";
            }
            ResetPasswordController.prototype.Submit = function () {
                var _this = this;
                this.scope.serverErrorMsg = "";
                this.scope.isSubmitting = true;
                if (this.scope.resetPasswordForm.$valid) {
                    var func = this.scope.isRequestPasswordReset ? function (formData, callback) {
                        return _this.accountService.RequestPasswordReset(formData, callback);
                    } : function (formData, callback) {
                        return _this.accountService.ResetPassword(formData, callback);
                    };

                    func(this.scope.formData, function (result) {
                        _this.scope.isSubmitting = false;
                        if (result.Success)
                            _this.scope.isSubmitted = true;
                        else
                            _this.scope.serverErrorMsg = result.ErrorMessage;
                    });
                } else {
                    this.scope.isSubmitting = false;
                }
            };
            return ResetPasswordController;
        })(Controllers.BaseController);
        Controllers.ResetPasswordController = ResetPasswordController;
    })(Application.Controllers || (Application.Controllers = {}));
    var Controllers = Application.Controllers;
})(Application || (Application = {}));
var Application;
(function (Application) {
    (function (Controllers) {
        var SignupController = (function (_super) {
            __extends(SignupController, _super);
            function SignupController($scope, $rootScope, accountService, $routeParams) {
                var _this = this;
                _super.call(this, $scope, $rootScope, accountService);

                this.routeParams = $routeParams;
                this.scope.formData = { userName: "", email: "", password: "", rememberMe: false };
                this.scope.isSubmitting = false;
                this.scope.Submit = function () {
                    return _this.Submit();
                };
                this.scope.Verify = function () {
                    return _this.Verify();
                };
                this.scope.OnUserNameBlur = function (elem) {
                    return _this.OnUserNameBlur(elem);
                };
                this.scope.OnEmailBlur = function (elem) {
                    return _this.OnEmailBlur(elem);
                };
                this.scope.OnUserNameFocus = function (elem) {
                    return _this.OnUserNameFocus(elem);
                };
                this.scope.OnEmailFocus = function (elem) {
                    return _this.OnEmailFocus(elem);
                };
                this.scope.userNameCheck = { isCheckPending: true, isChecking: false };
                this.scope.emailCheck = { isCheckPending: true, isChecking: false };
                this.scope.isVerified = false;
                this.scope.verifyErrorMsg = "";
                this.scope.serverErrorMsg = "";
            }
            SignupController.prototype.Submit = function () {
                var _this = this;
                this.scope.serverErrorMsg = "";
                this.scope.isSubmitting = true;
                if (this.scope.signupForm.$valid && !this.scope.userNameCheck.isChecking && !this.scope.emailCheck.isChecking) {
                    this.accountService.Signup(this.scope.formData, function (result) {
                        _this.scope.isSubmitting = false;
                        if (result.Success)
                            _this.scope.isSubmitted = true;
                        else
                            _this.scope.serverErrorMsg = result.ErrorMessage;
                    });
                } else {
                    this.scope.isSubmitting = false;
                }
            };

            SignupController.prototype.Verify = function () {
                var _this = this;
                this.scope.verifyErrorMsg = "";
                this.accountService.Verify(this.routeParams, function (result) {
                    if (result.Success)
                        _this.scope.isVerified = false;
                    else
                        _this.scope.verifyErrorMsg = result.ErrorMessage;
                });
            };

            SignupController.prototype.OnUserNameBlur = function (elem) {
                this.OnBlur(elem);
                this.OnUserNameOrEmailBlur(elem, this.scope.userNameCheck, "userNameUniqueness", { UserName: this.scope.formData.userName });
            };

            SignupController.prototype.OnEmailBlur = function (elem) {
                this.OnBlur(elem);
                this.OnUserNameOrEmailBlur(elem, this.scope.emailCheck, "emailUniqueness", { Email: this.scope.formData.email });
            };

            SignupController.prototype.OnUserNameOrEmailBlur = function (elem, check, validityKey, json) {
                if (elem.$valid && !elem.modelValueChanged && check.isCheckPending) {
                    // If model value is NOT changed and if check is still pending, then show error without making web request again.
                    elem.$setValidity(validityKey, false);
                } else if (elem.$valid && elem.modelValueChanged) {
                    // Whenever model value is changed, check user.
                    check.isChecking = true;
                    this.accountService.CheckUser(json, function (result) {
                        if (result.Success) {
                            check.isCheckPending = false;
                            elem.$setValidity(validityKey, true);
                        } else {
                            elem.$setValidity(validityKey, false);
                        }
                        check.isChecking = false;
                    });
                }
            };

            SignupController.prototype.OnPasswordBlur = function (elem) {
                this.OnBlur(elem);

                if (elem.$valid && elem.$modelValue.Length > 0 && (elem.$modelValue == this.scope.formData.userName || elem.$modelValue == this.scope.formData.email))
                    elem.$setValidity("notSameAsUserNameOrEmail", false);
            };

            SignupController.prototype.OnUserNameFocus = function (elem) {
                this.OnFocus(elem);

                elem.$setValidity("userNameUniqueness", true);
            };

            SignupController.prototype.OnEmailFocus = function (elem) {
                this.OnFocus(elem);

                elem.$setValidity("emailUniqueness", true);
            };

            SignupController.prototype.OnPasswordFocus = function (elem) {
                this.OnFocus(elem);

                elem.$setValidity("notSameAsUserNameOrEmail", true);
            };
            return SignupController;
        })(Controllers.BaseController);
        Controllers.SignupController = SignupController;
    })(Application.Controllers || (Application.Controllers = {}));
    var Controllers = Application.Controllers;
})(Application || (Application = {}));
var Application;
(function (Application) {
    (function (Controllers) {
        var LogoutController = (function (_super) {
            __extends(LogoutController, _super);
            function LogoutController($scope, $rootScope, accountService) {
                var _this = this;
                _super.call(this, $scope, $rootScope, accountService);
                this.scope.Logout = function () {
                    return _this.Logout();
                };
                this.scope.serverErrorMsg = "";
            }
            LogoutController.prototype.Logout = function () {
                var _this = this;
                this.scope.serverErrorMsg = "";
                this.accountService.Logout(function (result) {
                    if (!result.Success)
                        _this.scope.serverErrorMsg = result.ErrorMessage;
                });
            };
            return LogoutController;
        })(Controllers.BaseController);
        Controllers.LogoutController = LogoutController;
    })(Application.Controllers || (Application.Controllers = {}));
    var Controllers = Application.Controllers;
})(Application || (Application = {}));
var Application;
(function (Application) {
    (function (Controllers) {
        var LoginController = (function (_super) {
            __extends(LoginController, _super);
            function LoginController($scope, $rootScope, accountService) {
                var _this = this;
                _super.call(this, $scope, $rootScope, accountService);
                this.scope.formData = { emailOrUserName: "", password: "", rememberMe: false };
                this.scope.isSubmitting = false;
                this.scope.Submit = function () {
                    return _this.Submit();
                };
                this.scope.serverErrorMsg = "";
            }
            LoginController.prototype.Submit = function () {
                var _this = this;
                this.scope.serverErrorMsg = "";
                this.scope.isSubmitting = true;
                if (this.scope.loginForm.$valid) {
                    this.accountService.Login(this.scope.formData, function (result) {
                        _this.scope.isSubmitting = false;
                        if (result.Success) {
                            _this.scope.isSubmitted = true;
                        } else {
                            _this.scope.serverErrorMsg = result.error_description;
                            _this.scope.loginForm.password.blurred = false;
                            _this.scope.formData.password = "";
                        }
                    });
                } else {
                    this.scope.isSubmitting = false;
                }
            };
            return LoginController;
        })(Controllers.BaseController);
        Controllers.LoginController = LoginController;
    })(Application.Controllers || (Application.Controllers = {}));
    var Controllers = Application.Controllers;
})(Application || (Application = {}));
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
//# sourceMappingURL=ts.js.map
