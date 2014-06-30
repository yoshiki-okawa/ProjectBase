var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
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
//# sourceMappingURL=LogoutController.js.map
