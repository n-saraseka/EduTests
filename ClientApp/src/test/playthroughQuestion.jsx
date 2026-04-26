import BBCode from '@bbob/react';
import { createPreset } from '@bbob/preset';
import presetReact from "@bbob/preset-react";

function PlaythroughQuestion({question, answer, onChange}) {
    const allowedTags = ['b', 'i', 'img', 'center', 'u', 'color', 's', 'quote', 'url', 'ul', 'ol', 'li', 'table', 'tr', 'td', 'th'];
    const preset = presetReact.extend((tags, options) => ({
        ...tags,
        center: (node) => ({
            tag: "span",
            content: node.content,
            attrs: {style: {textAlign: "center"}}
        })
    }))();
    const plugins = [preset];
    const editChoiceData = (event, index, type) => {
        switch (type) {
            // Single choice
            case 0:
                onChange({questionId: question.id, data: {chosenIndices: [index]}});
                break;
            // Multiple choice
            case 1:
                let indices = answer.data.chosenIndices;
                if (event.target.checked) {
                    indices.push(index);
                }
                else {
                    indices = indices.filter(i => i !== index);
                }
                onChange({questionId: question.id, data: {validIndices: indices}});
                break;
        }
    }
    
    const editNumberAnswer = (event) => {
        const newAnswer = parseInt(event.target.value);
        if (newAnswer !== answer.data.numberAnswer) {
            onChange({questionId: question.id, data: {numberAnswer: newAnswer}});
        }
    }

    const editTextAnswer = (event) => {
        const newAnswer = event.target.value;
        if (newAnswer !== answer.data.textAnswer) {
            onChange({questionId: question.id, data: {textAnswer: newAnswer}});
        }
    }

    function QuestionSwitch({type}) {
        switch(type) {
            // Single choice
            case 0:
                return (<div id="asnwers">
                        {question.data.options.map((option, index) => (<div className="test-card-row" key={index}>
                            <input type="radio" id={`option-${index}`} value={option} name="answer"
                                   checked={answer === undefined ? false : answer.data.chosenIndices.includes(index)}
                                   onChange={(e) => editChoiceData(e, index, type)}/>
                            <label htmlFor={`option-${index}`} key={`label-${index}`} >{option}</label>
                        </div>))}
                    </div>);
            // Multiple choice
            case 1:
                return (<div id="asnwers">
                    {question.data.options.map((option, index) => (<div className="test-card-row" key={index}>
                        <input type="checkbox" id={`option-${index}`} value={option} name="answer"
                               checked={answer === undefined ? false : answer.data.chosenIndices.includes(index)}
                               onChange={(e) => editChoiceData(e, index, type)}/>
                        <label htmlFor={`option-${index}`} key={`label-${index}`} >{option}</label>
                    </div>))}
                </div>);
            // Number input
            case 2:
                return (<div id="asnwers">
                    <div className="test-card-row">
                        <label htmlFor="answer">Правильный ответ: </label>
                        <input type="number" id="answer" name="answer" defaultValue={answer === undefined ? "" : answer.data.numberAnswer}
                               onChange={editNumberAnswer}/>
                    </div>
                </div>);
            // Text input
            case 3:
                return (<div id="answers">
                    <input type="text" id="answer" name="answer" defaultValue={answer === undefined ? "" : answer.data.textAnswer}
                           onChange={editTextAnswer}/>
                </div>)
        }
    }

    return (<div className="test-question">
        <BBCode plugins={plugins} options={{onlyAllowTags: allowedTags}} >
            {question.description}
        </BBCode>
        <QuestionSwitch type={question.type}/>
    </div>);
}

export default PlaythroughQuestion;