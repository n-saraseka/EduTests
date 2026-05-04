import {useState} from "react";
import Pagination from "../pagination.jsx";
import DownloadXlsxButton from "./downloadXlsxButton.jsx";
import CompletionRow from "./completionRow.jsx";

function IndividualStats({test, questions, baseCompletions, version, basePages, rowsPerPage}) {
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
        const result = await getCompletions(test.id, newPage, rowsPerPage, version);
        const newCompletions = await result.json();
        const pages = parseInt(newCompletions.pages);
        if (pages !== pageCount) {
            setPageCount(pages)
        }
        setCompletions(newCompletions.completions);
        setIsLoading(false);
    }

    return (<div id="individual-stats">
        <div id="table-settings">
            <DownloadXlsxButton questions={questions} test={test} version={version}/>
        </div>
        <table>
            <thead>
            <tr>
                <td>Пользователь</td>
                <td>Начат</td>
                <td>Пройден</td>
                <td>Количество верных ответов</td>
                <td>Процент верных ответов</td>
                <td></td>
            </tr>
            </thead>
            <tbody>
            {isLoading ? <tr>
                    <td colSpan={6} className="loading">
                        <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
                    </td>
                </tr>
                : completions.map(completion => <CompletionRow key={completion.id} baseCompletion={completion}/>)}
            </tbody>
        </table>
        {basePages > 1 && <Pagination page={page} pageCount={pageCount} onChangePage={handlePageChange}/>}
    </div>)
}

function getCompletions(testId, page, amountPerPage, version) {
    return fetch(`/api/tests/${testId}/completions/finished?page=${page}&amountPerPage=${amountPerPage}&version=${version}`);
}

export default IndividualStats;