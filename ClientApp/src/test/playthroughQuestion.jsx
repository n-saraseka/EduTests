import BbcodePreset from "../bbcodePreset.jsx";
import {useState} from "react";
import EditButton from "../buttons/editButton.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";

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

    function SequenceContainer({sequence}) {
        const changeSequence = (seq) => onChange({questionId: question.id, answer: {sequence: seq}});

        return (<>
            <div className="sequence-items">
                {sequence.map((e, i) =>
                    <SequenceItem key={i} baseText={e} index={i}
                                  sequence={sequence} onModification={changeSequence}/>)}
            </div>
        </>);
    }

    function SequenceItem({baseText, sequence, onModification, index}) {
        const [isDragged, setIsDragged] = useState(false);
        const [shadow, setShadow] = useState(null);

        const handleDragStart = (event) => {
            event.dataTransfer.setData('text/plain', JSON.stringify({
                draggedIndex: index,
                draggedText: baseText
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

        return (<div className={`sequence-item${isDragged ? ' dragged' : ''}`} draggable={true} data-index={index}
                     onDragStart={handleDragStart} onDragEnd={() => setIsDragged(false)}
                     onDragOver={handleDragOver} onDragLeave={handleDragLeave} onDrop={handleDrop}
                     style={{boxShadow: shadow}}>{baseText}</div>)
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
                    <SequenceContainer sequence={answer === undefined ? question.data.options : answer.answer.sequence}/>
                </div>)
        }
    }

    return (<div className="test-question">
        <BbcodePreset text={question.description}/>
        <QuestionSwitch type={question.type}/>
    </div>);
}

export default PlaythroughQuestion;