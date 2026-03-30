import {useEffect, useState} from "react";

function RegisterAccountHelper() {
    const [isRegistered, setIsRegistered] = useState(false);

    useEffect(() => {
        const handleReg = (event) => {
            setIsRegistered(event.detail.isRegistered);
        };
        window.addEventListener('auth:account', handleReg);
        return () => window.removeEventListener('auth:account', handleReg);
    }, []);

    return isRegistered ? <p>Вы успешно зарегистрировались. <a href="/login">Войти в аккаунт</a></p> :
        <p>Уже имеете аккаунт? <a href="/login">Войдите здесь</a>.</p>;
}

export default RegisterAccountHelper