$(document).ready(function () {
    var marker;
    var map;
    var currentUrl = window.location.href.toLowerCase();
    var defaultLat = "35.7144537";
    var defaultLng = "51.3437968";

    var inputLat = $("#address_attribute_@Model.LatitudeAddressAttributeId");
    var inputLng = $("#address_attribute_@Model.LongitudeAddressAttributeId");

    inputLat.prop("readonly", true);
    inputLng.prop("readonly", true);

    if (currentUrl.includes("admin")) {
        inputLat.parent().parent().hide();
        inputLng.parent().parent().hide();
    }
    else {
        inputLat.parent().hide();
        inputLng.parent().hide();
    }

    var currentLat = inputLat.val();
    var currentLng = inputLng.val();

    if (currentUrl.includes("onepagecheckout")) {

        var addressId = $("#billing-address-select").val();

        if (addressId && addressId !== "") {
            GetLatLngByAddressId(addressId).then(latLng => {

                if (latLng) {

                    var lat = latLng.CustomAddressAttributes.filter(function (element) { return element.Id === @Model.LatitudeAddressAttributeId })[0]?.DefaultValue;

                    var lng = latLng.CustomAddressAttributes.filter(function (element) { return element.Id === @Model.LongitudeAddressAttributeId })[0]?.DefaultValue;

                    marker = InitialMap(lat, lng);

                    setTimeout(function () {
                        map.invalidateSize(true);
                    }, 100);
                }
            });
        }
        else {
            marker = InitialMap(defaultLat, defaultLng);
            setTimeout(function () {
                map.invalidateSize(true);
            }, 100);
        }
    }
    //else {

    //    marker = InitialMap(currentLat, currentLng);

    //    setTimeout(function () {
    //        map.invalidateSize(true);
    //    }, 100);
    //}

    $("#edit-billing-address-button, #edit-shipping-address-button").on("click", function () {
        currentUrl = window.location.href.toLowerCase();

        var addressId;

        if ($(this).prop("id") === "edit-billing-address-button")
            addressId = $("#billing-address-select").val();
        else
            addressId = $("#shipping-address-select").val();

        if (addressId && addressId !== "") {
            GetLatLngByAddressId(addressId).then(latLng => {

                if (latLng) {
                    var lat = latLng.CustomAddressAttributes.filter(function (element) { return element.Id === @Model.LatitudeAddressAttributeId })[0]?.DefaultValue;
                    var lng = latLng.CustomAddressAttributes.filter(function (element) { return element.Id === @Model.LongitudeAddressAttributeId })[0]?.DefaultValue;

                    inputLat = $("#address_attribute_@Model.LatitudeAddressAttributeId");
                    inputLng = $("#address_attribute_@Model.LongitudeAddressAttributeId");

                    inputLat.prop("readonly", true);
                    inputLng.prop("readonly", true);

                    inputLat.parent().hide();
                    inputLng.parent().hide();
                    marker = InitialMap(lat, lng);
                }
            });
        }

        setTimeout(function () {
            map.invalidateSize(true);
        }, 100);

    });

    $("#billing-address-select").on("change", function () {

        //if (!$(this).val()) {
        //    marker = InitialMap(defaultLat, defaultLng);

        setTimeout(function () {
            map.invalidateSize(true);
        }, 100);
        //}
    });

    $("#find-my-location").on("click", function () {

        map.locate({ setView: true, maxZoom: 16 });

        map.on('locationfound', onLocationFound);
        map.on('locationerror', onLocationError);

    });

    function InitialMap(lat, lng) {

        if (currentUrl.includes("opc-billing")) {
            $("#checkout-step-shipping .al-map-container").remove();
        }
        else if (currentUrl.includes("opc-shipping")) {
            $("#checkout-step-billing .al-map-container").remove();
        }

        if (currentUrl.includes("opc-billing") && $("#checkout-step-billing .al-map-container").length == 0) {
            $("#checkout-step-billing .custom-attributes").last().after('<div id="map" class="al-map-container" style="height: 500px;"></div>');
        }
        else if (currentUrl.includes("opc-shipping") && $("#checkout-step-shipping .al-map-container").length == 0) {
            $("#checkout-step-shipping .custom-attributes").last().after('<div id="map" class="al-map-container" style="height: 500px;"></div>');
        }

        var lat = !lat || lat === "" ? defaultLat : lat;
        var lng = !lng || lng === "" ? defaultLng : lng;

        map = L.map('map')
            .setView([lat, lng], 16)
            .on("click", UpdateMarkerLocation);

        L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
            maxZoom: 16
        }).addTo(map);

        var marker = L.marker([lat, lng], { draggable: 'true' })
            .addTo(map)
            .on("mouseup", UpdateMarkerLocation);

        return marker;
    }

    function UpdateMarkerLocation(e) {
        marker.setLatLng(e.latlng);
        UpdateLatLong(e);
    }

    function onLocationFound(e) {
        var radius = e.accuracy;

        UpdateMarkerLocation(e);
    }

    function onLocationError(e) {
        alert(e.message);
    }

    function UpdateLatLong(e) {

        inputLng.val(e.latlng.lng);

        inputLat.val(e.latlng.lat);
    }

    function GetLatLngByAddressId(addressId) {

        return $.ajax({
            url: "Checkout/GetAddressById?addressId=" + addressId
        });
    }

});
