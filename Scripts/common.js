$(document).ready(function () {
    $(":text").labelify({
        labelledClass: "labelify"
    });

    if (getParameterByName("success") != "") {
        $(document).showMessage({
            thisMessage: [getParameterByName("success")],
            className: "success",
            opacity: 95,
            displayNavigation: false,
            autoClose: true,
            delayTime: 5000
        });

        history.replaceState(null, document.title, getRawUrl());
    }

    if (getParameterByName("warning") != "") {
        $(document).showMessage({
            thisMessage: [getParameterByName("warning")],
            className: "warning",
            opacity: 95,
            displayNavigation: false,
            autoClose: true,
            delayTime: 30000
        });

        history.replaceState(null, document.title, getRawUrl());
    }

    if (getParameterByName("error") != "") {
        $(document).showMessage({
            thisMessage: [getParameterByName("error")],
            className: "error",
            opacity: 95,
            displayNavigation: false,
            autoClose: true,
            delayTime: 5000
        });

        history.replaceState(null, document.title, getRawUrl());
    }
});


function getRawUrl() {
    var regexS = "^(.*)&(?:.*)$";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return window.location.href;
    else
        return results[1];
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}