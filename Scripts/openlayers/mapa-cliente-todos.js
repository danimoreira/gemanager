var regioes = [];

var raster = new ol.layer.Tile({
    source: new ol.source.OSM()
});

var iconStyle = new ol.style.Style({
    image: new ol.style.Circle({
        radius: 6,
        stroke: new ol.style.Stroke({
            color: '#fff'
        }),
        fill: new ol.style.Fill({
            color: '#3399CC'
        })
    })
});

var labelStyle = new ol.style.Style({
    text: new ol.style.Text({
        font: '12px Calibri,sans-serif',
        overflow: true,
        fill: new ol.style.Fill({
            color: '#000'
        }),
        stroke: new ol.style.Stroke({
            color: '#fff',
            width: 3
        })
    })
});

var style = [iconStyle, labelStyle];

var source = new ol.source.Vector();



var vector = new ol.layer.Vector({
    name: 'regiao',
    style: function (feature) {
        labelStyle.getText().setText(feature.label);
        return style;
    },
    source: source
});



const loadLocais = () => {
    //faz o loop sobre a/as regiões salvas no bd
    //marca a região no mapa
    let features = JSON.parse($('#LocaisLatiLong').val());
  
    features.forEach((f) => {

        var point = new ol.geom.Point([f.latitude, f.longitude]);
        var pointFeature = new ol.Feature(point);
        //circleFeature.label = f.Id;
        // vector layer
        source.addFeature(pointFeature);

    })


    var vectorLayer = new ol.layer.Vector({
        source: source
    });
    map.addLayer(vectorLayer);

}



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

loadLocais();

//Visão de todas regiões. edição travada, apenas permite cliques nas regiões

$(map.getViewport()).on("click", function (e) {
    map.forEachFeatureAtPixel(map.getEventPixel(e), function (feature, layer) {
        //do something
        console.log('thanks for clicking', feature.getGeometry().getCoordinates());
        e.preventDefault();
        e.stopPropagation();
    });
});