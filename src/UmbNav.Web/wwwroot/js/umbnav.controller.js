function UmbNav($scope, editorService, umbnavResource, $routeParams) {
    var vm = this;
    var dialogOptions = $scope.model;
    vm.wideMode = Object.toBoolean(dialogOptions.config.hideLabel);
    vm.allowChildNodes = Object.toBoolean(!dialogOptions.config.hideIncludeChildren);
    vm.showTopAddButton = Object.toBoolean(dialogOptions.config.showTopAddButton);
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


    vm.toggleVar = vm.items.some((element) => element.collapsed === false);
    $scope.toggleCollapse = function () {
        vm.toggleVar = !vm.toggleVar;
        if (!vm.toggleVar) {
            $scope.$broadcast('angular-ui-tree:collapse-all');
        } else {
            $scope.$broadcast('angular-ui-tree:expand-all');
        }
        angular.forEach(vm.items,
            function (value, key) {
                toggleChildren(value, !vm.toggleVar);
            });
    }

    function toggleChildren(item, b) {
        item.collapsed = b;
        if (item.children) {
            angular.forEach(item.children,
                function (value, key) {
                    toggleChildren(value, b);
                });
        }
    }

    $scope.showExpandCollapseButtons = function () {
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
            if (item.udi) {
                umbnavResource.getById(item.udi, $routeParams.cculture ? $routeParams.cculture : $routeParams.mculture)
                    .then(function (response) {
                        angular.extend(item, response.data);
                        item.description = item.url;
                    }
                    );

                if (item.children.length > 0) {
                    getItemEntities(item.children);
                }
            }
        });
    }

    function openSettings(item, callback) {

        if (item != null && item.itemType == null) {
            item.itemType = 'link';
        }

        var settingsEditor = {
            title: "Settings",
            view: "/App_Plugins/UmbNav/views/settings.html",
            size: "small",
            hideNoreferrer: dialogOptions.config.hideNoreferrer,
            hideNoopener: dialogOptions.config.hideNoopener,
            allowDisplay: dialogOptions.config.allowDisplay,
            allowCustomClasses: dialogOptions.config.allowCustomClasses,
            allowImageIcon: dialogOptions.config.allowImageIcon,
            hideIncludeChildren: dialogOptions.config.hideIncludeChildren,
            allowLabels: dialogOptions.config.allowLabels,
            allowDisplayAsLabel: dialogOptions.config.allowDisplayAsLabel,
            currentTarget: item,
            submit: function (model) {

                if (model.target.itemType === 'nolink') {
                    model.target.anchor = null;
                    model.target.url = null;
                    model.target.description = null;
                    model.target.target = null;
                    model.target.noopener = false;
                    model.target.noreferrer = false;
                    model.target.includeChildren = false;
                    model.target.udi = null;
                } else {
                    model.target.description = model.target.url + model.target.anchor;
                    if (model.target.anchor && model.target.anchor[0] !== '?' && model.target.anchor[0] !== '#') {
                        model.target.anchor = (model.target.anchor.indexOf('=') === -1 ? '#' : '?') + model.target.anchor;
                    }
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

        var icon = data.icon;
        if (data.itemType === 'nolink') {
            icon = "icon-tag";

        } else if (data.icon == null) {
            icon = "icon-link";
        }

        if (data.title == (null || "")) {
            data.title = data.name;
        }

        return {
            udi: data.udi,
            key: data.key,
            name: data.name,
            description: url,
            collapsed: data.collapsed,
            title: data.title,
            menuitemdescription: data.menuitemdescription,
            target: data.target,
            noopener: data.noopener,
            noreferrer: data.noreferrer,
            hideLoggedIn: data.hideLoggedIn,
            hideLoggedOut: data.hideLoggedOut,
            customClasses: data.customClasses,
            imageUrl: data.imageUrl,
            image: data.image,
            anchor: data.anchor,
            url: url,
            children: data.children || [],
            icon: icon,
            published: data.published,
            naviHide: data.naviHide,
            culture: data.culture,
            includeChildNodes: data.includeChildren,
            itemType: data.itemType,
            displayAsLabel: data.displayAsLabel
        }
    }
}

angular.module("umbraco").controller("Our.UmbNav.Controller", UmbNav);

app.requires.push("ui.tree");
