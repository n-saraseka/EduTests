import {useState} from "react";
import ConfirmationModal from "../modals/confirmationModal.jsx";
import FileUploader from "../inputs/fileUploader.jsx";
import TextareaField from "../inputs/textareaField.jsx";
import EditButton from "../buttons/editButton.jsx";

function UserName({isEditing, name, onChange, isDisabled}) {
    return isEditing ? (<input type="text" id="edit-text" value={name} onChange={onChange} autoFocus 
                               disabled={isDisabled}/> ) :
        (<h2>{name}</h2>);
}

function ProfilePic({userId, cacheTrickSeed}) {
    return cacheTrickSeed === null ?
        (<img src={`/files/users/${userId}`} alt="Profile picture" id="profile-avatar"/>)
        : (<img src={`/files/users/${userId}#${cacheTrickSeed}`} alt="Profile picture" id="profile-avatar"/>);
}

function FieldChangeStatus({isSuccess}) {
    return (isSuccess === null ? null : 
        (isSuccess ? <p style={{color: "green"}}>Успешно</p> : <p style={{color: "red"}}>Ошибка</p>));
}

function UserCardHandler({baseUser}) {
    const [user, setUser] = useState(baseUser);
    const [isEditingUsername, setIsEditingUsername] = useState(false);
    const [username, setUsername] = useState(user.username);
    
    const [isEditingDescription, setIsEditingDescription] = useState(false);
    const [description, setDescription] = useState(user.description);
    
    const [isLoginSuccess, setIsLoginSuccess] = useState(null);
    const [isPasswordSuccess, setIsPasswordSuccess] = useState(null);
    
    const [cacheTrick, setCacheTrick] = useState(null);
    
    const [isLoading, setIsLoading] = useState(false);
    
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);

    const handleUsernameEditing = (event) => {
        event.preventDefault();
        if (isLoading) return;
        setIsEditingUsername(true);
    }
    
    const confirmUsernameEdit = async (event) => {
        event.preventDefault();
        if (isLoading) return;
        
        if (username !== user.username) {
            const command = new ChangeUsernameCommand(username);

            setIsLoading(true);
            const result = await changeUsername(command, user.id);
            setIsLoading(false);

            if (!result.ok) {
                setUsername(user.username);
            }
            else {
                setUser({...user, username: username})
            }
        }
        setIsEditingUsername(false);
    }
    
    const cancelUsernameEdit = (event) => {
        event.preventDefault();
        if (isLoading) return;
        setUsername(user.username);
        setIsEditingUsername(false);
    }

    const handleDescriptionEditing = async (event) => {
        event.preventDefault();
        if (isLoading) return;
        setIsEditingDescription(true);
    }
    
    const confirmDescriptionEdit = async (event) => {
        event.preventDefault();
        if (isLoading) return;
        
        if (description !== user.description) {
            const command = new ChangeDescriptionCommand(description);

            setIsLoading(true);
            const result = await changeDescription(command, user.id);
            setIsLoading(false);

            if (!result.ok) {
                setDescription(user.description);
            }
            else {
                setUser({...user, description: description});
            }
        }
        
        setIsEditingDescription(false);
    }
    
    const cancelDescriptionEdit = (event) => {
        event.preventDefault();
        if (isLoading) return;
        setDescription(user.description);
        setIsEditingDescription(false);
    }
    
    const handleAvatarChange = async (event) => {
        event.preventDefault();
        if (isLoading) return;
        
        if (event.target.files.length === 1) {
            
            setIsLoading(true);
            const result = await uploadAvatar(event.target.files[0], user.id);
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
        const result = await changeLogin(command, user.id);
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
        const result = await changePassword(command, user.id);
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
    
    const handleDeletion = async () => {
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
                <ProfilePic userId={user.id} cacheTrickSeed={cacheTrick}/>
                <div id="card-username">
                    <div className="card-row">
                        <UserName name={username} onChange={(e) => setUsername(e.target.value)}
                                  isEditing={isEditingUsername} isDisabled={isLoading}/>
                        <EditButton isEditing={isEditingUsername} onEditToggle={handleUsernameEditing}
                                    onConfirm={confirmUsernameEdit} onCancel={cancelUsernameEdit} isDisabled={isLoading}/>
                    </div>
                    <div className="card-row">
                        <FileUploader text="Изменить фото профиля" onChange={handleAvatarChange} isDisabled={isLoading}/>
                    </div>
                </div>
            </div>
            <div className="card-row">
                <TextareaField text={description} placeholder="Нет описания" isEditing={isEditingDescription}
                               onChange={(e) => setDescription(e.target.value)} isDisabled={isLoading} 
                               handleEdit={handleDescriptionEditing} onConfirm={confirmDescriptionEdit}
                               onCancel={cancelDescriptionEdit}/>
            </div>
            <h3>Настройки аккаунта</h3>
            <div className="account-settings-section">
                <span className="account-section-header">Логин</span>
                <label htmlFor="new-login">Новый логин: </label> <input type="text" id="new-login"/>
                <label htmlFor="login-confirm-password">Пароль для подтверждения: </label> <input type="password" id="login-confirm-password"/>
                <button className="btn btn-primary" onClick={handleLoginChange} disabled={isLoading}>Изменить логин</button>
                <FieldChangeStatus isSuccess={isLoginSuccess}/>
                
            </div>
            <div className="account-settings-section">
                <span className="account-section-header">Пароль</span>
                <label htmlFor="new-password">Новый пароль: </label> <input type="password" id="new-password"/>
                <label htmlFor="password-confirm-password">Пароль для подтверждения: </label> <input type="password" id="password-confirm-password"/>
                <button className="btn btn-primary" onClick={handlePasswordChange} disabled={isLoading}>Изменить пароль</button>
                <FieldChangeStatus isSuccess={isPasswordSuccess}/>
            </div>
            <button className="btn btn-danger" onClick={openDeleteModal}>Удалить аккаунт</button>
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