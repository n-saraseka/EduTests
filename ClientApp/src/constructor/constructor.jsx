import {useRef, useState} from "react";
import TestAside from "./testAside.jsx";
import TestTab from "./testTab.jsx";
import ResultTab from "./resultTab.jsx";
import ExtraSettingsTab from "./extraSettingsTab.jsx";
import ConfirmationModal from "../modals/confirmationModal.jsx";

const tabs = ["test", "result", "extra-settings", "publish"];

function Constructor({baseTest, user}) {
    const [test, setTest] = useState(baseTest !== null ? baseTest :
        {
            userId: user.id,
            name: "Без названия",
            description: null,
            tags: [],
            questions: [],
            results: [],
            timeLimit: null,
            attemptLimit: null,
            defaultResult: null, 
            password: null
        });
    const thumbnailRef = useRef(null);
    const [currentTab, setCurrentTab] = useState(0); // Default to test and questions settings
    const fieldNames = ['timeLimit', 'attemptLimit'];
    const [lastFocusedField, setLastFocusedField] = useState(null);
    const [isUploading, setIsUploading] = useState(false);
    const [showingUpload, setShowingUpload] = useState(false);
    const [error, setError] = useState(null);
    
    const uploadTest = async () => {
        const command = new CreateOrUpdateTestCommand(
            test.name,
            test.description,
            test.tags,
            test.questions,
            test.attemptLimit,
            test.timeLimit,
            test.results,
            test.password
        );
        let postResult;
        
        setShowingUpload(true);
        setIsUploading(true);
        
        if (test.id === undefined) {
            postResult = await createTest(command);
        }
        else {
            postResult = await updateTest(command, test.id);
        }
        
        const resultText = await postResult.text();
        
        if (!postResult.ok) {
            setError(`Ошибка: ${resultText}`);
        }
        else {
            if (thumbnailRef.current !== null) {
                const resultJson = JSON.parse(resultText);
                const postThumbResult = await uploadTestImage(thumbnailRef.current, resultJson.id);
                const postThumbText = await postThumbResult.text();
                if (!postThumbResult.ok) {
                    setError(`Ошибка: ${postThumbText}`);
                }
            }
        }
        
        setIsUploading(false);
    }
    
    const addThumbnail = (event) => {
        if (event.target.files.length === 1) {
            thumbnailRef.current = event.target.files[0];
        }
    }
    
    function TabSwitch() {
        switch(currentTab) {
            // General settings and questions
            case 0:
                return <TestTab test={test} setTest={setTest} onSetThumbnail={addThumbnail}/>;
            // Result settings
            case 1:
                return <ResultTab test={test} setTest={setTest}/>;
            // Extra settings
            case 2:
                return <ExtraSettingsTab test={test} setTest={setTest} fieldNames={fieldNames}
                                         lastFocusedField={lastFocusedField} setLastFocusedField={setLastFocusedField}/>;
            default:
                return <>
                    <TestTab test={test} setTest={setTest}/>
                    {showingUpload ?
                        <div className="modal">
                            <div className="modal-window">
                                {isUploading && <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>}
                                {error != null && <p className="error">{error}</p>}
                                {!isUploading && <button className="btn btn-primary" onClick={() => {
                                    setShowingUpload(false);
                                    setError(null);
                                    setCurrentTab(0);}}>
                                    Закрыть окно</button>}
                            </div>
                        </div>
                        : <ConfirmationModal title="Опубликовать тест?" onCancel={() => setCurrentTab(0)}
                                                 onConfirm={uploadTest}/>
                    }
                </>;
        }
    }
    
    return (<>
        <TestAside tabs={tabs} currentTab={currentTab} setCurrentTab={setCurrentTab}/>
        <TabSwitch/>
    </>)
}

class CreateOrUpdateTestCommand {
    constructor(name, description, tags, questions, attemptLimit, timeLimit, results, password) {
        this.name = name;
        this.description = description;
        this.tags = tags;
        this.questions = questions;
        this.attemptLimit = attemptLimit;
        this.timeLimit = timeLimit;
        this.results = results;
        this.password = password;
    }
}

function createTest(command) {
    return fetch('/api/tests', {
        method: "POST",
        body: JSON.stringify(command),
        headers: {
            "Content-Type": "application/json"
        }
    });
}

function updateTest(command, id) {
    return fetch(`/api/tests/${id}`, {
        method: "PATCH",
        body: JSON.stringify(command),
        headers: {
            "Content-Type": "application/json"
        }
    });
}

function uploadTestImage(file, id) {
    const formData = new FormData();
    formData.append('file', file);

    return fetch(`/files/tests/${id}`, {
        method: 'PATCH',
        body: formData
    });
}

export default Constructor;