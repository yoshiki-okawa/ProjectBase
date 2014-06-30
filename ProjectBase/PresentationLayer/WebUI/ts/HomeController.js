var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
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
//# sourceMappingURL=HomeController.js.map
