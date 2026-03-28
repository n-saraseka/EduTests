function ConfirmationModal({title, subtitle, onConfirm, onCancel}) {
    return (
        <div className="modal">
            <div className="modal-window">
                <span className="modal-window-title">{title}</span>
                {subtitle !== undefined && (<span className="modal-window-subtitle">{subtitle}</span>)}
                <div className="modal-confirmation">
                    <button className="btn-secondary" onClick={onConfirm}>Подтвердить</button>
                    <button className="btn-primary" onClick={onCancel}>Отмена</button>
                </div>
            </div>
        </div>
    )
}

export default ConfirmationModal;