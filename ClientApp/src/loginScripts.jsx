import { createRoot } from 'react-dom/client';
import ErrorHandler from './errorHandler.jsx';
import NavLogin from './NavLogin.jsx';
import LoginAccountHelper from "./loginAccountHelper.jsx";

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
const form = document.getElementById('login-form');

const navRoot = createRoot(document.getElementsByClassName("header-navigation auth-navigation")[0]);
const errorRoot = createRoot(document.getElementById("error-text"));
const accountHelperRoot = createRoot(document.getElementById("login-account-help"));

if (form) {
    navRoot.render(<NavLogin />);
    errorRoot.render(<ErrorHandler />);
    accountHelperRoot.render(<LoginAccountHelper />);
}

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    let formData = new FormData(form);
    let command = JSON.stringify(formCommand(formData));

    let result = await Login(command, token);
    if (result.ok) {
        const user = await result.json();
        window.dispatchEvent(new CustomEvent('auth:changed', { detail: { user: user } }));
        window.dispatchEvent(new CustomEvent('auth:error', { detail: { message: '' } }));
        window.dispatchEvent(new CustomEvent('auth:account', { detail: { isAuthenticated: true } }));
        form.style.display = 'none';
    }
    else {
        console.log("Error: " + result.status);
        let errorText = await result.text();
        window.dispatchEvent(new CustomEvent('auth:error', { detail: { message: errorText } }));
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

async function Login(data, token) {
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
