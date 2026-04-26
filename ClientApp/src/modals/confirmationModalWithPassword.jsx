import { useState } from "react";

function ConfirmationModalWithPassword({title, subtitle, onConfirm, onCancel, passwordError}) {
    const [password, setPassword] = useState('');
    
    const passPassword = () => {
        onConfirm(password);
    }
    
    return (
        <div className="modal">
            <div className="modal-window">
                <span className="modal-window-title">{title}</span>
                {subtitle !== undefined && (<span className="modal-window-subtitle">{subtitle}</span>)}
                <input type="password" name="modal-password" id="modal-password" maxLength={256} placeholder="Введите текст..." 
                       defaultValue={password} onChange={(event) => setPassword(event.target.value)}/>
                {passwordError != null && (<span style={{color: "red"}}>{passwordError}</span>)}
                <div className="modal-confirmation">
                    <button className="btn btn-secondary" onClick={passPassword}>Подтвердить</button>
                    <button className="btn btn-primary" onClick={onCancel}>Отмена</button>
                </div>
            </div>
        </div>
    )
}

export default ConfirmationModalWithPassword;