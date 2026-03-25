import {useEffect, useState} from "react";

function ErrorHandler() {
    const [message, setMessage] = useState('');

    useEffect(() => {
        const handleError = (event) => {
            setMessage(event.detail.message);
        };
        window.addEventListener('auth:error', handleError);
        return () => window.removeEventListener('auth:error', handleError);
    }, []);

    return message ? <p style={{ color: 'red' }}>{message}</p> : null;
}

export default ErrorHandler