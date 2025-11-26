mergeInto(LibraryManager.library, {
    SendLogToReactNative: function (messagePtr) {
        var message = UTF8ToString(messagePtr);
        // console.log('jslib fun : ' + message);
        if (window.ReactNativeWebView) {
          window.ReactNativeWebView.postMessage(message);
        } 
    },

    SendPostMessage: function(messagePtr) {
      var message = UTF8ToString(messagePtr);
      // console.log('SendReactPostMessage, message sent: ' + message);
      if(window.ReactNativeWebView){
        console.log("Inside ReactNativeWebView");
        window.ReactNativeWebView.postMessage(message);
      }
      else if(window.parent){
        if(window.parent.dispatchReactUnityEvent){
          console.log("Inside ReactNativeWebView"); 
          window.parent.dispatchReactUnityEvent(message); 
        }
      }
    }
});
