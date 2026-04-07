import EditButton from "../buttons/editButton.jsx";

function TextareaField({isEditing, text, placeholder, onChange, isDisabled, handleEdit}) {
    return (<>
        {isEditing ? (
            <textarea value={text === null ? undefined : text} placeholder="Введите свой текст..." 
                      onChange={onChange} autoFocus disabled={isDisabled}/>) 
            : <p>{text == null || text === "" ? placeholder : text}</p>}
        <EditButton isEditing={isEditing} onEditToggle={handleEdit} isDisabled={isDisabled}/>
    </>);
}

export default TextareaField;