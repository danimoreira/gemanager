var regiaoObj = {};

var raster = new ol.layer.Tile({
    source: new ol.source.OSM()
});

var source = new ol.source.Vector({ wrapX: false });

var vector = new ol.layer.Vector({  source});



var defaultMapValues = {
    layers: [raster, vector],
    controls: ol.control.defaults({ attribution: false }),
    target: 'map',
    view: new ol.View({
        center: ol.proj.fromLonLat([-44.142891, -20.027849]),
        zoom: 5.5,
        maxZoom: 6,
    })
};

var map = new ol.Map(defaultMapValues);

var modify = new ol.interaction.Modify({ source });
map.addInteraction(modify);

var draw, snap; // global so we can remove them later

// creates unique id's
function uid() {
    var id = 0;
    return function () {
        if (arguments[0] === 0) {
            id = 0;
        }
        return id++;
    }
}

function updateData(event) {
    if ($('#limit_1_feature').length == 1) {
        clearMap();
    }
    //draw : event.feature.getGeometry()
    //change : event.target

    let geo = typeof event.feature !== "undefined" ? event.feature.getGeometry() : event.target;

    let regiao = document.getElementById('Latilong').value;

    if (!regiao) {
        regiao = '';
    }

    console.log(geo.getCoordinates());

    regiaoObj = { latitude: geo.getCoordinates()[0], longitude: geo.getCoordinates()[1] };


    $('#Latilong').val(JSON.stringify(regiaoObj));
}


const loadLatilong = () => {
    //faz o loop sobre a/as regiões salvas no bd
    //marca a região no mapa
    const viewObject = JSON.parse('{"latitude":-5359368.62353164,"longitude":-2261070.1252551987}');
    const coordinates = [viewObject.latitude, viewObject.longitude];
    var point = new ol.geom.Point(coordinates);
    var pointFeature = new ol.Feature(point);

    var geometry = pointFeature.getGeometry();
    geometry.on('change', function (evt) {
        updateData(evt);
    }, this);

    // vector layer
    source.addFeature(pointFeature);
    var vectorLayer = new ol.layer.Vector({
        source: source
    });
    map.addLayer(vectorLayer); 
}



function addInteractions() {
 
    draw = new ol.interaction.Draw({
        source: source,
        type: 'Point',
        maxPoints: 1
    });

    draw.on('drawend', function (event) {

        updateData(event);


    });


    map.addInteraction(draw);

    loadLatilong();

}

addInteractions();




// clears the map and the output of the data
function clearMap() {
    vector.getSource().clear();
    map.updateSize();
    map.getView().setZoom(5.5);
    map.getView().setCenter(ol.proj.fromLonLat([-44.142891, -20.027849]));
    document.getElementById('Latilong').value = '';
    regiaoObj = {};
}
