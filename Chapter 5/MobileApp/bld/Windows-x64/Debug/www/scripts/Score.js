var MobileApp;
(function (MobileApp) {
    "use strict";
    var Score;
    (function (Score) {
        function initialize() {
            reload_session_info("http://sqlsat495.azurewebsites.net");
            // initialize take photo and share
            $(document).find("#takePhotoAndShare").click(function () {
                get_picture(function (photo) {
                    var url = "http://sqlsat495.azurewebsites.net";
                    var twitter_handle = "marco_parenzan";
                    var hashtag = "sqlsat495";
                    alert(photo);
                    var options = {
                        body: {
                            hashtags: [hashtag],
                            twitter_handles: [twitter_handle],
                            photoData: photo,
                            photoName: twitter_handle + Date.now() + ".jpg"
                        },
                        method: "POST"
                    };
                    var mobileAppsClient = new WindowsAzure.MobileServiceClient(url);
                    var sessionsVotesTable = mobileAppsClient.invokeApi("Photos", options).done(function (result) {
                        alert("Picture posted");
                    }, function (error) {
                        alert("ERROR:" + error);
                    });
                });
            });
            $(document).find("#vote_more").click(function () {
                var id = /\d+/.exec(/id=\d+/.exec(document.URL)[0])[0];
                vote_session("http://sqlsat495.azurewebsites.net", id, "more");
            });
        }
        Score.initialize = initialize;
        function vote_session(url, session_id, vote) {
            var mobileAppsClient = new WindowsAzure.MobileServiceClient(url);
            var sessionsVotesTable = mobileAppsClient.getTable("SessionsVotes");
            sessionsVotesTable.insert({
                session_id: session_id,
                vote: vote
            });
        }
        function reload_session_info(url) {
            var id = /\d+/.exec(/id=\d+/.exec(document.URL)[0])[0];
            var mobileAppsClient = new WindowsAzure.MobileServiceClient(url);
            var sessionsTable = mobileAppsClient.getTable("Sessions");
            sessionsTable.where({
                "id": id
            }).read().then(function (result) {
                var element = result[0];
                $("#session_name").html(element.session_name);
                $("#session_url").attr("href", element.session_url);
                $("#speaker_name").html(element.speaker_name);
                $("#speaker_url").attr("href", element.speaker_url);
            }, function (error) {
                alert(error);
            });
        }
        function get_picture(handler) {
            navigator.camera.getPicture(handler, function (error) {
                navigator.notification.alert(error, function () {
                });
            }, {
                quality: 100,
                destinationType: Camera.DestinationType.DATA_URL,
                sourceType: Camera.PictureSourceType.PHOTOLIBRARY,
                mediaType: Camera.MediaType.PICTURE,
                encodingType: Camera.EncodingType.PNG,
                saveToPhotoAlbum: true
            });
        }
    })(Score = MobileApp.Score || (MobileApp.Score = {}));
    window.onload = function () {
        MobileApp.Application.initialize();
        Score.initialize();
    };
})(MobileApp || (MobileApp = {}));
//# sourceMappingURL=Score.js.map