import {useState} from "react";
import TestCard from "../testList/testCard.jsx";
import Pagination from "../pagination.jsx";

function ProfileTests({baseTests, basePages, pageSize, userId}) {
    const [tests, setTests] = useState(baseTests);
    const [page, setPage] = useState(1);
    const [pages, setPages] = useState(basePages);
    const [currentSort, setCurrentSort] = useState(0);
    const [isLoading, setIsLoading] = useState(false);
    
    const changePage = async (page) => {
        setIsLoading(true);
        const response = await getTests(page, pageSize);
        const json = await response.json();
        setIsLoading(false);
        const newTests = json.tests;
        setTests(newTests);
        setPage(page);
    }
    
    const changeSort = async (event) => {
        const newSort = parseInt(event.target.value);
        if (newSort !== currentSort) {
            setIsLoading(true);
            const testsResponse = await getTests(1, pageSize, userId, newSort);
            if (testsResponse.ok) {
                const json = await testsResponse.json();
                setIsLoading(false);
                const newTests = json.tests;
                setTests(newTests);
                setPages(json.pages);
                setPage(1);
                setCurrentSort(newSort);
            }
            else {
                setIsLoading(false);
            }
        }
    }
    
    return (<>
        <div id="sort-options">
            <label htmlFor="sort">Сортировка:</label>
            <select name="sort" id="sort" value={currentSort} onChange={changeSort}>
                <option value="0">По дате создания</option>
                <option value="1">По названию</option>
                <option value="2">По оценке</option>
                <option value="3">По прохождениям</option>
            </select>
        </div>
        {isLoading 
            ? <div className="loading">
            <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
        </div> 
            : <ul id="test-cards">
            {tests.map((test, index) => (
                <li key={index} className="card">
                    <TestCard test={test}/>
                </li>
            ))}
        </ul>}
        {pages > 1 && <Pagination page={page} pageCount={pages} onChangePage={changePage}/>} 
    </>)
}

function getTests(page, pageSize, userId, sort) {
    return fetch(`/api/tests?page=${page}&amountPerPage=${pageSize}&userId=${userId}&sort=${sort}&isDescending=true&isProfile=true`);
}

export default ProfileTests;