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
        console.log(feature);
        labelStyle.getText().setText(feature.label);
        return style;
    },
    source: source
});



const loadRegioes = () => {
    //faz o loop sobre a/as regiões salvas no bd
    //marca a região no mapa
    let features = JSON.parse($('#data').val());
  
    features.forEach((f) => {

        var circle = new ol.geom.Circle(f.center, f.radius);
        var circleFeature = new ol.Feature(circle);
        circleFeature.label = f.regiao;
        // vector layer
        source.addFeature(circleFeature);

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

loadRegioes();





// clears the map and the output of the data
function clearMap() {
    vector.getSource().clear();
    map.updateSize();
    map.getView().setZoom(5.5);
    map.getView().setCenter(ol.proj.fromLonLat([-44.142891, -20.027849]));
    document.getElementById('data').value = '';
    regioes = [];
    regioes.length = 0;
}



//Visão de todas regiões. edição travada, apenas permite cliques nas regiões

$(map.getViewport()).on("click", function (e) {
    map.forEachFeatureAtPixel(map.getEventPixel(e), function (feature, layer) {
        //do something
        console.log('thanks for clicking');
        e.preventDefault();
        e.stopPropagation();
    });
});