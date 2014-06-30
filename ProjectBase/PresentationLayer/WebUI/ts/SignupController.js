var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
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
//# sourceMappingURL=SignupController.js.map
