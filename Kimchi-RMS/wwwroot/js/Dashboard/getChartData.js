
$(document).ready(function () {
    const charts = [
        {
            endpoint: "/Admin/Dashboard/GetTotalBookingChartData",
            countSelector: "#spanTotalBookingCount",
            sectionSelector: "#sectionBookingCount",
            chartElementId: "totalBookingsChart"
        },
        {
            endpoint: "/Admin/Dashboard/GetTotalOrdersChartData",
            countSelector: "#spanTotalOrderCount",
            sectionSelector: "#sectionOrderCount",
            chartElementId: "totalOrderChart"
        },
        {
            endpoint: "/Admin/Dashboard/GetTotalRevenuChartData",
            countSelector: "#spanTotalRevenueCount",
            sectionSelector: "#sectionRevenueCount",
            chartElementId: "totalRevenueChart"
        },
    ];

    charts.forEach(config => loadChart(config));
});

function loadChart({ endpoint, countSelector, sectionSelector, chartElementId }) {
    $(".chart-spinner").show();

    $.ajax({
        url: endpoint,
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            updateCount(countSelector, response.totalCount);
            updateRatioIndicator(sectionSelector, response.hasRatioIncreased, response.countInCurrentMonth);
            loadBarChart(chartElementId, response);
        },
        complete: function () {
            $(".chart-spinner").hide();
        }
    });
}

function updateCount(selector, count) {
    $(selector).text(count);
}

function updateRatioIndicator(sectionSelector, hasIncreased, currentCount) {
    const section = $(sectionSelector);
    section.empty();

    const iconClass = hasIncreased ? "bi-arrow-up-right-circle" : "bi-arrow-down-right-circle";
    const textClass = hasIncreased ? "text-success" : "text-danger";

    const ratioElement = $(`
        <span class="${textClass} me-1">
            <i class="bi ${iconClass} me-1"></i>
            <span>${currentCount}</span>
        </span>
    `);

    section.append(ratioElement).append("since last month");
}

