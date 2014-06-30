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
//# sourceMappingURL=AccountController.js.map
