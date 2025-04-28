function loadBarChart(id, data) {
    var chartColors = getChartColor(id);
    var options = {
        fill: {
            colors: chartColors
        },

        chart: {
            height: 90,
            width: 90,
            type: "radialBar",
            sparkline: {
                enable: true
            },
            offsetY: -10,
        },

        series: data.series,

        plotOptions: {
            radialBar: {

                dataLabels: {
                    value: {

                        offsetY: -10,
                    }
                }
            }
        },
        labels: ['']

    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);

    chart.render();
}
function getChartColor(id) {
    if (document.getElementById(id) !== null) {
        var colors = document.getElementById(id).getAttribute("data-colors");
        if (colors) {
            colors = JSON.parse(colors);
            return colors.map(function (value) {
                var newValue = value.replace(" ", "");
                if (newValue.indexOf(",") === -1) {
                    var color = getComputedStyle(document.documentElement).getPropertyValue(newValue);
                    if (color) return color;
                    else return newValue;
                }
            });
        }
    }
}


