module MobileApp {
    "use strict";

    export module Score {
        export function initialize() {
            reload_session_info("http://sqlsat495.azurewebsites.net");

            // initialize take photo and share
            $(document).find("#takePhotoAndShare").click((): void => {

                get_picture((photo: string) => {

                    var url = "http://sqlsat495.azurewebsites.net";
                    var twitter_handle = "marco_parenzan";
                    var hashtag = "sqlsat495";

                    alert(photo);

                    var options: Microsoft.WindowsAzure.InvokeApiOptions = {
                        body: {
                            hashtags: [hashtag]
                            , twitter_handles: [twitter_handle]
                            , photoData: photo
                            , photoName: twitter_handle + Date.now() + ".jpg"
                        },
                        method: "POST"
                    };

                    var mobileAppsClient = new WindowsAzure.MobileServiceClient(url);
                    var sessionsVotesTable = mobileAppsClient.invokeApi("Photos", options).done(
                        (result: any): void => {
                            alert("Picture posted");
                        }
                        , (error: any): void => {
                            alert("ERROR:" + error);
                        }
                    );

                });

            });

            $(document).find("#vote_more").click(function () {
                var id: string = /\d+/.exec(/id=\d+/.exec(document.URL)[0])[0];
                vote_session("http://sqlsat495.azurewebsites.net", id, "more");
            });
        }

        function vote_session(url: string, session_id: string, vote: string): void {
            var mobileAppsClient = new WindowsAzure.MobileServiceClient(url);
            var sessionsVotesTable = mobileAppsClient.getTable("SessionsVotes");
            sessionsVotesTable.insert({
                session_id: session_id
                ,
                vote: vote
            });
        }

        function reload_session_info(url: string): void {

            var id: string = /\d+/.exec(/id=\d+/.exec(document.URL)[0])[0];

            var mobileAppsClient = new WindowsAzure.MobileServiceClient(url);
            var sessionsTable = mobileAppsClient.getTable("Sessions");
            sessionsTable.where({
                "id": id
            }).read().then((result: any) => {
                var element: any = result[0];

                $("#session_name").html(element.session_name);
                $("#session_url").attr("href", element.session_url);
                $("#speaker_name").html(element.speaker_name);
                $("#speaker_url").attr("href", element.speaker_url);

            }, (error: any) => {
                alert(error);
            });
        }

        function get_picture(handler: (data: string) => any): void {
            navigator.camera.getPicture(
                handler
                , function (error: string): void {
                    navigator.notification.alert(error, (): void => {
                    });
                },
                {
                    quality: 100,
                    destinationType: Camera.DestinationType.DATA_URL,
                    sourceType: Camera.PictureSourceType.PHOTOLIBRARY,
                    mediaType: Camera.MediaType.PICTURE,
                    encodingType: Camera.EncodingType.PNG,
                    saveToPhotoAlbum: true
                }
            );
        }
    }

    window.onload = function () {
        Application.initialize();
        Score.initialize();
    };
}