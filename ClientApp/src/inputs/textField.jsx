import EditButton from "../buttons/editButton.jsx";

function TextField({isEditing, text, placeholder, onChange, isDisabled, handleEdit, onConfirm, onCancel}) {
    return (<>
        {isEditing ? (
                <input id="edit-text" defaultValue={text === null ? undefined : text} 
                       autoFocus disabled={isDisabled}/>)
            : <p>{text == null || text === "" ? placeholder : text}</p>}
        <EditButton isEditing={isEditing} onEditToggle={handleEdit} onConfirm={onConfirm} onCancel={onCancel} 
                    isDisabled={isDisabled}/>
    </>);
}

export default TextField;