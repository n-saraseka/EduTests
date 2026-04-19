import {useState} from "react";
import PlaythroughQuestion from "./playthroughQuestion.jsx";

function TestPlaythrough({baseQuestions, baseAnswers, baseLastUnanswered, baseTest}) {
    const [answers, setAnswers] = useState(baseAnswers);
    const [currentQuestion, setCurrentQuestion] = useState(getQuestionByOrderIndex(baseLastUnanswered));
    const [currentAnswer, setCurrentAnswer] = useState(getAnswerByQuestionId(currentQuestion.id));
    const [currentCompletionPercentage, setCurrentCompletionPercentage] = useState(currentQuestion.orderIndex / baseQuestions.length * 100);
    
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
    
    function changeQuestion(index) {
        if (index !== currentQuestion.orderIndex) {
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
                    <li className="page-item" onClick={() => currentQuestion.orderIndex !== baseQuestions.length &&
                        changeQuestion(currentQuestion.orderIndex + 1)}>
                        {currentQuestion.orderIndex !== baseQuestions.length ? "Продолжить" : "Завершить тест"}
                    </li>
                </ul>
            </div>
        </div>
        <div id="progress-bar"
             style={{background: `linear-gradient(to right, #4f98ff, #4f98ff ${currentCompletionPercentage}%, #fcfcfc ${currentCompletionPercentage}%, #fcfcfc 100%)`}}></div>
    </>)
}

export default TestPlaythrough;