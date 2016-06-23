var MobileApp;
(function (MobileApp) {
    "use strict";
    var Application;
    (function (Application) {
        function initialize() {
            document.addEventListener("deviceready", onDeviceReady, false);
        }
        Application.initialize = initialize;
        function onDeviceReady() {
            // handle the Cordova pause and resume events
            document.addEventListener("pause", onPause, false);
            document.addEventListener("resume", onResume, false);
            // todo: Cordova has been loaded. Perform any initialization that requires Cordova here.
        }
        function onPause() {
            // todo: This application has been suspended. Save application state here.
        }
        function onResume() {
            // todo: This application has been reactivated. Restore application state here.
        }
    })(Application = MobileApp.Application || (MobileApp.Application = {}));
})(MobileApp || (MobileApp = {}));
//# sourceMappingURL=Application.js.map