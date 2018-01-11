thirdKnowledgeMyQueriesApp.factory('thirdKnowledgeDataService', ['$http', '$q',
    function ($http, $q) {
        var _myQueries = [];

        var _getContacts = function () {
            var deferred = $q.defer();
            var controllerQuery = "contact/GetContacts";

            $http.get(controllerQuery)
                .then(function (result) {
                    // Successful
                    angular.copy(result.data, _myQueries);
                    deferred.resolve();
                },
                function (error) {
                    // Error
                    deferred.reject();
                });
            return deferred.promise;
        }



        //Expose methods and fields through revealing pattern
        return {
            contacts: _contacts,
            getContacts: _getContacts,
            addContact: _addContact,
            updateContact: _updateContact,
            deleteContact: _deleteContact,
            findContactById: _findContactById
        }

    }]);