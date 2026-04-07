function FileUploader({text, onChange, isDisabled}) {
    return (<>
        <p>{text}</p>
        <input type="file" onChange={onChange} disabled={isDisabled}/>
    </>)
}

export default FileUploader;