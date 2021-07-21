function UmbNav($scope, editorService, umbnavResource, $routeParams) {
    var vm = this;
    var dialogOptions = $scope.model;
    vm.wideMode = Object.toBoolean(dialogOptions.config.hideLabel);
    vm.items = [];

    if (dialogOptions.config.expandOnHoverTimeout > 0) {
        vm.expandOnHover = dialogOptions.config.expandOnHoverTimeout;
    } else {
        vm.expandOnHover = dialogOptions.config.expandOnHover;
    }

    if (!_.isEmpty($scope.model.value)) {
        // retreive the saved items
        vm.items = $scope.model.value;

        // get updated entities for content
        getItemEntities(vm.items);
    }

    $scope.add = function () {
        openSettings(null, function (navItem) {
            // add item to scope
            vm.items.push(buildNavItem(navItem));
        });
    };

    $scope.edit = function (item) {
        openSettings(item, function (navItem) {
            // update item in scope
            // assign new values via extend to maintain refs
            angular.extend(item, buildNavItem(navItem));
        });
    };

    $scope.collapseAll = function () {
        $scope.$broadcast('angular-ui-tree:collapse-all');
    };

    $scope.expandAll = function () {
        $scope.$broadcast('angular-ui-tree:expand-all');
    };

    $scope.showButtons = function () {
        const maxDepth = dialogOptions.config.maxDepth;
        if (maxDepth === 0 || maxDepth > 1) {
            return true;
        }

        return false;
    };

    $scope.remove = function (item) {
        item.remove();
    };

    $scope.$on("formSubmitting", function (ev, args) {
        $scope.model.value = vm.items;
    });

    $scope.treeOptions = {
        toggle: function (collapsed, sourceNodeScope) {
            sourceNodeScope.$modelValue.collapsed = collapsed;
        }
    }

    function getItemEntities(items) {
        _.each(items, function (item) {
            item.description = item.url;
            if (item.udi) {
                umbnavResource.getById(item.udi, $routeParams.cculture ? $routeParams.cculture : $routeParams.mculture)
                    .then(function (response) {
                        angular.extend(item, response.data);
                    }
                    );

                if (item.children.length > 0) {
                    getItemEntities(item.children);
                }
            }
        });
    }

    function openSettings(item, callback) {

        var settingsEditor = {
            title: "Settings",
            view: "/App_Plugins/UmbNav/Views/settings.html",
            size: "small",
            hideNoreferrer: dialogOptions.config.hideNoreferrer,
            hideNoopener: dialogOptions.config.hideNoopener,
            allowDisplay: dialogOptions.config.allowDisplay,
            currentTarget: item,
            submit: function (model) {
                model.target.description = model.target.url + model.target.anchor;
                if (model.target.anchor && model.target.anchor[0] !== '?' && model.target.anchor[0] !== '#') {
                    model.target.anchor = (model.target.anchor.indexOf('=') === -1 ? '#' : '?') + model.target.anchor;
                }
                if (model.target.udi) {
                    umbnavResource.getById(model.target.udi, $routeParams.cculture ? $routeParams.cculture : $routeParams.mculture)
                        .then(function (response) {
                            // merge metadata
                            angular.extend(model.target, response.data);

                            callback(model.target);
                        }
                        );
                }
                else {
                    callback(model.target);
                }

                editorService.close();
            },
            close: function () {
                editorService.close();
            }
        };

        editorService.open(settingsEditor);
    }

    function buildNavItem(data) {
        var url;
        if (data.anchor) {
            url = data.url + data.anchor;
        } else {
            url = data.url;
        }

        return {
            id: data.id,
            udi: data.udi,
            name: data.name,
            description: url,
            collapsed: data.collapsed,
            title: data.title,
            target: data.target,
            noopener: data.noopener,
            noreferrer: data.noreferrer,
            hideLoggedIn: data.hideLoggedIn,
            hideLoggedOut: data.hideLoggedOut,
            anchor: data.anchor,
            url: url || "#",
            children: data.children || [],
            icon: data.icon || "icon-link",
            published: data.published,
            naviHide: data.naviHide,
            culture: data.culture
        }
    }
}

angular.module("umbraco").controller("AaronSadler.UmbNav.Controller", UmbNav);

app.requires.push("ui.tree");