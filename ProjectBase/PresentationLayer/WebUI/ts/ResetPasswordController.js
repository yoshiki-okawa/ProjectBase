var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
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
//# sourceMappingURL=ResetPasswordController.js.map
