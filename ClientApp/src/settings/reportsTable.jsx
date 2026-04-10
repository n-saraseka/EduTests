import {useState} from "react";
import Pagination from "../pagination.jsx";

function ReportRow({baseReport, onStatusChange}) {
    const [status, setStatus] = useState(parseInt(baseReport.reportStatus));
    
    const changeStatus = async (event) => {
        event.preventDefault();
        setStatus(parseInt(event.target.value));
        await onStatusChange(event);
    }
    
    return (
        <tr className="settings-row">
            <td><a href={baseReport.reportedUser !== null ? `/user/${baseReport.reportedUser.id}` 
                : baseReport.reportedTest !== null ? `/test/${baseReport.reportedTest.id}` : 
                    `/comment/${baseReport.reportedComment.id}`}>
                {baseReport.reportedUser !== null ? `Пользователь ${baseReport.reportedUser.username}` 
                    : baseReport.reportedTest !== null ? `Тест ${baseReport.reportedTest.name}` 
                        : `Комментарий пользователя ${baseReport.reportedComment.user.username}`}
            </a></td>
            <td>{baseReport.reportText}</td>
            <td><select name="status" id={baseReport.id} defaultValue={baseReport.reportStatus} 
                        onChange={changeStatus}
                        disabled={status !== 0}>
                <option value="0">Не проверена</option>
                <option value="1">Принята</option>
                <option value="2">Отклонена</option>
            </select></td>
        </tr>
    )
}

function ReportsTable({baseReports, basePages, rowsPerPage}) {
    const [reports, setReports] = useState(baseReports);
    const [page, setPage] = useState(1);
    const [pageCount, setPageCount] = useState(basePages);
    const [statusFilter, setStatusFilter] = useState('');
    const [isPosting, setIsPosting] = useState(false);
    const [isLoading, setIsLoading] = useState(false);

    const handlePageChange = async (newPage) => {
        if (isPosting || isLoading) return;

        const targetPage = Math.max(1, Math.min(newPage, pageCount));
        if (targetPage === page) return;

        setPage(targetPage);

        setIsLoading(true);
        const result = await getReports(targetPage, rowsPerPage, statusFilter);
        const newReports = await result.json();
        const pages = parseInt(newReports.pages);
        if (pages !== pageCount) {
            setPageCount(pages)
        }
        setReports(newReports.reports);
        setIsLoading(false);
    }
    
    const handleReportStatusChange = async (event) => {
        if (isPosting || isLoading) return;
        
        const reportId = event.target.id;
        const newValue = event.target.value;
        const command = new ChangeReportStatusCommand(newValue);
        
        setIsPosting(true);
        const result = await changeReportStatus(reportId, command);
        setIsLoading(false);
    }
    
    const filterReports = async (event) => {
        event.preventDefault();
        if (isPosting || isLoading) return;
        
        const newValue = parseInt(event.target.value);
        if (newValue === statusFilter) return;
        setIsLoading(true);
        if (newValue === -1) {
            setStatusFilter('');
        }
        else {
            setStatusFilter(newValue);
        }

        const result = await getReports(page, rowsPerPage, newValue);
        const newReports = await result.json();
        const pages = parseInt(newReports.pages);
        if (pages !== pageCount) {
            setPageCount(pages)
        }

        setReports(newReports.reports);
        setIsLoading(false);
    }
    
    return (<>
        <div>
            <label htmlFor="statusFilter">Статус: </label>
            <select name="statusFilter" id="statusFilter" defaultValue="-1" onChange={filterReports}>
                <option value="-1">Все</option>
                <option value="0">Не проверена</option>
                <option value="1">Принята</option>
                <option value="2">Отклонена</option>
            </select>
        </div>
        {reports.length > 0 ? <>
            <table className="settings-table">
                <thead>
                <tr>
                    <td>Объект жалобы</td>
                    <td>Текст жалобы</td>
                    <td>Статус жалобы</td>
                </tr>
                </thead>
                <tbody>
                {isLoading ? <tr>
                        <td colSpan={3} className="loading">
                            <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
                        </td>
                    </tr>
                    : reports.map(report => <ReportRow key={report.id} baseReport={report} onStatusChange={handleReportStatusChange}/>)}
                </tbody>
            </table>
            {basePages > 1 && <Pagination page={page} pageCount={pageCount} onChangePage={handlePageChange}/>}
        </> : <p>Жалоб нет... Хорошо.</p>}
    </>)
}

function getReports(page, amountPerPage, status) {
    let reportStatus = ''
    
    switch (status) {
        case 0:
            reportStatus = 'Pending';
            break;
        case 1:
            reportStatus = 'Accepted';
            break;
        case 2:
            reportStatus = 'Rejected';
            break;
    }
    
    let query = `/api/report?page=${page}&amountPerPage=${amountPerPage}`
    if (reportStatus !== '') query += `&status=${reportStatus}`;
    
    return fetch(query);
}

class ChangeReportStatusCommand {
    constructor(reportStatus) {
        this.reportStatus = reportStatus;
    }
}

function changeReportStatus(id, data) {
    return fetch(`/api/report/${id}/status`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
        },
        body: JSON.stringify(data)
    });
}

export default ReportsTable;