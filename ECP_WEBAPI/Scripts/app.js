var ViewModel = function () {
    var self = this;
    self.phienlvs = ko.observableArray();
    self.error = ko.observable();

    var phienlvsUri = '/api/PhienLamViec';

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });
    }

    function getAllPhienLviecs() {
        ajaxHelper(phienlvsUri, 'GET').done(function (data) {
            self.phienlvs(data);
        });
    }

    // Fetch the initial data.
    getAllPhienLviecs();
};

ko.applyBindings(new ViewModel());