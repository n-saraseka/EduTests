import {useState} from "react";
import PlaythroughQuestion from "./playthroughQuestion.jsx";

function TestPlaythrough({baseQuestions, baseAnswers, baseLastUnanswered, baseTest, completion}) {
    const [answers, setAnswers] = useState(baseAnswers);
    const [currentQuestion, setCurrentQuestion] = useState(getQuestionByOrderIndex(baseLastUnanswered));
    const [currentAnswer, setCurrentAnswer] = useState(getAnswerByQuestionId(currentQuestion.id));
    const [currentCompletionPercentage, setCurrentCompletionPercentage] = useState(currentQuestion.orderIndex / baseQuestions.length * 100);
    
    console.log(currentQuestion);
    
    const updateAnswer = (updatedAnswer) => {
        const existingAnswer = getAnswerByQuestionId(updatedAnswer.questionId);
        if (existingAnswer === undefined) {
            setAnswers({...answers, updatedAnswer});
        }
        else {
            const newAnswers = answers.map(a => a.questionId === updatedAnswer.questionId ? updatedAnswer : a);
            setAnswers(newAnswers);
        }
        setCurrentAnswer(updatedAnswer);
    }
    
    function getQuestionByOrderIndex(index) {
        return baseQuestions.find(q => q.orderIndex === index);
    }
    
    function getAnswerByQuestionId(questionId) {
        return baseAnswers.find(a => a.questionId === questionId);
    }
    
    const changeQuestion = async (index) => {
        if (index !== currentQuestion.orderIndex) {
            await submitAnswer();
            if (index < 1) {
                setCurrentQuestion(getQuestionByOrderIndex(1))
            }
            else if (index > baseQuestions.length) {
                setCurrentQuestion(getQuestionByOrderIndex(baseQuestions.length))
            }
            else {
                setCurrentQuestion(getQuestionByOrderIndex(index))
            }
            setCurrentCompletionPercentage(currentQuestion.orderIndex / baseQuestions.length * 100);
            setCurrentAnswer(getAnswerByQuestionId(currentQuestion.id));
        }
    }
    
    const submitAnswer = async () => {
        let newAnswer;
        if (currentAnswer === undefined) {
            switch (currentQuestion.type) {
                // Sequence
                case 4:
                    newAnswer = {questionId: currentQuestion.id, answer: {sequence: currentQuestion.data.options}};
                    break;
                // Match pairs
                case 5:
                    newAnswer = {questionId: currentQuestion.id, answer: {
                        pairs: currentQuestion.data.leftColumn.map((p, i) => {
                            return {left: currentQuestion.data.leftColumn[i], right: currentQuestion.data.rightColumn[i]};
                        })}};
                    break;
                default:
                    return;
            }
        }
        else newAnswer = currentAnswer;
        const answersResponse = await getAnswers(completion.testId, completion.id);
        if (answersResponse.ok) {
            const answersJson = await answersResponse.json();
            const existingAnswer = answersJson.find(a => a.questionId === currentAnswer.questionId);
            if (existingAnswer === undefined) {
                const command = new AddAnswerCommand(newAnswer.questionId, newAnswer.answer);
                const addResponse = await addAnswer(completion.testId, completion.id, command);
                if (addResponse.ok) {
                    const answer = await addResponse.json();
                    const answerWithId = {...currentAnswer, id: answer.id};
                }
            }
            else {
                const command = new EditTestAnswerCommand(newAnswer.answer);
                const editResponse = await editAnswer(completion.testId, completion.id, newAnswer.id, command);
            }
        }
    }
    
    const finishTest = async () => {
        await submitAnswer();
        await finishCompletion(completion.testId, completion.id);
        window.location.href = `/testplaythrough/${completion.id}/result`;
    }
    
    return (<>
        <div id="test-info">
            <img src={`/files/tests/${baseTest.id}`} alt="Изображение теста"/>
                <span>Прохождение теста <strong>
                    <a href={`/test/${baseTest.id}`}>{baseTest.name}</a>
                </strong></span>
        </div>
        <div id="test">
            <strong id="test-progress">Вопрос {currentQuestion.orderIndex} из {baseQuestions.length}</strong>
            <div id="question-controls">
                <PlaythroughQuestion question={currentQuestion} answer={currentAnswer}
                                     onChange={(answer) => updateAnswer(answer)}/>
                <ul className="pagination">
                    {currentQuestion.orderIndex !== 1 &&
                        <li className="page-item"
                            onClick={() => changeQuestion(currentQuestion.orderIndex - 1)}>Назад</li>}
                    <li className="page-item" onClick={() => currentQuestion.orderIndex !== baseQuestions.length ?
                        changeQuestion(currentQuestion.orderIndex + 1) : finishTest()}>
                        {currentQuestion.orderIndex !== baseQuestions.length ? "Продолжить" : "Завершить тест"}
                    </li>
                </ul>
            </div>
        </div>
        <div id="progress-bar"
             style={{background: `linear-gradient(to right, #4f98ff, #4f98ff ${currentCompletionPercentage}%, #fcfcfc ${currentCompletionPercentage}%, #fcfcfc 100%)`}}></div>
    </>)
}

class AddAnswerCommand {
    constructor(questionId, answer) {
        this.questionId = questionId;
        this.answer = answer;
    }
}

class EditTestAnswerCommand {
    constructor(newAnswer) {
        this.newAnswer = newAnswer;
    }
}

function finishCompletion(testId, completionId) {
    return fetch(`/api/tests/${testId}/completions/${completionId}`, {
        method: "PATCH"
    });
}

function getAnswers(testId, completionId) {
    return fetch (`/api/tests/${testId}/completions/${completionId}/answers`);
}

function addAnswer(testId, completionId, command) {
    return fetch (`/api/tests/${testId}/completions/${completionId}/answers`, {
        method: "POST",
        body: JSON.stringify(command),
        headers: {
            "Content-Type": "application/json"
        }
    });
}

function editAnswer(testId, completionId, answerId, command) {
    return fetch (`/api/tests/${testId}/completions/${completionId}/answers/${answerId}`, {
        method: "PATCH",
        body: JSON.stringify(command),
        headers: {
            "Content-Type": "application/json"
        }
    });
}

export default TestPlaythrough;