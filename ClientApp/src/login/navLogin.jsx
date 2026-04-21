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
                    <li><input type="text" maxLength={64} className="searchbar" placeholder="Поиск..."/></li>
                    <li><img id="nav-avatar" src={`/files/users/${user.id}`} alt="Profile picture"/></li>
                    <li><a href={`/user/${user.id}`} className="nav-link" id="nav-user">{user.username}</a></li>
                    <li><a href={`user/${user.id}/my_account`} className="nav-link">
                        <img src="/files/icons/myacocunt.png" alt="Личный кабинет" className="nav-icon"/>
                    </a></li>
                    <li><a href="/constructor" className="nav-link">
                        <img src="/files/icons/constructor.png" alt="Конструктор" className="nav-icon"/>
                    </a></li>
                    <li><a href="/logout" className="nav-link">Выход</a></li>
                </>
                :
                <>
                    <li><input type="text" maxLength={64} className="searchbar" placeholder="Поиск..."/></li>
                    <li><a href="/login" className="nav-link">Вход</a></li>
                    <li><a href="/register" className="nav-link">Регистрация</a></li>
                </>
            }
        </ul>
    );
}

export default NavLogin