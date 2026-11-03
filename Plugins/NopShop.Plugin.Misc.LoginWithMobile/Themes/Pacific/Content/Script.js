$(document).ready(function () {
    $("#ActivationCode").keypress(function (e) {
        if (e.keyCode == "13") 
            $(".login-button").click();

        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });

    $("#Username").keypress(function (e) {
        if (e.keyCode == "13")
            $(".login-button").click();

        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });
});
