var MobileApp;
(function (MobileApp) {
    "use strict";
    var Index;
    (function (Index) {
        function initialize() {
            reload_sessions("http://sqlsat495.azurewebsites.net");
        }
        Index.initialize = initialize;
        function reload_sessions(url) {
            var mobileAppsClient = new WindowsAzure.MobileServiceClient(url);
            var sessionsTable = mobileAppsClient.getTable("Sessions");
            sessionsTable.read().then(function (result) {
                var table = $("#sessions-table > tbody");
                $(result).each(function (index, element) {
                    var session_url = element.session_url;
                    var sid = /\d+/.exec(/sid=\d+/.exec(session_url)[0])[0];
                    var id = element.id;
                    table.append("<tr><td><a href='" + element.session_url + "'>" + element.session_name + "</a></td><td><a href='" + element.speaker_url + "'>" + element.speaker_name + "</a></td><td>" + element.time + "</td><td>" + element.room + "</td><td><a href='score.html?id=" + id + "'>Score</a></td></tr>");
                });
            }, function (error) {
                alert(error);
            });
        }
    })(Index = MobileApp.Index || (MobileApp.Index = {}));
    window.onload = function () {
        MobileApp.Application.initialize(); // reattach application events for the page
        Index.initialize(); // initialize specific page code
    };
})(MobileApp || (MobileApp = {}));
//# sourceMappingURL=Index.js.map