'use strict';

window.onload = function() {
    let registered = false;
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const form = document.getElementById('login-form');
    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        let formData = new FormData(form);
        let command = JSON.stringify(formCommand(formData));

        let result = await Register(command, token);
        if (result.ok) {
            console.log(result);
        }
        else {
            console.log("Error: " + result.status);
            let errorText = await result.text();
            console.log("Details: " + errorText);
        }
    })

    function formCommand(data) {
        return new LoginCommand(data.get('Login'), data.get('Password'));
    }

    class LoginCommand {
        constructor(login, password) {
            this.login = login;
            this.password = password;
        }
    }

    async function Register(data, token) {
        let response = await fetch("api/auth/login", {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token,
                'Content-Type': 'application/json;charset=utf-8',
            },
            body: data,
        })

        return response;
    }
}