import {useState} from "react";
import Pagination from "../pagination.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";

function TestRow({baseTest, onDelete}) {
    return (
        <tr className="settings-row">
            <td colSpan={4}><a href={`/test/${baseTest.id}`}>{baseTest.name}</a></td>
            <td><a href={`/test/${baseTest.id}/constructor`}>
                <img src="/files/icons/edit.png" alt="Конструктор" className="edit-icon"/>
            </a></td>
            <td>
                <a href={`/test/${baseTest.id}/statistics`}>
                    <img src="/files/icons/stats.png" alt="Статистика" className="edit-icon"/>
                </a>
            </td>
            <td><DeleteButton entityType={"test"} onDelete={onDelete}/></td>
        </tr>
    )
}

function TestsTable({userId, baseTests, basePages, rowsPerPage}) {
    const [tests, setTests] = useState(baseTests);
    const [page, setPage] = useState(1);
    const [pageCount, setPageCount] = useState(basePages);
    const [isLoading, setIsLoading] = useState(false);
    const constructorUrl = "/constructor";

    const handlePageChange = async (newPage) => {
        if (isLoading) return;

        const targetPage = Math.max(1, Math.min(newPage, pageCount));
        if (targetPage === page) return;

        setPage(targetPage);

        setIsLoading(true);
        const result = await getTests(userId, newPage, rowsPerPage);
        const newTests = await result.json();
        const pages = parseInt(newTests.pages);
        if (pages !== pageCount) {
            setPageCount(pages)
        }
        setTests(newTests.tests);
        setIsLoading(false);
    }

    const handleTestDeletion = async (testId) => {
        if (isLoading) return;

        setIsLoading(true);
        const deleteResult = await deleteTest(testId);
        if (deleteResult.ok) {
            const result = await getTests(userId, page, rowsPerPage);
            const newTests = await result.json();
            const pages = parseInt(newTests.pages);
            if (pages !== pageCount) {
                setPageCount(pages)
            }
            setTests(newTests.tests);
        }
        setIsLoading(false);
    }

    return (<>
        {tests.length > 0 ? <>
                <table className="settings-table">
                    <thead>
                    <tr>
                        <td colSpan="4">Тест</td>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                    </thead>
                    <tbody>
                    {isLoading ? <tr>
                            <td colSpan={7} className="loading">
                                <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
                            </td>
                        </tr>
                        : tests.map(test => <TestRow key={test.id} baseTest={test} onDelete={() => handleTestDeletion(test.id)}/>)}
                    </tbody>
                </table>
                {basePages > 1 && <Pagination page={page} pageCount={pageCount} onChangePage={handlePageChange}/>}
            </>
            : <>
                <p>Вы не создали ни одного теста.</p>
                <button className="btn btn-primary" onClick={() => window.location.href = constructorUrl}>Перейти в конструктор</button>
            </>
        }
    </>)
}

function getTests(page, pageSize, userId) {
    return fetch(`/api/tests?page=${page}&amountPerPage=${pageSize}&userId=${userId}&isProfile=false`);
}

function deleteTest(id) {
    return fetch(`/api/tests/${id}`, {
        method: 'DELETE',
    });
}

export default TestsTable;