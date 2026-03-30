import { createRoot } from 'react-dom/client';
import ErrorHandler from './errorHandler.jsx';
import RegisterAccountHelper from './register/registerAccountHelper.jsx';

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
const form = document.getElementById('register-form');

const errorRoot = createRoot(document.getElementById("error-text"));
const accountHelperRoot = createRoot(document.getElementById("reg-account-help"));

if (form) {
    errorRoot.render(<ErrorHandler />);
    accountHelperRoot.render(<RegisterAccountHelper />);
}

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    let formData = new FormData(form);
    let command = JSON.stringify(formCommand(formData));

    let result = await register(command, token);
    if (result.ok) {
        window.dispatchEvent(new CustomEvent('auth:account', { detail: { isRegistered: true } }));
        window.dispatchEvent(new CustomEvent('auth:error', { detail: { message: '' } }));
        form.style.display = 'none';
    }
    else {
        console.log("Error: " + result.status);
        let errorText = await result.text();
        window.dispatchEvent(new CustomEvent('auth:error', { detail: { message: errorText } }));
    }
})

function formCommand(data) {
    return new RegisterCommand(data.get('Login'), data.get('Password'), data.get('Username'));
}

class RegisterCommand {
    constructor(login, password, username) {
        this.login = login;
        this.password = password;
        this.username = username;
    }
}

async function register(data, token) {
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