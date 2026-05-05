import BbcodePreset from "../bbcodePreset.jsx";
import {useState} from "react";
import EditButton from "../buttons/editButton.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";
import PlaythroughSequenceContainer from "./playthroughSequenceContainer.jsx";
import PlaythroughMatchPairsContainer from "./playthroughMatchPairsContainer.jsx";

function PlaythroughQuestion({question, answer, onChange}) {
    const editChoiceData = (event, index, type) => {
        switch (type) {
            // Single choice
            case 0:
                onChange({questionId: question.id, answer: {chosenIndices: [index]}});
                break;
            // Multiple choice
            case 1:
                let indices = answer.answer.chosenIndices;
                if (event.target.checked) {
                    indices.push(index);
                }
                else {
                    indices = indices.filter(i => i !== index);
                }
                onChange({questionId: question.id, answer: {validIndices: indices}});
                break;
        }
    }
    
    const editNumberAnswer = (event) => {
        const newAnswer = parseInt(event.target.value);
        if (newAnswer !== answer.answer.numberAnswer) {
            onChange({questionId: question.id, data: {numberAnswer: newAnswer}});
        }
    }

    const editTextAnswer = (event) => {
        const newAnswer = event.target.value;
        if (newAnswer !== answer.answer.textAnswer) {
            onChange({questionId: question.id, answer: {textAnswer: newAnswer}});
        }
    }

    function QuestionSwitch({type}) {
        switch(type) {
            // Single choice
            case 0:
                return (<div id="asnwers">
                        {question.data.options.map((option, index) => (<div className="test-card-row" key={index}>
                            <input type="radio" id={`option-${index}`} value={option} name="answer"
                                   checked={answer === undefined ? false : answer.answer.chosenIndices.includes(index)}
                                   onChange={(e) => editChoiceData(e, index, type)}/>
                            <label htmlFor={`option-${index}`} key={`label-${index}`} >{option}</label>
                        </div>))}
                    </div>);
            // Multiple choice
            case 1:
                return (<div id="asnwers">
                    {question.data.options.map((option, index) => (<div className="test-card-row" key={index}>
                        <input type="checkbox" id={`option-${index}`} value={option} name="answer"
                               checked={answer === undefined ? false : answer.answer.chosenIndices.includes(index)}
                               onChange={(e) => editChoiceData(e, index, type)}/>
                        <label htmlFor={`option-${index}`} key={`label-${index}`} >{option}</label>
                    </div>))}
                </div>);
            // Number input
            case 2:
                return (<div id="asnwers">
                    <div className="test-card-row">
                        <label htmlFor="answer">Правильный ответ: </label>
                        <input type="number" id="answer" name="answer" defaultValue={answer === undefined ? "" : answer.answer.numberAnswer}
                               onChange={editNumberAnswer}/>
                    </div>
                </div>);
            // Text input
            case 3:
                return (<div id="answers">
                    <input type="text" id="answer" name="answer" defaultValue={answer === undefined ? "" : answer.answer.textAnswer}
                           onChange={editTextAnswer}/>
                </div>)
            // Sequence
            case 4:
                return (<div id="asnwers">
                    <PlaythroughSequenceContainer question={question} onChange={onChange} 
                                                  sequence={answer === undefined ? question.data.options : answer.answer.sequence}/>
                </div>)
            case 5:
                return (<div id="asnwers">
                    <PlaythroughMatchPairsContainer question={question} answer={answer} onChange={onChange}/>
                </div>)
        }
    }

    return (<div className="test-question">
        <BbcodePreset text={question.description}/>
        <QuestionSwitch type={question.type}/>
    </div>);
}

export default PlaythroughQuestion;