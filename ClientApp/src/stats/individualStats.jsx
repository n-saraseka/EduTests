import {useState} from "react";
import Pagination from "../pagination.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";

function parseDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString().replace(",", "");
}

function CompletionRow({baseCompletion}) {
    return (
        <tr>
            <td>{baseCompletion.userId === null ? 
                <span>Аноним</span>
                : <a href={`/user/${baseCompletion.userId}`}>{baseCompletion.user.username}</a>}</td>
            <td>{parseDate(baseCompletion.startedAt)}</td>
            <td>{parseDate(baseCompletion.completedAt)}</td>
            <td>{baseCompletion.correctAnswers.length}</td>
            <td>{baseCompletion.completionPercentage}</td>
        </tr>
    )
}

function IndividualStats({testId, baseCompletions, basePages, rowsPerPage}) {
    const [completions, setCompletions] = useState(baseCompletions);
    const [page, setPage] = useState(1);
    const [pageCount, setPageCount] = useState(basePages);
    const [isLoading, setIsLoading] = useState(false);

    const handlePageChange = async (newPage) => {
        if (isLoading) return;

        const targetPage = Math.max(1, Math.min(newPage, pageCount));
        if (targetPage === page) return;

        setPage(targetPage);

        setIsLoading(true);
        const result = await getCompletions(testId, newPage, rowsPerPage);
        const newCompletions = await result.json();
        const pages = parseInt(newCompletions.pages);
        if (pages !== pageCount) {
            setPageCount(pages)
        }
        setCompletions(newCompletions.completions);
        setIsLoading(false);
    }

    return (<>
        <table>
            <thead>
            <tr>
                <td>Пользователь</td>
                <td>Начат</td>
                <td>Пройден</td>
                <td>Количество верных ответов</td>
                <td>Процент верных ответов</td>
            </tr>
            </thead>
            <tbody>
            {isLoading ? <tr>
                    <td colSpan={5} className="loading">
                        <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
                    </td>
                </tr>
                : completions.map(completion => <CompletionRow key={completion.id} baseCompletion={completion}/>)}
            </tbody>
        </table>
        {basePages > 1 && <Pagination page={page} pageCount={pageCount} onChangePage={handlePageChange}/>}
    </>)
}

function getCompletions(testId, page, amountPerPage) {
    return fetch(`/api/tests/${testId}/completions/finished?page=${page}&amountPerPage=${amountPerPage}`);
}

export default IndividualStats;