var regiaoObj = {};

var raster = new ol.layer.Tile({
    source: new ol.source.OSM()
});

var source = new ol.source.Vector();

var vector = new ol.layer.Vector({
    name: 'regiao',
    source: source,
    style: new ol.style.Style({
        fill: new ol.style.Fill({
            color: 'rgba(255, 255, 255, 0.2)'
        }),
        stroke: new ol.style.Stroke({
            color: '#ffcc33',
            width: 2
        }),
        image: new ol.style.Circle({
            radius: 7,
            fill: new ol.style.Fill({
                color: '#ffcc33'
            })
        })
    })
});



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

    let regiao = document.getElementById('Descricao').value;
    regiaoObj = { regiao, center: geo.getCenter(), radius: geo.getRadius() };


    $('#data').val(JSON.stringify(regiaoObj));
}


const loadregiao = () => {
    //faz o loop sobre a/as regiões salvas no bd
    //marca a região no mapa
    let viewObject = JSON.parse($('#data').val());
    let center = viewObject.center;
    var circle = new ol.geom.Circle(center, viewObject.radius);
    var circleFeature = new ol.Feature(circle);

    var geometry = circleFeature.getGeometry();
    geometry.on('change', function (evt) {
        updateData(evt);
    }, this);

    // vector layer
    source.addFeature(circleFeature);
    var vectorLayer = new ol.layer.Vector({
        source: source
    });
    map.addLayer(vectorLayer); 
}



function addInteractions() {
    draw = new ol.interaction.Draw({
        source: source,
        type: 'Circle',
        freehand: false
    });

    draw.on('drawend', function (event) {

        updateData(event);


    });


    map.addInteraction(draw);

    snap = new ol.interaction.Snap({ source });
    map.addInteraction(snap);



    loadregiao();

}

addInteractions();




// clears the map and the output of the data
function clearMap() {
    vector.getSource().clear();
    map.updateSize();
    map.getView().setZoom(5.5);
    map.getView().setCenter(ol.proj.fromLonLat([-44.142891, -20.027849]));
    document.getElementById('data').value = '';
    regiaoObj = {};
}
