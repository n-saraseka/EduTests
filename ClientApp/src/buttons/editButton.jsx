function EditButton({isEditing, onEditToggle, isDisabled}) {
    return isEditing ? (
        <img src="/files/icons/check.png" alt="Finish editing" onClick={onEditToggle} style={{opacity: isDisabled ? 0.5 : 1}} className="edit-icon"/>
    ) : (
        <img src="/files/icons/edit.png" alt="Edit" onClick={onEditToggle} style={{opacity: isDisabled ? 0.5 : 1}} className="edit-icon"/>
    )
}

export default EditButton;