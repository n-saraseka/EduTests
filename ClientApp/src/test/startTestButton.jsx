import {useState} from "react";
import ConfirmationModal from "../modals/confirmationModal.jsx";

function StartTestButton({testId}) {
    const [isLoading, setIsLoading] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [completionId, setCompletionId] = useState(null);
    
    const handleStart = async () => {
        setIsLoading(true);
        const response = await getActiveCompletion(testId);
        if (response.ok) {
            if (response.status !== 204) {
                const activeCompletion = await response.json();
                setCompletionId(activeCompletion.id);
                setIsLoading(false);
                setIsModalOpen(true);
            }
            else {
                await startPlaythrough();
            }
        }
    }
    
    const createNewCompletion = async () => {
        const response = await createCompletion(testId);
        if (response.ok) {
            const newCompletion = await response.json();
            setCompletionId(newCompletion.id);
            return newCompletion.id;
        }
    }
    
    const redirectToPlaythrough = (id) => {
        window.location.href = `/test/${testId}/playthrough/${id}`;
    }
    
    const startPlaythrough = async () => {
        setIsLoading(true);
        const newId = await createNewCompletion();
        setIsLoading(false)
        redirectToPlaythrough(newId);
    }
    
    const restartPlaythrough = async () => {
        setIsLoading(true);
        console.log(testId);
        await deleteCompletion(testId, completionId);
        const newId = await createNewCompletion();
        setIsLoading(false)
        redirectToPlaythrough(newId);
    }
     
    return (<>
        <button id="start-test-button" className="btn btn-primary" onClick={handleStart}>Начать тест</button>
        {isLoading && (<div className="modal">
            <div className="modal-window">
                <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
            </div>
        </div>)}
        {isModalOpen && (<ConfirmationModal title="Вы уже начали прохождение этого теста. Хотите продолжить?"
                                            onCancel={restartPlaythrough}
                                            onConfirm={redirectToPlaythrough}/>)}
    </>);
}

function createCompletion(id) {
    return fetch(`/api/tests/${id}/completions`,{
        method: "POST",
    })
}

function getActiveCompletion(id) {
    return fetch(`/api/tests/${id}/completions/active`);
}

function deleteCompletion(testId, completionId) {
    return fetch(`/api/tests/${testId}/completions/${completionId}`, {
        method: "DELETE"
    });
}

export default StartTestButton;