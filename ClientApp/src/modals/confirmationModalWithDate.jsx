function ConfirmationModalWithDate({title, subtitle, dateTitle, dateMin, onConfirm, onCancel}) {
    return (
        <div className="modal">
            <div className="modal-window">
                <span className="modal-window-title">{title}</span>
                {subtitle !== undefined && (<span className="modal-window-subtitle">{subtitle}</span>)}
                <textarea name="modal-text" id="modal-text" maxLength={256} placeholder="Введите текст..."></textarea>
                <div className="modal-date-block">
                    <label htmlFor="modal-date">{dateTitle}</label>
                    <input type="date" min={dateMin} id="modal-date"/>
                </div>
                <div className="modal-confirmation">
                    <button className="btn btn-secondary" onClick={onConfirm}>Подтвердить</button>
                    <button className="btn btn-primary" onClick={onCancel}>Отмена</button>
                </div>
            </div>
        </div>
    )
}

export default ConfirmationModalWithDate;