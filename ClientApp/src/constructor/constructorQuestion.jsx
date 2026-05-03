import {useState} from "react";
import EditButton from "../buttons/editButton.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";
import BbcodePreset from "../bbcodePreset.jsx";

function ConstructorQuestion({question, onChange}) {
    // Editing
    const [isEditingText, setIsEditingText] = useState(false);
    const [editingAnswerIndex, setEditingAnswerIndex] = useState(null);
    const [editingAnswer, setEditingAnswer] = useState(null);
    const [description, setDescription] = useState(question.description);
    
    const changeQuestionType = (event) => {
        const newType = parseInt(event.target.value);
        if (newType !== question.type) {
            onChange({...question,
                data: {
                ...question.data, 
                    options: [],
                    leftColumn: [],
                    rightColumn: [],
                    pairs: [],
                    tolerance: null,
                    numberAnswer: null,
                    sequence: [],
                    textAnswer: null,
                    validAnswers: [],
                    validIndices: [],
                },
                correctData: {
                ...question.correctData,
                    options: [],
                    leftColumn: [],
                    rightColumn: [],
                    pairs: [],
                    tolerance: null,
                    numberAnswer: null,
                    sequence: [],
                    textAnswer: null,
                    validAnswers: [],
                    validIndices: []
                },
                type: newType});
        }
    }
    
    const handleDescriptionEdit = () => setIsEditingText(true);
    const descriptionConfirm = () => {
        if (description !== question.description) {
            onChange({...question, description: description});
        }
        setIsEditingText(false);
    }
    
    const descriptionCancel = () => {
        setDescription(question.description);
        setIsEditingText(false);
    }
    
    const editNumberAnswer = (event) => {
        const newNumber = parseFloat(event.target.value);
        if (question.correctData.numberAnswer !== newNumber) {
            onChange({...question, correctData: {...question.correctData, numberAnswer: newNumber}});
        }
    }

    const editNumberTolerance = (event) => {
        const newTolerance = parseFloat(event.target.value);
        if (question.data.tolerance !== newTolerance) {
            onChange({...question, data: {...question.data, tolerance: newTolerance}});
        }
    }
    
    const addAnswer = (type) => {
        switch(type) {
            // Single choice
            case 0:
            case 1:
                const newOptions = [...question.data.options, "Новый ответ"];
                onChange ({...question, data: {...question.data, options: newOptions}});
                break;
            // Text answer
            case 3:
                const newAnswers = [...question.correctData.validAnswers, "Новый ответ"];
                onChange ({...question, correctData: {...question.correctData, validAnswers: newAnswers}});
                break;
        }
    }
    
    const editAnswer = (index, type) => {
        setEditingAnswerIndex(index);
        switch(type) {
            // Single choice
            case 0:
            case 1:
                setEditingAnswer(question.data.options[index]);
                break;
            // Text answer
            case 3:
                setEditingAnswer(question.correctData.validAnswers[index]);
        }
    }
    
    const confirmEditAnswer = (index, type) => {
        switch(type) {
            // Single choice and multiple choice
            case 0:
            case 1:
                if (editingAnswer !== question.data.options[index]) {
                    onChange({...question, data: {...question.data, 
                            options: question.data.options.map((option, i) => 
                                i === index ? editingAnswer : option)}});
                }
                break;
            // Text answer
            case 3:
                if (editingAnswer !== question.correctData.validAnswers[index]) {
                    onChange({...question, 
                        correctData: {...question.correctData, 
                            validAnswers: question.correctData.validAnswers.map((option, i) => 
                                i === index ? editingAnswer : option)}});
                }
                break;
        }
        setEditingAnswerIndex(null);
    }
    
    const cancelEditAnswer = () => {
        setEditingAnswerIndex(null);
    }
    
    const onDeleteAnswer = (index, type) => {
        let option = "";
        switch(type) {
            // Single choice and multiple choice
            case 0:
            case 1:
                option = question.data.options[index];
                const newOptions = question.data.options.filter(o => o !== option);
                const newCorrectOptions = question.correctData.options.filter(o => o !== option);
                onChange({...question, 
                    data: {...question.data, options: newOptions},
                    correctData: {...question.correctData, options: newCorrectOptions}});
                break;
            // Text answer
            case 3:
                option = question.correctData.validAnswers[index];
                const newAnswers = question.correctData.validAnswers.filter(a => a !== option);
                onChange({...question, correctData: {...question.correctData, validAnswers: newAnswers}});
                break;
        }
    }
    
    const editChoiceData = (event, index, type) => {
        switch (type) {
            // Single choice
            case 0:
                const newIndices = [index];
                onChange({...question, correctData: {...question.correctData, validIndices: newIndices}});
                break;
            // Multiple choice
            case 1:
                let indices = question.correctData.validIndices;
                if (event.target.checked) {
                    indices.push(index);
                }
                else {
                    indices = indices.filter(i => i !== index);
                }
                onChange({...question, correctData: {...question.correctData, validIndices: indices}});
                break;
        }
    }
    
    function SequenceContainer() {
        const addItem = () => {
            const newSequence = question.correctData.sequence.concat(["Введите текст"]);
            onChange({...question, correctData: {...question.correctData, sequence: newSequence}});
        }
        
        const changeSequence = (seq) => {
            onChange({...question, correctData: {...question.correctData, sequence: seq}});
        }
        
        const deleteItem = (index) => {
            onChange({...question, correctData: {...question.correctData, sequence: question.correctData.sequence.filter((e, i) => i !== index)}});
        }
        
        return (<>
            <div className="sequence-items">
                {question.correctData.sequence.map((e, i) =>
                    <SequenceItem key={i} baseText={e} index={i} 
                                  sequence={question.correctData.sequence} onModification={changeSequence} 
                                  onDeletion={deleteItem}/>)}
                <button className="btn btn-primary" onClick={addItem}>Добавить</button>
            </div>
        </>);
    }
    
    function SequenceItem({baseText, sequence, onModification, onDeletion, index}) {
        const [isDragged, setIsDragged] = useState(false);
        const [showEditButtons, setShowEditButtons] = useState(false);
        const [isEditing, setIsEditing] = useState(false);
        const [text, setText] = useState(baseText);
        const [tempText, setTempText] = useState(baseText);
        const [shadow, setShadow] = useState(null);
        
        const handleDragStart = (event) => {
            event.dataTransfer.setData('text/plain', JSON.stringify({
                draggedIndex: index,
                draggedText: text
            }));
            setIsDragged(true);
        }

        const handleDragOver = (event) => {
            event.preventDefault();
            if (event.target.classList.contains('sequence-item')) {
                const potentialElementCoord = event.target.getBoundingClientRect();
                const center = potentialElementCoord.left + potentialElementCoord.width / 2;

                if (event.clientX < center) {
                    setShadow('-2px 0 0 0 blue');
                } else {
                    setShadow('2px 0 0 0 blue');
                }
            }
        };

        const handleDragLeave = (event) => {
            event.preventDefault();
            if (event.target.classList.contains('sequence-item')) {
                setShadow(null);
            }
        };
        
        const handleDrop = (event) => {
            event.preventDefault();
            if (event.target.classList.contains('sequence-item') && event.dataTransfer.getData('text/plain')) {
                const draggedData = JSON.parse(event.dataTransfer.getData('text/plain'));
                const { draggedIndex, draggedText } = draggedData;

                const potentialElementCoord = event.target.getBoundingClientRect();
                const center = potentialElementCoord.left + potentialElementCoord.width / 2;

                // if cursor is to the left - insert to the left
                // insert to the right otherwise
                const cursorPosition = event.clientX;
                const newSequence = [...sequence];
                
                newSequence.splice(draggedIndex, 1);
                
                let usedIndex = index;
                
                if (draggedIndex < index) {
                    usedIndex--;
                }

                if (cursorPosition < center) {
                    newSequence.splice(usedIndex, 0, draggedText);
                }
                else {
                    newSequence.splice(usedIndex + 1, 0, draggedText);
                }
                
                onModification(newSequence);
                setIsDragged(false);
            }
        }
        
        const confirmEdit = () => {
            setText(tempText);
            setTempText(text);
            setIsEditing(false);
        }
        
        return (<div className={`sequence-item${isDragged ? ' dragged' : ''}`} draggable={true} data-index={index}
                     onDragStart={handleDragStart} onDragEnd={() => setIsDragged(false)} 
                     onDragOver={handleDragOver} onDragLeave={handleDragLeave} onDrop={handleDrop}
                     onMouseOver={() => setShowEditButtons(true)} onMouseLeave={() => setShowEditButtons(false)} 
                     style={{boxShadow: shadow}}>
            {isEditing ? <input type="text" id={`sequence-item-${question.orderIndex}-${index}`} 
                                defaultValue={tempText} onChange={(event) => setTempText(event.target.value)} autoFocus={true}/>
            : <div>{text}</div>}
            {showEditButtons && <>
                <EditButton isEditing={isEditing} onEditToggle={setIsEditing}
                            onCancel={() => setIsEditing(false)} onConfirm={confirmEdit}
                            isDisabled={false}/>
                <DeleteButton entityType="answer" onDelete={() => onDeletion(index)}/>
            </>}
        </div>)
    }
    
    function QuestionSwitch({type}) {
        switch(type) {
            // Single choice
            case 0:
                return <>
                    <p>Варианты ответа:</p>
                    <div id="asnwers">
                        {question.data.options.map((option, index) => (<div className="test-card-row" key={index}>
                            <input type="radio" id={`option-${index}`} value={option} name="answer" 
                                   checked={question.correctData.validIndices.includes(index)} 
                                   onChange={(e) => editChoiceData(e, index, type)}/>
                            {editingAnswerIndex === index ? <input type="text" key={`input-${index}`} 
                                                                   value={editingAnswer} autoFocus={true}
                                                                   onChange={(e) => setEditingAnswer(e.target.value)}/> 
                                : <label htmlFor={`option-${index}`} key={`label-${index}`} >{option}</label>}
                            <EditButton isEditing={editingAnswerIndex === index} onEditToggle={() => editAnswer(index, type)}
                                       onCancel={cancelEditAnswer} onConfirm={() => confirmEditAnswer(index, type)}
                                        isDisabled={editingAnswerIndex !== null && editingAnswerIndex !== index}/>
                            <DeleteButton entityType="answer" onDelete={() => onDeleteAnswer(index, type)}/>
                        </div>))}
                        <button className="btn btn-primary" onClick={() => addAnswer(0)}>Добавить</button>
                    </div>
                </>
            // Multiple choice
            case 1:
                return <>
                    <p>Варианты ответа:</p>
                    <div id="asnwers">
                        {question.data.options.map((option, index) => (<div className="test-card-row" key={index}>
                            <input type="checkbox" id={`option-${index}`} value={option} name="answer"
                                   checked={question.correctData.validIndices.includes(index)}
                                   onChange={(e) => editChoiceData(e, index, type)}/>
                            {editingAnswerIndex === index ? <input type="text" key={`input-${index}`} 
                                                                   value={editingAnswer} autoFocus={true}
                                                                   onChange={(e) => setEditingAnswer(e.target.value)}/>
                                : <label htmlFor={`option-${index}`} key={`label-${index}`} >{option}</label>}
                            <EditButton isEditing={editingAnswerIndex === index} onEditToggle={() => editAnswer(index, type)}
                                        onCancel={cancelEditAnswer} onConfirm={() => confirmEditAnswer(index, type)}
                                        onChange={(e) => editChoiceData(e, index, type)}
                                        isDisabled={editingAnswerIndex !== null && editingAnswerIndex !== index}/>
                            <DeleteButton entityType="answer" onDelete={() => onDeleteAnswer(index, type)}/>
                        </div>))}
                        <button className="btn btn-primary" onClick={() => addAnswer(1)}>Добавить</button>
                    </div>
                </>
            // Number input
            case 2:
                return <>
                    <div className="test-card-row">
                        <label htmlFor="answer">Правильный ответ: </label>
                        <input type="number" id="answer" name="answer" defaultValue={question.correctData.numberAnswer} 
                               onChange={editNumberAnswer}/>
                    </div>
                    <div className="test-card-row">
                        <label htmlFor="answer">Погрешность: </label>
                        <input type="number" id="answer" name="answer" defaultValue={question.data.tolerance}
                               onChange={editNumberTolerance}/>
                    </div>
                </>
            // Text input
            case 3:
                return <>
                    <p>Варианты ответа:</p>
                    <div id="answers">
                        {question.correctData.validAnswers.map((option, index) => 
                            (<div className="test-card-row" key={index}>
                            {editingAnswerIndex === index ? <input type="text" id={`option-${index}`} key={`input-${index}`} 
                                                                   value={editingAnswer} autoFocus={true}
                                                                   onChange={(e) => setEditingAnswer(e.target.value)}/>
                                : <p className="answer" key={`p-${index}`} >{option}</p>}
                            <EditButton isEditing={editingAnswerIndex === index} onEditToggle={() => editAnswer(index, type)}
                                        onCancel={cancelEditAnswer} onConfirm={() => confirmEditAnswer(index, type)}
                                        isDisabled={editingAnswerIndex !== null && editingAnswerIndex !== index}/>
                            <DeleteButton entityType="answer" onDelete={() => onDeleteAnswer(index, type)}/>
                        </div>)
                        )
                        }
                        <button className="btn btn-primary" onClick={() => addAnswer(3)}>Добавить</button>
                    </div>
                </>
            case 4:
                return <>
                    <p>Последовательность:</p>
                    <SequenceContainer question={question}/>
                </>
        }
    }
    
    return (<div className="test-card">
        <div className="test-card-row">
            <label htmlFor="question-type">Тип вопроса: </label>
            <select name="question-type" id="question-type" defaultValue={question.type} onChange={changeQuestionType}>
                <option value="0">Одиночный выбор</option>
                <option value="1">Множественный выбор</option>
                <option value="2">Ввод числа</option>
                <option value="3">Ввод текста</option>
                <option value="4">Последовательность</option>
            </select>
        </div>
        <div className="test-card-row question-description">
            {isEditingText ? (
                    <textarea id="edit-text" value={description === null ? undefined : description} onChange={onChange}
                              placeholder="Введите свой текст..." autoFocus disabled={false}/>)
                : <BbcodePreset text={description === null ? "Без описания" : description}/>}
            <EditButton isEditing={isEditingText} onEditToggle={handleDescriptionEdit} 
                        onConfirm={descriptionConfirm} onCancel={descriptionCancel} isDisabled={false}/>
        </div>
        <QuestionSwitch type={question.type}/>
    </div>);
}

export default ConstructorQuestion;