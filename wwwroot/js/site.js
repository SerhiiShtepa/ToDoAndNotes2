// full return url
document.addEventListener("DOMContentLoaded", function () {
    var returnUrlInputs = document.querySelectorAll(".returnUrlInput");
    var fullPageUrl = window.location.href;

    returnUrlInputs.forEach(function (returnUrlInput) {
        returnUrlInput.value = fullPageUrl;
    });
});

// cancelButton
document.addEventListener("DOMContentLoaded", function () {
    var cancelButton = document.getElementById("cancelButton");

    cancelButton.addEventListener("click", function () {
        history.back();
    });
});