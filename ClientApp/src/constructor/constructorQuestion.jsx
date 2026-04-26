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