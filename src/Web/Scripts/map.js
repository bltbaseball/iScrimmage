var map;
function initialize() {
    var myLatlng = new google.maps.LatLng(26.099867, -80.137750);
    var mapOptions = {
        zoom: 13,
        center: myLatlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    var map = new google.maps.Map(document.getElementById('map'), mapOptions);

    var marker = new google.maps.Marker({
        position: myLatlng,
        map: map
    });
}

google.maps.event.addDomListener(window, 'load', initialize);