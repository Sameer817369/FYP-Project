function filterData({ url, data = {}, targetElement, errorMessage = 'Error loading data.' }) {
    $.ajax({
        url: url,
        type: 'GET',
        data: data,
        beforeSend: function () {
            $(targetElement).html(`
                <div class="text-center">
                    <i class="spinner-border text-primary"></i> Loading...
                </div>
            `);
        },
        success: function (response) {
            $(targetElement).html(response);
        },
        error: function () {
            alert(errorMessage);
        }
    });
}
// Menu Category
function FilterMenuByCategories(category = '') {
    filterData({
        url: '/Admin/Menu/FilterByCategories',
        data: { category: category },
        targetElement: '#menuTableItems',
        errorMessage: 'Error loading menu items.'
    });
}

// Menu Status
function FilterMenuByStatus(status = '') {
    filterData({
        url: '/Admin/Menu/FilterByStatus',
        data: { status: status },
        targetElement: '#menuTableItems',
        errorMessage: 'Error loading menu items.'
    });
}

// Staff Roles
function FilterStaffByRoles(roles = '') {
    filterData({
        url: '/Admin/Employee/FilterByRole',
        data: { roles: roles },
        targetElement: '#staffTableItems',
        errorMessage: 'Error loading staff.'
    });
}

// Inventory Category
function FilterInventoryByCategories(category = '') {
    filterData({
        url: '/Admin/Inventroy/FilterInventoryByCategory',
        data: { category: category },
        targetElement: '#inventoryTableItems',
        errorMessage: 'Error loading inventory items.'
    });
}

// Inventory Status
function FilterInventoryByStatus(status = '') {
    filterData({
        url: '/Admin/Inventroy/FilterInventoryByStatus',
        data: { status: status },
        targetElement: '#inventoryTableItems',
        errorMessage: 'Error loading inventory items.'
    });
}

// Order Status
function FilterOrderByStatus(status = '') {
    filterData({
        url: '/Admin/Order/FilterOrderByStatus',
        data: { status: status },
        targetElement: '#ordersTableItems',
        errorMessage: 'Error loading order items.'
    });
}

// Booking Status
function FilterBookingByStatus(status = '') {
    filterData({
        url: '/Admin/BookingManagement/FilterByStatus',
        data: { status: status },
        targetElement: '#bookingTableItems',
        errorMessage: 'Error loading menu items.'
    });
}
// User Status
function FilterUserByStatus(status = '') {
    filterData({
        url: '/Admin/User/FilterByStatus',
        data: { status: status },
        targetElement: '#userTableItems',
        errorMessage: 'Error loading users.'
    });
}
// User role
function FilterUserByRole(role = '') {

    filterData({
        url: '/Admin/User/FilterByRole',
        data: { role: role },
        targetElement: '#userTableItems',
        errorMessage: 'Error loading users.'
    });
}
   

