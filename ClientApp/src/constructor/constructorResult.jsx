import {useState} from "react";
import TextField from "../inputs/textField.jsx";

function ConstructorResult({isDefault, result, onChange})  {
    const [isEditingText, setIsEditingText] = useState(false);
    const [text, setText] = useState(isDefault ? result : result.result);
    const [threshold, setThreshold] = useState(isDefault ? null : result.percentageThreshold);
    
    const changeText = (e) => {
        const newThreshold = parseFloat(e.target.value);
        if (newThreshold !== threshold) {
            setThreshold(parseFloat(e.target.value));
            onChange({...result,
                percentageThreshold: parseFloat(e.target.value)});
        }
    }
    
    const onTextConfirm = () => {
        if (isDefault) {
            if (text !== result) {
                onChange(text);
            }
        }
        else {
            if (text !== result.result) {
                onChange({...result,
                    result: text});
            }
        }
    }
    
    const onTextCancel = () => {
        if (isDefault) {
            setText(result);
        }
        else {
            setText(result.result);
        }
        setIsEditingText(false);
    }
    
    return (
        <div className="test-card-row">
            {isDefault ? <label htmlFor="default-result">Стандартный результат: </label> 
                : <>
                    <input type="number" min={0} max={100} step={1} value={threshold} onChange={changeText} 
                           autoFocus={true}/>
                    <span>%: </span>
                </>}
            <TextField text={text === null ? "" : text} placeholder="Нет текста" handleEdit={() => setIsEditingText(true)} 
                       isEditing={isEditingText} isDisabled={false} onChange={(e) => setText(e.target.value)} 
                       onCancel={onTextCancel} onConfirm={onTextConfirm}/>
        </div>
    )
}

export default ConstructorResult;