//Ajax script to filter
    function FilterByCategories(category = '') {
            var url = category ? '/Customer/Home/FilterByCategories':'/Customer/Home/SelectAll';
    $.ajax({
        url: url,  // Target server endpoint (controller action)
    type: 'GET',  // HTTP method (GET to retrieve data)
    data: {category: category },  // Data sent to the server (category selected)
    beforeSend: function () {
        $("#menuItems").html('<div class="text-center"><i class="spinner-border text-primary"></i> Loading...</div>');
        },
    success: function (data) {
        $("#menuItems").html(data);  // Update the #menuContainer with returned HTML
        },
    error: function () {
        alert('Error loading menu items.');  // Error handling in case the request fails
        }
    });
}






