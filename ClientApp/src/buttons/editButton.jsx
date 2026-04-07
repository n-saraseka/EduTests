function EditButton({isEditing, onEditToggle, onConfirm, onCancel, isDisabled}) {
    return isEditing ? (<>
            <img src="/files/icons/check.png" alt="Confirm edit" onClick={onConfirm} 
                 style={{opacity: isDisabled ? 0.5 : 1}} className="edit-icon"/>
            <img src="/files/icons/close.png" alt="Cancel edit" onClick={onCancel}
                 style={{opacity: isDisabled ? 0.5 : 1}} className="edit-icon"/>
        </>
    ) : (
        <img src="/files/icons/edit.png" alt="Edit" onClick={onEditToggle} style={{opacity: isDisabled ? 0.5 : 1}} className="edit-icon"/>
    )
}

export default EditButton;