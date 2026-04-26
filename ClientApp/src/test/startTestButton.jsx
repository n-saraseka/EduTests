import {useState} from "react";
import ConfirmationModal from "../modals/confirmationModal.jsx";
import ConfirmationModalWithPassword from "../modals/confirmationModalWithPassword.jsx";

function StartTestButton({testId, hasPassword}) {
    const [isLoading, setIsLoading] = useState(false);
    const [isPasswordModalOpen, setIsPasswordModalOpen] = useState(false);
    const [isContinueModalOpen, setIsContinueModalOpen] = useState(false);
    const [completionId, setCompletionId] = useState(null);
    const [successfulVerification, setSuccessfulVerification] = useState(hasPassword);
    const [error, setError] = useState(null);
    
    console.log(successfulVerification);
    
    const handleStart = async () => {
        if (!successfulVerification) {
            setIsPasswordModalOpen(true);
        }
        else {
            setIsLoading(true);
            const response = await getActiveCompletion(testId);
            if (response.ok) {
                if (response.status !== 204) {
                    const activeCompletion = await response.json();
                    setCompletionId(activeCompletion.id);
                    setIsLoading(false);
                    setIsContinueModalOpen(true);
                }
                else {
                    await startPlaythrough();
                }
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
        await deleteCompletion(testId, completionId);
        const newId = await createNewCompletion();
        setIsLoading(false)
        redirectToPlaythrough(newId);
    }
    
    const verifyPassword = async (pass) => {
        setIsLoading(true);
        const command = new VerifyPasswordCommand(pass);
        const response = await passwordVerification(testId, command);
        if (response.ok) {
            setIsPasswordModalOpen(false);
            setSuccessfulVerification(true);
            setIsLoading(true);
            const response = await getActiveCompletion(testId);
            if (response.ok) {
                if (response.status !== 204) {
                    const activeCompletion = await response.json();
                    setCompletionId(activeCompletion.id);
                    setIsLoading(false);
                    setIsContinueModalOpen(true);
                }
                else {
                    await startPlaythrough();
                }
            }
        }
        else {
            setError("Неверный пароль");
        }
    }
    
    const redirectToHome = () => {
        window.location.href = `/`;
    }
     
    return (<>
        <button id="start-test-button" className="btn btn-primary" onClick={handleStart}>Начать тест</button>
        {isLoading && (<div className="modal">
            <div className="modal-window">
                <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
            </div>
        </div>)}
        {isContinueModalOpen && (<ConfirmationModal title="Вы уже начали прохождение этого теста. Хотите продолжить?"
                                                    onCancel={restartPlaythrough}
                                                    onConfirm={() => redirectToPlaythrough(completionId)}/>)}
        {isPasswordModalOpen && (<ConfirmationModalWithPassword title="Введите пароль для того, чтобы начать тест." onConfirm={verifyPassword}
                                                                onCancel={redirectToHome} passwordError={error}/>)}
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

class VerifyPasswordCommand {
    constructor(password) {
        this.password = password;
    }

}

function passwordVerification(testId, command) {
    return fetch(`/api/tests/${testId}/verify_password`, {
        method: "POST",
        body: JSON.stringify(command),
        headers: {
            "Content-Type": "application/json"
        }
    });
}

export default StartTestButton;