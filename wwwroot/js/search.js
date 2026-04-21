'use strict';

document.addEventListener('DOMContentLoaded', function() {
    const searchbars = document.querySelectorAll(".searchbar");
    for (let i = 0; i < searchbars.length; i++) {
        const searchbar = searchbars[i];
        searchbar.addEventListener("keydown", function(event) {
            if (event.key === "Enter") {
                event.preventDefault();
                const query = event.target.value;
                if (query !== "") {
                    window.location.href = `/search?query=${query}`;
                }
            }
        })
    }
})