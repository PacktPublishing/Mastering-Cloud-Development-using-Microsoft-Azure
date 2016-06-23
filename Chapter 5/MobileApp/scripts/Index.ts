module MobileApp {
    "use strict";

    export module Index {
        export function initialize() {
            reload_sessions("http://sqlsat495.azurewebsites.net");
        }

        function reload_sessions(url:string): void {
            var mobileAppsClient = new WindowsAzure.MobileServiceClient(url);
            var sessionsTable = mobileAppsClient.getTable("Sessions");
            sessionsTable.read().then((result: any) => {
                var table = $("#sessions-table > tbody");
                $(result).each(function (index: number, element: any): any {
                    var session_url: string = element.session_url;
                    var sid: string = /\d+/.exec(/sid=\d+/.exec(session_url)[0])[0];
                    var id = element.id;
                    table.append("<tr><td><a href='" + element.session_url + "'>" + element.session_name + "</a></td><td><a href='" + element.speaker_url + "'>" + element.speaker_name + "</a></td><td>" + element.time + "</td><td>" + element.room + "</td><td><a href='score.html?id=" + id + "'>Score</a></td></tr>");
                });
            }, (error: any) => {
                alert(error);
            });
        }
    }

    window.onload = function () {
        Application.initialize(); // reattach application events for the page
        Index.initialize(); // initialize specific page code
    };
}
