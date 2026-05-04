import {useState} from "react";
import GeneralStats from "./generalStats.jsx";
import IndividualStats from "./individualStats.jsx";

function Stats({test, baseQuestions, baseCompletions, baseGenStats, versions, basePages, rowsPerPage}) {
    const [questions, setQuestions] = useState(baseQuestions);
    const [completions, setCompletions] = useState(baseCompletions);
    const [genStats, setGenStats] = useState(baseGenStats);
    const [version, setVersion] = useState(versions[0]);
    const [pageCount, setPageCount] = useState(basePages);
    const [isLoading, setIsLoading] = useState(false);

    const handleVersionChange = async (event) => {
        event.preventDefault();
        const newVersion = event.target.value;
        if (newVersion !== version) {
            setIsLoading(true);
            const result = await getCompletions(test.id, 1, rowsPerPage, newVersion);
            if (result.ok) {
                const json = await result.json();
                setVersion(newVersion);
                setCompletions(json.completions);
                const pages = parseInt(json.pages);
                if (pages !== pageCount) {
                    setPageCount(pages)
                }
                const questionResult = await getQuestions(test.id, newVersion);
                if (questionResult.ok) {
                    const json = await questionResult.json();
                    console.log(json);
                    setQuestions(json);
                }
                const statsResult = await getStats(test.id, newVersion);
                if (statsResult.ok) {
                    const json = await statsResult.json();
                    console.log(json);
                    setGenStats(json);
                }
            }
            setIsLoading(false);
        }
    }
    
    return (
        <>
            <div id="row">
                <label htmlFor="versions">Версия теста: </label>
                <select onChange={handleVersionChange} value={version} id="versions">
                    {versions.map((v, i) => <option value={v} key={i}>{dateStringToDate(v)}</option>)}
                </select>
            </div>
            {isLoading 
                ? <div className="loading">
                    <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
            </div> 
                : <>
                    {completions.length > 0 
                        ? <>
                            <div className="text-center"><h1>Общая статистика</h1></div>
                            <GeneralStats data={genStats}/>
                            <div className="text-center"><h1>Индивидуальная статистика</h1></div>
                            <IndividualStats test={test} questions={questions} basePages={pageCount} rowsPerPage={rowsPerPage}
                                             baseCompletions={completions} version={version}/>
                        </> 
                        : <p>Этот тест не был пройден ни одним пользователем на этой версии.</p>}
                    
                </>}
        </>
    )
}

function getCompletions(testId, page, amountPerPage, version) {
    return fetch(`/api/tests/${testId}/completions/finished?page=${page}&amountPerPage=${amountPerPage}&version=${version}`);
}

function getQuestions(testId, version) {
    return fetch(`/api/tests/${testId}/questions?version=${version}`);
}

function getStats(testId, version) {
    return fetch(`/api/tests/${testId}/completion_stats?version=${version}`);
}

function dateStringToDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString();
}

export default Stats;