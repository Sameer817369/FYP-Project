
// Function for showing a confirmation alert with dynamic title and text
function showConfirmationAlert(title, text, confirmCallback, confirmText = 'Yes', cancelText = 'Cancel') {
    Swal.fire({
        title: title,
        text: text,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: confirmText,
        cancelButtonText: cancelText
    }).then((result) => {
        if (result.isConfirmed) {
            confirmCallback();
        }
    });
}
// Function to confirm the removal of a cart
function confirmRemoveCart(event, url) {
    event.preventDefault(); // Prevent the default anchor click behavior
    showConfirmationAlert('Are you sure?', 'Do you want to remove this item?', function () {
        window.location.href = url;
    });
}
function confirmRemoveCustomer(event, url) {
    event.preventDefault(); // Prevent the default anchor click behavior
    showConfirmationAlert('Are you sure?', 'Do you want to remove this item from the cart?', function () {
        window.location.href = url;
    });
}
function confirmLogout(event, url) {
    event.preventDefault();
    showConfirmationAlert('Are you sure?', 'Do you want to log out!', function (){
        window.location.herf = url;
    });
}
function confirmAction(event, formId, actionType, confirmColor, cancelColor) {
    event.preventDefault(); // Stop form from submitting immediately

    Swal.fire({
        title: `Are you sure you want to ${actionType}?`,
        text: "This action cannot be undone!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: confirmColor,
        cancelButtonColor: cancelColor,
        confirmButtonText: `Yes, ${actionType.toLowerCase()} it!`
    }).then((result) => {
        if (result.isConfirmed) {
            const form = document.getElementById(formId).submit();
            if (form) {
                form.submit();//Submit the dynamically specified form
            }
        }
    });
}



