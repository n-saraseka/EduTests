import ConstructorResult from "./constructorResult.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";

function ResultTab({test, setTest}){
    const updateResult = (index, updatedResult) => {
        setTest(prevTest => ({...prevTest, results: prevTest.results.map((r, i) =>
                i === index? updatedResult : r)}));
    }
    
    const updateDefaultResult = (updatedResult) => {
        setTest(prevTest => ({...prevTest, defaultResult: updatedResult}));
    }
    
    const addResult = () => {
        const newResult = {
            percentageThreshold: 100,
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
            {test.results.map((r, i) => <div className="test-card-row">
                <ConstructorResult key={i} isDefault={false} result={r}
                                   onChange={(updated) => updateResult(i, updated)}/>
                <DeleteButton entityType="result" onDelete={() => removeResult(i)}/>
            </div>)}
            <button className="btn btn-primary" onClick={addResult}>Добавить результат</button>
        </div>
    </div>)
}

export default ResultTab;