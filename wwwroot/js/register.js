'use strict';

window.onload = function() {
    let registered = false;
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
    
    const form = document.getElementById('register-form');
    form.addEventListener('submit', async function(e) {
        e.preventDefault();
        let formData = new FormData(form);
        let command = JSON.stringify(formCommand(formData));
        
        let result = await Register(command, token);
        if (result.ok) {
            let data = await result.json();
            console.log(result);
        }
        else {
            console.log("Error: " + result.status);
            let errorText = await result.text();
            console.log("Details: " + errorText);
        }
    })
    
    function formCommand(data) {
        return new RegisterCommand(data.get('Login'), data.get('Username'), data.get('Password'));
    }
    
    class RegisterCommand {
        constructor(login, username, password) {
            this.login = login;
            this.username = username;
            this.password = password;
        }
    }
    
    async function Register(data, token) {
        let response = await fetch("api/users", {
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