$(document).ready(function () {

    var year = ($("#ActivationCodeExpiredDateYear").val());
    var month = ($("#ActivationCodeExpiredDateMonth").val());
    var day = $("#ActivationCodeExpiredDateDay").val();
    var hour = $("#ActivationCodeExpiredDateHour").val();
    var min = $("#ActivationCodeExpiredDateMinute").val();
    var sec = $("#ActivationCodeExpiredDateSecond").val();
    var temp = new Date(year,
        month, day, hour,
        min, sec);

    document.getElementById("countDownDate").value = temp;

    var flag = false;
    // Update the count down every 1 second
    var t = setInterval(function () {
        if (!flag) {
            // Get today's date and time
            var now = new Date().getTime();
            // Find the distance between now and the count down date
            var date = new Date(document.getElementById("countDownDate").value);
            var distance = Date.parse(date) - now;
            // Time calculations for days, hours, minutes and seconds
            var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            var seconds = Math.floor((distance % (1000 * 60)) / 1000);

            var strMinutes = "";
            if (minutes > -1 && minutes < 10) {
                strMinutes = "0" + minutes;
            } else {
                strMinutes = minutes;
            }

            var strSeconds = "";
            if (seconds > -1 && seconds < 10) {
                strSeconds = "0" + seconds;
            } else {
                strSeconds = seconds;
            }

            var ele = document.getElementById("timer");
            if (minutes >= 0 && strSeconds >= 0) {
                ele.innerHTML = strMinutes + ":" + strSeconds;

            }

            if (ele.innerHTML == "00:00") {
                $("#timer-decription").hide();

                $("#send-activation-code-again").addClass("lwm-display-block");
                $("#send-activation-code-again").removeClass("lwm-display-none");

                flag = true;
            } else {
                $("#timer-decription").show();

                $("#send-activation-code-again").removeClass("lwm-display-block");
                $("#send-activation-code-again").addClass("lwm-display-none");
            }
        }
    }, 1000);


    $("#send-activation-code-again").click(function (e) {
        e.preventDefault();

        var postData = {
            phonenumber: $("#Username").val()
        }
        addAntiForgeryToken(postData);

        $.ajax({
            cache: false,
            url: "/GetActivationCodeChange",
            type: "POST",
            data: postData,
            dataType: "json",
            success: function (data) {
                if (data.message == "success") {
                    $("#countDownDate").val(data.expireDate);

                    $("#timer-decription").show();

                    $("#send-activation-code-again").addClass("lwm-display-none");
                    $("#send-activation-code-again").removeClass("lwm-display-block");

                    flag = false;
                }
                else
                    alert(data.message);
            },
            error: function (jqXHR, textStatus, errorThrown) {
            },
            complete: function (jqXHR, textStatus) {
            }
        });

    });
});