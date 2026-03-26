import {useEffect, useState} from "react";

function LoginAccountHelper() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    useEffect(() => {
        const handleAuth = (event) => {
            setIsAuthenticated(event.detail.isAuthenticated);
        };
        window.addEventListener('auth:account', handleAuth);
        return () => window.removeEventListener('auth:account', handleAuth);
    }, []);

    return isAuthenticated ? <p>Вы вошли в аккаунт. <a href="/logout">Выход</a></p> : 
        <p>Нет аккаунта? <a href="/register">Зарегистрируйтесь здесь</a>.</p>;
}

export default LoginAccountHelper