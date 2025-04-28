

    // Restaurant address
    const restaurantAddress = "Shanti Marg, Kathmandu 44600, Nepal";

    // Function to initialize the map with both locations
    function initMap(userCoords, restaurantCoords) {
        const map = L.map('map').setView(restaurantCoords, 14);

    // Add OpenStreetMap tiles
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
    attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

    // Add restaurant marker
    const restaurantMarker = L.marker(restaurantCoords)
    .addTo(map)
    .bindPopup("<b>Kimchi</b><br>" + restaurantAddress);

        // Add user marker if available
        let markers = [restaurantMarker];

        if (userCoords) {
            const userMarker = L.marker(userCoords)
        .addTo(map)
        .bindPopup("<b>Your Location</b>")
        .openPopup();
        markers.push(userMarker);
        }

        // Adjust view to fit both markers
        const group = new L.featureGroup(markers);
        map.fitBounds(group.getBounds().pad(0.3));
    }

        // Get restaurant coordinates from Nominatim
        function getRestaurantCoordinates(callback) {
            fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(restaurantAddress)}`)
                .then(response => response.json())
                .then(data => {
                    if (data && data.length > 0) {
                        const lat = parseFloat(data[0].lat);
                        const lon = parseFloat(data[0].lon);
                        callback([lat, lon]);
                    } else {
                        alert("Could not find the restaurant location.");
                        callback(null);
                    }
                })
                .catch(error => {
                    console.error("Error fetching restaurant location:", error);
                    callback(null);
                });
    }

        // Get user's current location
        function getUserCoordinates(callback) {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                position => {
                    const { latitude, longitude } = position.coords;
                    callback([latitude, longitude]);
                },
                error => {
                    console.warn("User location not available. Showing only restaurant.");
                    callback(null); // Fallback to only restaurant
                }
            );
        } else {
            console.warn("Geolocation not supported.");
        callback(null);
        }
    }

    // Run both location fetches
    getRestaurantCoordinates(restaurantCoords => {
        if (!restaurantCoords) return;
        getUserCoordinates(userCoords => {
            initMap(userCoords, restaurantCoords);
        });
    });