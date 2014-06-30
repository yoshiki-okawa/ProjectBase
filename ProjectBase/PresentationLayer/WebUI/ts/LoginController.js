var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
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
//# sourceMappingURL=LoginController.js.map
