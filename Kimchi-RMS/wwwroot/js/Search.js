document.querySelectorAll('input[data-table-id]').forEach(searchInput => {
    searchInput.addEventListener('input', function () {
        const searchValue = this.value.toLowerCase().trim(); // Get user's search input
        const tableId = this.dataset.tableId; // Retrieve associated table ID dynamically
        const rows = document.querySelectorAll(`#${tableId} tr`); // Select rows in the table
        const cards = document.querySelectorAll(`#${tableId} .col-xl-3`); // Select cards in the container

        let hasMatch = false; // Track matches

        // Handle table rows
        if (rows.length) {
            rows.forEach(row => {
                const firstCell = row.querySelector('td:first-child'); // First column of the row
                if (firstCell) {
                    const itemName = firstCell.textContent.toLowerCase();
                    if (itemName.includes(searchValue)) {
                        row.style.display = ''; // Show matching rows
                        hasMatch = true;
                    } else {
                        row.style.display = 'none'; // Hide non-matching rows
                    }
                }
            });
        }

        // Handle cards
        if (cards.length) {
            cards.forEach(card => {
                const cardTitle = card.querySelector('.card-title'); // Card title
                if (cardTitle) {
                    const itemName = cardTitle.textContent.toLowerCase();
                    if (itemName.includes(searchValue)) {
                        card.style.display = ''; // Show matching cards
                        hasMatch = true;
                    } else {
                        card.style.display = 'none'; // Hide non-matching cards
                    }
                }
            });
        }
        // Display or hide the warning message
        const warningMessage = document.querySelector(`#${tableId}-warning`);
        if (warningMessage) {
            warningMessage.classList.toggle('d-none', hasMatch);
            warningMessage.classList.toggle('d-block', !hasMatch);
        }
    });
});