import EditButton from "../buttons/editButton.jsx";

function TextField({isEditing, text, placeholder, onChange, isDisabled, handleEdit}) {
    return (<>
        {isEditing ? (
                <input type="text" value={text === null ? undefined : text} 
                       onChange={onChange} autoFocus disabled={isDisabled}/>)
            : <p>{text == null || text === "" ? placeholder : text}</p>}
        <EditButton isEditing={isEditing} onEditToggle={handleEdit} isDisabled={isDisabled}/>
    </>);
}

export default TextField;