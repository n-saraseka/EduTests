import {useState} from "react";
import ConfirmationModal from "../modals/confirmationModal.jsx";

const initialData = window.__INITIAL_DATA__;

function UserName({isEditing, name, onNameChange, isDisabled}) {
    return isEditing ? (<input type="text" value={name} onChange={onNameChange} autoFocus disabled={isDisabled}/> ) : 
        (<h2>{name}</h2>);
}

function ProfilePic({cacheTrickSeed}) {
    return cacheTrickSeed === null ?
        (<img src={`/files/users/${initialData.id}`} alt="Profile picture" id="profile-avatar"/>)
        : (<img src={`/files/users/${initialData.id}#${cacheTrickSeed}`} alt="Profile picture" id="profile-avatar"/>);
}

function UserAvatar({onAvatarChange, isDisabled}) {
    return (<div className="card-row">
        <p>Изменить фото профиля: </p>
        <input type="file" onChange={onAvatarChange} disabled={isDisabled}/>
    </div>)
}

function UserDescription({isEditing, text, onDescriptionChange, isDisabled}) {
    return isEditing ? (
        <input type="text" value={text} onChange={onDescriptionChange} autoFocus disabled={isDisabled}/>
    ) : (<p>{text == null ? "Нет описания" : text}</p>);
}

function EditIcon({isEditing, onEditToggle, isDisabled}) {
    return isEditing ? (
        <img src="/files/icons/check.png" alt="Finish editing" onClick={onEditToggle} style={{opacity: isDisabled ? 0.5 : 1}} className="edit-icon"/>
    ) : (
        <img src="/files/icons/edit.png" alt="Edit" onClick={onEditToggle} style={{opacity: isDisabled ? 0.5 : 1}} className="edit-icon"/>
    )
}

function FieldChangeStatus({isSuccess}) {
    return (isSuccess === null ? null : 
        (isSuccess ? <p style={{color: "green"}}>Успешно</p> : <p style={{color: "red"}}>Ошибка</p>));
}

function UserCardHandler() {
    const [isEditingUsername, setIsEditingUsername] = useState(false);
    const [username, setUsername] = useState(initialData.username);
    
    const [isEditingDescription, setIsEditingDescription] = useState(false);
    const [description, setDescription] = useState(initialData.description);
    
    const [isLoginSuccess, setIsLoginSuccess] = useState(null);
    const [isPasswordSuccess, setIsPasswordSuccess] = useState(null);
    
    const [cacheTrick, setCacheTrick] = useState(null);
    
    const [isLoading, setIsLoading] = useState(false);
    
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);

    const handleUsernameEditing = async (event) => {
        event.preventDefault();
        if (isLoading) return;
        
        if (isEditingUsername) {
            const command = new ChangeUsernameCommand(username);
            
            setIsLoading(true);
            const result = await changeUsername(command, initialData.id);
            setIsLoading(false);
            
            if (!result.ok) {
                setUsername(initialData.username);
            }
        }
        setIsEditingUsername(!isEditingUsername);
    }
    
    const handleUsernameChange = (event) => {
        event.preventDefault();
        setUsername(event.target.value);
    }

    const handleDescriptionChange = (event) => {
        event.preventDefault();
        setDescription(event.target.value);
    }

    const handleDescriptionEditing = async (event) => {
        event.preventDefault();
        if (isLoading) return;
        
        if (isEditingDescription) {
            const command = new ChangeDescriptionCommand(description);
            
            setIsLoading(true);
            const result = await changeDescription(command, initialData.id);
            setIsLoading(false);

            if (!result.ok) {
                setDescription(initialData.description);
            }
        }
        setIsEditingDescription(!isEditingDescription);
    }
    
    const handleAvatarChange = async (event) => {
        event.preventDefault();
        if (isLoading) return;
        
        if (event.target.files.length === 1) {
            
            setIsLoading(true);
            const result = await uploadAvatar(event.target.files[0], initialData.id);
            setIsLoading(false);
            
            if (result.ok) {
                setCacheTrick(new Date().getTime());
            }
        }
    }
    
    const handleLoginChange = async (event) => {
        event.preventDefault();
        
        if (isLoading) return;
        
        const newLogin = document.getElementById("new-login");
        const oldPassword = document.getElementById("login-confirm-password");
        
        const command = new ChangeLoginCommand(newLogin.value, oldPassword.value);
        
        setIsLoading(true);
        const result = await changeLogin(command, initialData.id);
        setIsLoading(false);
        
        if (result.ok) {
            setIsLoginSuccess(true);
        }
        else {
            setIsLoginSuccess(false);
        }
    }

    const handlePasswordChange = async (event) => {
        event.preventDefault();

        if (isLoading) return;

        const newPassword = document.getElementById("new-password");
        const oldPassword = document.getElementById("password-confirm-password");

        const command = new ChangePasswordCommand(oldPassword.value, newPassword.value);

        setIsLoading(true);
        const result = await changePassword(command, initialData.id);
        setIsLoading(false);

        if (result.ok) {
            setIsPasswordSuccess(true);
        }
        else {
            setIsPasswordSuccess(false);
        }
    }
    
    const openDeleteModal = () => {
        setIsDeleteModalOpen(true);
    }
    
    const handleDeletion = async (event) => {
        if (isLoading) return;

        let result = await deleteAccount();
        if (result.ok) {
            setIsDeleteModalOpen(false);
            window.location.replace("/home");
        }
    }
    
    const onDeleteModalCancel = () => {
        setIsDeleteModalOpen(false);
    }
    
    return (
        <>
            <div id="card-top">
                <ProfilePic cacheTrickSeed={cacheTrick}/>
                <div id="card-username">
                    <div className="card-row">
                        <UserName name={username} isEditing={isEditingUsername} onNameChange={handleUsernameChange} isDisabled={isLoading}/>
                        <EditIcon isEditing={isEditingUsername} onEditToggle={handleUsernameEditing} isDisabled={isLoading}/>
                    </div>
                    <UserAvatar onAvatarChange={handleAvatarChange} isDisabled={isLoading}/>
                </div>
            </div>
            <div className="card-row">
                <UserDescription text={description} isEditing={isEditingDescription} onDescriptionChange={handleDescriptionChange} isDisabled={isLoading}/>
                <EditIcon isEditing={isEditingDescription} onEditToggle={handleDescriptionEditing} isDisabled={isLoading}/>
            </div>
            <h3>Настройки аккаунта</h3>
            <div className="account-settings-section">
                <span className="account-section-header">Логин</span>
                <label htmlFor="new-login">Новый логин: </label> <input type="text" id="new-login"/>
                <label htmlFor="login-confirm-password">Пароль для подтверждения: </label> <input type="password" id="login-confirm-password"/>
                <button className="btn-primary" onClick={handleLoginChange} disabled={isLoading}>Изменить логин</button>
                <FieldChangeStatus isSuccess={isLoginSuccess}/>
                
            </div>
            <div className="account-settings-section">
                <span className="account-section-header">Пароль</span>
                <label htmlFor="new-password">Новый пароль: </label> <input type="password" id="new-password"/>
                <label htmlFor="password-confirm-password">Пароль для подтверждения: </label> <input type="password" id="password-confirm-password"/>
                <button className="btn-primary" onClick={handlePasswordChange} disabled={isLoading}>Изменить пароль</button>
                <FieldChangeStatus isSuccess={isPasswordSuccess}/>
            </div>
            <button className="btn-danger" onClick={openDeleteModal}>Удалить аккаунт</button>
            {isDeleteModalOpen && <ConfirmationModal onConfirm={handleDeletion} onCancel={onDeleteModalCancel} 
                                                     title="Вы действительно хотите удалить аккаунт?" 
                                                     subtitle="Отменить это действие будет невозможно. Все данные, комментарии и тесты будут утеряны."/>}
        </>
    )
}

class ChangeUsernameCommand {
    constructor(username) {
        this.username = username;
    }
}

class ChangeDescriptionCommand {
    constructor(description) {
        this.description = description;
    }
}

class ChangeLoginCommand {
    constructor(login, password) {
        this.login = login;
        this.password = password;
    }
}

class ChangePasswordCommand {
    constructor(oldPassword, newPassword) {
        this.oldPassword = oldPassword;
        this.newPassword = newPassword;
    }
}

function changeUsername(data, id) {
    return fetch(`/api/users/${id}/username`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
        },
        body: JSON.stringify(data)
    });
}

function changeDescription(data, id) {
    return fetch(`/api/users/${id}/description`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
        },
        body: JSON.stringify(data)
    });
}

function uploadAvatar(file, id) {
    const formData = new FormData();
    formData.append('file', file);
    
    return fetch(`/files/users/${id}`, {
        method: 'PATCH',
        body: formData
    });
}

function changeLogin(data, id) {
    return fetch(`/api/users/${id}/login`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
        },
        body: JSON.stringify(data)
    });
}

function changePassword(data, id) {
    return fetch(`/api/users/${id}/password`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
        },
        body: JSON.stringify(data)
    });
}

function deleteAccount(id) {
    return fetch(`/api/users/${id}`, {
        method: 'DELETE',
    });
}

export default UserCardHandler;