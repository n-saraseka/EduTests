import ConstructorResult from "./constructorResult.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";

function ResultTab({test, setTest}){
    const updateResult = (index, updatedResult) => {
        const oldResult = test.results[index];
        const newThreshold = updatedResult.percentageThreshold;
        const existingResult = test.results.find(r => r.percentageThreshold === newThreshold);
        if (existingResult) {
            updatedResult.percentageThreshold = oldResult.percentageThreshold;
        }
        setTest(prevTest => ({...prevTest, results: prevTest.results.map((r, i) =>
                i === index? updatedResult : r)}));
    }
    
    const updateDefaultResult = (updatedResult) => {
        setTest(prevTest => ({...prevTest, defaultResult: updatedResult}));
    }
    
    const addResult = () => {
        const allThresholds = Array.from({length: 101}, (n, i) => i);
        const existingThresholds = test.results.map(r => r.percentageThreshold);
        const newThresholds = allThresholds.filter(t => !existingThresholds.includes(t));
        const minThreshold = Math.min.apply(null, newThresholds);
        
        const newResult = {
            percentageThreshold: minThreshold,
            result: ""
        }
        let results = test.results;
        results.push(newResult);
        setTest(prevTest => ({...prevTest, results: results}));
    }
    
    const removeResult = (index) => {
        const newResults = test.results.filter((r, i) => i !== index);
        setTest(prevTest => ({...prevTest, results: newResults}));
    }

    return (<div className="constructor-tab">
        <h1>Настройки результатов</h1>
        <div className="test-card">
            <ConstructorResult isDefault={true} result={test.defaultResult} onChange={(updated) => updateDefaultResult(updated)}/>
            {test.results.map((r, i) => <div className="test-card-row" key={i}>
                <ConstructorResult isDefault={false} result={r}
                                   onChange={(updated) => updateResult(i, updated)}/>
                <DeleteButton entityType="result" onDelete={() => removeResult(i)}/>
            </div>)}
            <button className="btn btn-primary" onClick={addResult}>Добавить результат</button>
        </div>
    </div>)
}

export default ResultTab;