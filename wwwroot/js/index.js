'use strict';

window.onload = function() {
    const initialData = window.__INITIAL_DATA__;
    const buttons = document.querySelectorAll(".card > button.btn-primary");
    for (let i = 0; i < buttons.length; i++) {
        buttons[i].addEventListener("click", function() {
            window.location.href = `/test/${initialData[i].id}`;
        })
    }
}