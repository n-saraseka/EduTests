import ConstructorResult from "./constructorResult.jsx";

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

    return (<div className="constructor-tab">
        <h1>Настройки результатов</h1>
        <div className="test-card">
            <ConstructorResult isDefault={true} result={test.defaultResult} onChange={(updated) => updateDefaultResult(updated)}/>
            {test.results.map((r, i) => <ConstructorResult key={i} isDefault={false} result={r}
                                                      onChange={(updated) => updateResult(i, updated)}/>)}
            <button className="btn btn-primary" onClick={addResult}>Добавить результат</button>
        </div>
    </div>)
}

export default ResultTab;