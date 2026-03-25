import React, {createContext, useEffect} from "react";
import { createRoot } from 'react-dom/client';
import ErrorHandler from './errorHandler.jsx';

const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
const form = document.getElementById('login-form');

function NavLogin() {
    const [isAuthenticated, setIsAuthenticated] = React.useState(false);
    
    useEffect(() => {
        const handleAuthChange = (event) => {
            setIsAuthenticated(event.detail.isAuthenticated);
        };
        window.addEventListener('auth:changed', handleAuthChange);
        return () => window.removeEventListener('auth:changed', handleAuthChange);
    }, []);
    
    return (
        <ul className="header-navigation auth-navigation">
            {isAuthenticated ? 
                    <li><a href="/logout" className="nav-link">Выход</a></li>
                :
                    <>
                        <li><a href="/login" className="nav-link">Вход</a></li>
                        <li><a href="/register" className="nav-link">Регистрация</a></li>
                    </>
            }
        </ul>
    );
}

const navRoot = createRoot(document.getElementsByClassName("header-navigation auth-navigation")[0]);
const errorRoot = createRoot(document.getElementById("error-text"));

if (form) {
    navRoot.render(<NavLogin />);
    errorRoot.render(<ErrorHandler />);
}

form.addEventListener('submit', async (e) => {
    e.preventDefault();
    let formData = new FormData(form);
    let command = JSON.stringify(formCommand(formData));

    let result = await Register(command, token);
    if (result.ok) {
        window.dispatchEvent(new CustomEvent('auth:changed', { detail: { isAuthenticated: true } }));
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
