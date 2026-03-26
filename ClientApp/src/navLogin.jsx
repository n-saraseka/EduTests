import {useEffect, useState} from "react";

function NavLogin() {
    const [user, setUser] = useState(null);

    useEffect(() => {
        const handleAuthChange = (event) => {
            setUser(event.detail.user);
        };
        window.addEventListener('auth:changed', handleAuthChange);
        return () => window.removeEventListener('auth:changed', handleAuthChange);
    }, []);

    return (
        <ul className="header-navigation auth-navigation">
            {user ?
                <>
                    <li><img id="nav-avatar" src={`/files/users/${user.id}`} alt="Profile picture"/></li>
                    <li><a href={`/user/${user.id}`} className="nav-link" id="nav-user">{user.username}</a></li>
                    <li><a href="/logout" className="nav-link">Выход</a></li>
                </>
                :
                <>
                    <li><a href="/login" className="nav-link">Вход</a></li>
                    <li><a href="/register" className="nav-link">Регистрация</a></li>
                </>
            }
        </ul>
    );
}

export default NavLogin