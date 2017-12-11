(function() {
    "use strict";

    var app = angular.module("admin", []);

    function EditRolesPermissionsMappingController($http, $scope, $timeout) {
        this.$http = $http;
        this.$scope = $scope;
        this.$timeout = $timeout;

        this.init();
    }

    EditRolesPermissionsMappingController.$inject = ["$http", "$scope", "$timeout"];

    EditRolesPermissionsMappingController.prototype = {
        init: function () {
            var that = this;

            this.getData(function () {
                that.rolesPermissionsMapping = that.getRolesPermissionsMapping();
                that.$timeout(function() {
                    that.$scope.onLoad();
                });
            });
        },

        getData: function (callback) {
            var that = this;

            var request = this.$http({
                method: "get",
                url: $("#EditRolesPermissionsMappingUrl").val(),
            });

            request.then(function (promise) {
                that.data = promise.data;
                callback();
            });
        },

        getRolesPermissionsMapping: function() {
            var RolesPermissionsMapping = {};

            for (var i = 0; i < this.data.Roles.length; i++) {
                var role = this.data.Roles[i];
                var cell = {};
                for (var j = 0; j < this.data.Permissions.length; j++) {
                    var permission = this.data.Permissions[j];
                    cell[permission.Id] = roleHasPermission(role, permission);
                }
                RolesPermissionsMapping[role.Id] = cell;
            }

            function roleHasPermission(role, permission) {
                return role.PermissionIds.indexOf(permission.Id) > -1;
            }

            return RolesPermissionsMapping;
        },

        getDataForSubmit: function () {
            var roles = [];
            for (var i = 0; i < this.data.Roles.length; i++) {
                var role = {
                    Id: this.data.Roles[i].Id,
                    Name: this.data.Roles[i].Name,
                    PermissionIds: []
                };

                for (var key in this.rolesPermissionsMapping[role.Id]) {
                    if (this.rolesPermissionsMapping[role.Id][key]) {
                        role.PermissionIds.push(key);
                    }
                }

                roles.push(role);
            }
            return roles;
        },

        submit: function () {
            this.$http.post($("#EditRolesPermissionsMappingUrl").val(), this.getDataForSubmit());
        }
    };

    app.controller("EditRolesPermissionsMappingController", EditRolesPermissionsMappingController);

}());