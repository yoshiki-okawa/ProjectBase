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
//# sourceMappingURL=BaseController.js.map
