$(document).ready(function () {
    const filters = [
        {
            endpoint: "/Admin/Order/FilterByDateRange",
            targetElement: '#ordersTableItems'
        },
        {
            endpoint: "/Admin/FeedbackManagement/FilterByDate",
            targetElement: '#feedbackTableItems'
        },
        {
            endpoint: "/Admin/employee/FilterByDate",
            targetElement: '#staffTableItems'
        },
        {
            endpoint: "/Admin/PaymentManagement/FilterByDate",
            targetElement: '#transactionTableItems'
        },
        {
            endpoint: "/Admin/BookingManagement/FilterByDate",
            targetElement: '#bookingTableItems'
        }
    ];

    $('#filterBtn').click(function (e) {
        e.preventDefault();

        const startDate = $('#startDate').val();
        const endDate = $('#endDate').val();

        // Loop through the filters and find the one whose target element exists on the page
        const activeFilter = filters.find(filter => $(filter.targetElement).length > 0);

        if (!activeFilter) {
            alert('No matching table found for filtering.');
            return;
        }

        applyDateFilter({
            endpoint: activeFilter.endpoint,
            targetElement: activeFilter.targetElement,
            startDate: startDate,
            endDate: endDate
        });
    });
});

// Reusable function
function applyDateFilter({ endpoint, targetElement, startDate, endDate }) {
    $.ajax({
        url: endpoint,
        type: 'GET',
        data: { startDate: startDate, endDate: endDate },
        beforeSend: function () {
            $(targetElement).html(`
                <div class="text-center">
                    <i class="spinner-border text-primary"></i> Loading...
                </div>
            `);
        },
        success: function (result) {
            $(targetElement).html(result);
        },
        error: function () {
            alert('Error loading data');
        }
    });
}


