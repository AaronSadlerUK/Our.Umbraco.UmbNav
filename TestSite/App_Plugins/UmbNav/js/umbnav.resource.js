angular.module("umbraco.resources").factory("umbnavResource", function ($http, iconHelper) {
    return {
        getById: function (id, culture) {

            return $http.get("backoffice/UmbNav/UmbNavEntityApi/GetById?id=" + id + "&culture=" + culture)
                .then(function (response) {
                        var item = response.data;
                        item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                        return response;
                    }
                );
        }
    }
});