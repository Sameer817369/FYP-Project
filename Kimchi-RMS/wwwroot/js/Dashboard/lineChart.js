$(document).ready(function () {
    loadLineChartData();
});

function loadLineChartData() {
    $(".chart-spinner").show();

    $.ajax({
        url: '/Admin/Dashboard/GetOrderLineChartData',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            loadLineChart("orderLineChart", data);
            $(".chart-spinner").hide();
        }
    });
}

function loadLineChart(id, data) {
    var options = {
        series: data.series,
        chart: {
            height: 350,
            type: "line",
        },
        stroke: {
            curve: 'smooth',
            width:2
        },
        markers: {
            size: 4,
            strokeWidth: 0,
            hover: {
                size: 8
            }
        },
        xaxis: {
            categories: data.categories
        },
    };

    var chart = new ApexCharts(document.querySelector("#" + id), options);

    chart.render();
}

