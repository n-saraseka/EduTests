import BbcodePreset from "../bbcodePreset.jsx";

function ResultDetail({question, answer}) {
    function QuestionSwitch({type}) {
        switch(type) {
            // Single choice
            case 0:
                return (<div id="asnwers">
                    {question.data.options.map((option, index) => (<div className="test-card-row" key={index}>
                        <input type="radio" id={`option-${index}`} value={option} name="answer"
                               checked={answer.answer.chosenIndices.includes(index)}
                               disabled={true}/>
                        <label htmlFor={`option-${index}`} key={`label-${index}`}
                               style={{
                                   color: question.correctData.validIndices.includes(index) ? 'green' : 'red',
                                   fontWeight: answer.answer.chosenIndices.includes(index) ? 'bold' : 'normal'}}>{option}</label>
                    </div>))}
                </div>);
            // Multiple choice
            case 1:
                return (<div id="asnwers">
                    {question.data.options.map((option, index) => (<div className="test-card-row" key={index}>
                        <input type="checkbox" id={`option-${index}`} value={option} name="answer"
                               checked={answer.answer.chosenIndices.includes(index)}
                               disabled={true}/>
                        <label htmlFor={`option-${index}`} key={`label-${index}`}
                               style={{
                                   color: question.correctData.validIndices.includes(index) ? 'green' : 'red',
                                   fontWeight: answer.answer.chosenIndices.includes(index) ? 'bold' : 'normal'}}>{option}</label>
                    </div>))}
                </div>);
            // Number input
            case 2:
                return (<div id="asnwers">
                    <div className="test-card-row">
                        <span>Ответ: </span>
                        <span style={{
                            color: answer.answer.numberAnswer >= (question.correctData.numberAnswer - question.data.tolerance) &&
                                answer.answer.numberAnswer <= (question.correctData.numberAnswer + question.data.tolerance) ? 'green' : 'red'}}>
                            {answer.answer.numberAnswer}</span>
                        <span> (Правильный ответ: {question.correctData.numberAnswer})</span>
                    </div>
                </div>);
            // Text input
            case 3:
                return (<div id="answers">
                    <div className="test-card-row">
                        <span>Ответ: </span>
                        <span style={{
                            color: answer.answer.textAnswer === question.correctData.textAnswer ? 'green' : 'red'}}>
                            {answer.answer.textAnswer}</span>
                        <span> (Правильный ответ: {question.correctData.textAnswer})</span>
                    </div>
                </div>)
        }
    }
    
    return (<div className="test-card">
        <h2>Вопрос №{question.orderIndex}</h2>
        <div className="test-card-row">
            <BbcodePreset text={question.description}/>
        </div>
        <QuestionSwitch type={question.type}/>
    </div>);
}

export default ResultDetail;