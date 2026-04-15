import {useState} from "react";
import TestCard from "../testList/testCard.jsx";
import Pagination from "../pagination.jsx";

function ProfileTests({baseTests, basePages, pageSize}) {
    const [tests, setTests] = useState(baseTests);
    const [page, setPage] = useState(1);
    const [pages, setPages] = useState(basePages);
    
    const changePage = async (page) => {
        const response = await getTests(page, pageSize);
        const json = await response.json();
        const newTests = json.tests;
        setTests(newTests);
        setPage(page);
    }
    
    return (<>
        <ul id="test-cards">
        {tests.map((test, index) => (
            <li key={index} className="card">
                <TestCard test={test}/>
            </li>
        ))}
        </ul>
        {pages > 1 && <Pagination page={page} pageCount={pages} onChangePage={changePage}/>} 
    </>)
}

function getTests(page, pageSize) {
    return fetch(`api/tests?page=${page}&amountPerPage=${pageSize}`);
}

export default ProfileTests;