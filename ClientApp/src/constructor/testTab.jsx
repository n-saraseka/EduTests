import {useState} from "react";
import TextField from "../inputs/textField.jsx";
import TextareaField from "../inputs/textareaField.jsx";
import FileUploader from "../inputs/fileUploader.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";
import ConstructorQuestion from "./constructorQuestion.jsx";

function TestTab({test, setTest, onSetThumbnail}) {
    const [isEditingName, setIsEditingName] = useState(false);
    const [name, setName] = useState(test.name);
    const [isEditingDescription, setIsEditingDescription] = useState(false);
    const [description, setDescription] = useState(test.description);
    const [isAddingTag, setIsAddingTag] = useState(false);
    const [showingTagRemoveButton, setShowingTagRemoveButton] = useState(false);
    const [newTag, setNewTag] = useState("");
    
    const handleNameEdit = () => {
        setIsEditingName(true);
    }
    
    const nameConfirm = () => {
        if (name !== test.name) {
            setTest({...test, name: name});
        }
        setIsEditingName(false);
    }
    
    const nameCancel = () => {
        setName(test.name);
        setIsEditingName(false);
    }
    
    const handleDescriptionEdit = () => {
        setIsEditingDescription(true);
    }
    
    const descriptionConfirm = () => {
        if (description !== test.description) {
            setTest({...test, description: description});
        }
        setIsEditingDescription(false);
    }
    
    const descriptionCancel = () => {
        setDescription(test.description);
        setIsEditingDescription(false);
    }
    
    const removeTag = (name) => {
        let oldTest = test;
        test.tags = test.tags.filter(t => t !== name);
        setTest(oldTest);
    }
    
    const showTagRemoveButton = (event) => {
        event.target.after(<DeleteButton entityType="tag" onDelete={() => removeTag(event.target.key)}/>);
    }
    
    const hideTagRemoveButton = (event) => {
        event.target.nextSibling.remove();
    }
    
    const addTag = (event) => {
        event.preventDefault();
        let oldTags = test.tags;
        const tagName = document.querySelector("#tag-input").value.toLowerCase();
        if (tagName === "" || tagName === null || oldTags.indexOf(tagName) === -1) {
            oldTags.push(tagName);
            setTest(prevTest => ({...prevTest, tags: oldTags}));
        }
        setIsAddingTag(false);
    }
    
    const cancelAddingTag = () => {
        setIsAddingTag(false);
    }
    
    const handleTag = () => {
        setIsAddingTag(true);
    }
    
    const changeAccessType = (event) => {
        let oldTest = test;
        const newValue = parseInt(event.target.value);
        if (newValue !== oldTest.accessType) {
            oldTest.accessType = newValue;
            setTest(oldTest);
        }
    }
    
    const updateQuestion = (index, updatedQuestion) => {
        setTest(prevTest => ({...prevTest, questions: prevTest.questions.map(q =>
            q.orderIndex === index ? updatedQuestion : q)}));
    }
    
    const addQuestion = () => {
        let newQuestions = test.questions;
        const emptyQuestionData = {
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
            orderIndex: newQuestions.length + 1,
        };
        newQuestions.push({
            orderIndex: newQuestions.length + 1,
            type: 0,
            description: null,
            data: emptyQuestionData,
            correctData: emptyQuestionData
        });
        setTest({...test, questions: newQuestions});
    }
    
    const moveQuestion = (question, isMovingDown) => {
        const questions = test.questions;
        const oldIndex = question.orderIndex;
        const newIndex = isMovingDown ? oldIndex + 1 : oldIndex - 1;
        
        if (newIndex < 1 || newIndex > questions.length) return;
        
        let newQuestions = [...questions];
        
        const temp = newQuestions[oldIndex - 1];
        newQuestions[oldIndex - 1] = newQuestions[newIndex - 1];
        newQuestions[oldIndex - 1].orderIndex = oldIndex;
        newQuestions[newIndex - 1] = temp;
        newQuestions[newIndex - 1].orderIndex = newIndex;

        setTest(prevTest => ({...prevTest, questions: newQuestions}));
    }
    
    const removeQuestion = (question) => {
        const questionIndex = question.orderIndex;
        const newQuestions = test.questions.filter(q => q.orderIndex !== questionIndex);
        const fixedQuestions = newQuestions.map(q => {
            if (q.orderIndex > questionIndex) {
                q.orderIndex -= 1;
            }
            return q;
        });
        setTest(prevTest => ({...prevTest, questions: fixedQuestions}));
    }
    
    return (<div className="constructor-tab">
        <h1>Настройки теста</h1>
        <div className="test-card">
            <div className="test-card-row">
                <TextField text={name} placeholder="Без названия"
                           isEditing={isEditingName} handleEdit={handleNameEdit} onConfirm={nameConfirm}
                           onChange={(e) => setName(e.target.value)} onCancel={nameCancel} isDisabled={false}/>
            </div>
            <div className="test-card-row">
                <TextareaField text={description} placeholder="Без описания"
                               isEditing={isEditingDescription} handleEdit={handleDescriptionEdit}
                               onChange={(e) => setDescription(e.target.value)}
                               onConfirm={descriptionConfirm} onCancel={descriptionCancel} isDisabled={false}/>
            </div>
            <div className="test-card-row">
                <FileUploader text="Обложка: " isDisabled={false} onChange={onSetThumbnail}/>
            </div>
            <div className="test-card-row">
                <label htmlFor="access-type">Тип доступа:</label>
                <select name="access-type" id="access-type" defaultValue={test.accessType} onChange={changeAccessType}>
                    <option value="0">Публичный</option>
                    <option value="1">Доступ по ссылке</option>
                    <option value="2">Частный доступ</option>
                </select>
            </div>
            <div className="test-card-row">
                <span>Теги: </span>
                <div id="test-tags">
                    <ul className="tags-list">
                        {test.tags.map(tag => (<li className="test-tag" key={tag}
                                                   onMouseEnter={() => setShowingTagRemoveButton(true)}
                                              onMouseLeave={() => setShowingTagRemoveButton(false)}>
                            <span>{tag}</span>
                            {showingTagRemoveButton && <DeleteButton entityType="tag" onDelete={() => removeTag(tag)}/>}
                        </li>))}
                    </ul>
                    {isAddingTag ? <>
                        <input type="text" placeholder="Текст тега..." defaultValue={newTag} 
                               onChange={(e) => setNewTag(e.target.value)} id="tag-input" maxLength="32"/>
                            <img src="/files/icons/check.png" alt="Confirm addition" onClick={addTag} 
                                 className="edit-icon"/>
                            <img src="/files/icons/close.png" alt="Cancel addition" onClick={cancelAddingTag}
                                 className="edit-icon"/>
                    </>
                        : <button className="btn btn-primary" onClick={handleTag}>Добавить</button>}
                </div>
            </div>
        </div>
        <h1>Вопросы</h1>
        <div id="questions">
            {test.questions.map(q => <div className="constructor-question" key={q.orderIndex}>
                <div className="question-control">
                    <img src="/files/icons/up.png" alt="Move up" onClick={() => moveQuestion(q, false)} 
                         className="question-icon"/>
                    <img src="/files/icons/down.png" alt="Move up" onClick={() => moveQuestion(q, true)}
                         className="question-icon"/>
                    <DeleteButton entityType="question" onDelete={() => removeQuestion(q)}/>
                </div>
                <ConstructorQuestion question={q} onChange={(updated) => updateQuestion(q.orderIndex, updated)}/>
            </div>)}
            <button className="btn btn-primary" onClick={addQuestion}>Добавить вопрос</button>
        </div>
    </div>)
}

export default TestTab;