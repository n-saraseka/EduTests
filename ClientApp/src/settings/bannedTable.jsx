import {useState} from "react";
import Pagination from "../pagination.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";

function BanRow({baseBan, onDelete}) {
    let banDate = new Date(baseBan.banDate);
    banDate = banDate.toISOString().split("T")[0];
    let unbanDate = baseBan.unbanDate === null ? null : new Date(baseBan.unbanDate);
    unbanDate = unbanDate === null ? "Бессрочная" : unbanDate.toISOString().split("T")[0];
    
    return (
        <tr className="settings-row">
            <td><img src={`/files/users/${baseBan.bannedUser.id}`} alt="Аватар пользователя" className="table-icon"/></td>
            <td><a href={`/user/${baseBan.bannedUser.id}`}>{baseBan.bannedUser.username}</a></td>
            <td><img src={`/files/users/${baseBan.bannedByUser.id}`} alt="Аватар пользователя" className="table-icon"/></td>
            <td><a href={`/user/${baseBan.bannedByUser.id}`}>{baseBan.bannedByUser.username}</a></td>
            <td>{baseBan.banReason}</td>
            <td>{banDate}</td>
            <td>{unbanDate}</td>
            <td><DeleteButton entityType={"ban"} onDelete={onDelete}/></td>
        </tr>
    )
}

function BannedTable({baseBans, basePages, rowsPerPage}) {
    const [bans, setBans] = useState(baseBans);
    const [page, setPage] = useState(1);
    const [pageCount, setPageCount] = useState(basePages);
    const [active, setActive] = useState(true);
    const [isPosting, setIsPosting] = useState(false);
    const [isLoading, setIsLoading] = useState(false);

    const handlePageChange = async (newPage) => {
        if (isPosting || isLoading) return;

        const targetPage = Math.max(1, Math.min(newPage, pageCount));
        if (targetPage === page) return;

        setPage(targetPage);

        setIsLoading(true);
        const result = await getBans(targetPage, rowsPerPage, active);
        const newBans = await result.json();
        const pages = parseInt(newBans.pages);
        if (pages !== pageCount) {
            setPageCount(pages)
        }
        setBans(newBans.bans);
        setIsLoading(false);
    }

    const handleBanDeletion = async (banId) => {
        if (isPosting || isLoading) return;

        setIsPosting(true);
        const deleteResult = await deleteBan(banId);
        if (deleteResult.ok) {
            const result = await getBans(page, rowsPerPage, active);
            const newBans = await result.json();
            const pages = parseInt(newBans.pages);
            if (pages !== pageCount) {
                setPageCount(pages)
            }
            setBans(newBans.bans);
        }
        setIsLoading(false);
    }

    const filterBans = async (event) => {
        event.preventDefault();
        if (isPosting || isLoading) return;

        const newValue = event.target.checked;
        setActive(!active);
        setIsLoading(true);

        const result = await getBans(page, rowsPerPage, newValue);
        const newBans = await result.json();
        const pages = parseInt(newBans.pages);
        if (pages !== pageCount) {
            setPageCount(pages)
        }

        setBans(newBans.bans);
        setIsLoading(false);
    }

    return (<>
        <div>
            <label htmlFor="activeFilter">Показывать только активные блокировки: </label>
            <input type="checkbox" name="activeFilter" id="activeFilter" onChange={filterBans} checked={active}/>
        </div>
        {bans.length > 0 ? <>
            <table className="settings-table">
                <thead>
                <tr>
                    <td colSpan="2">Заблокированный пользователь</td>
                    <td colSpan="2">Заблокировавший пользователь</td>
                    <td>Причина блокировки</td>
                    <td>Дата блокировки</td>
                    <td>Дата снятия блокировки</td>
                    <td></td>
                </tr>
                </thead>
                <tbody>
                {isLoading ? <tr>
                        <td colSpan={8} className="loading">
                            <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
                        </td>
                    </tr>
                    : bans.map(ban => <BanRow key={ban.id} baseBan={ban} onDelete={() => handleBanDeletion(ban.id)}/>)}
                </tbody>
            </table>
            {basePages > 1 && <Pagination page={page} pageCount={pageCount} onChangePage={handlePageChange}/>}
            </>
            : <p>Нет блокировок... Ура!</p>
        }
    </>)
}

function getBans(page, amountPerPage, active) {
    return fetch(`/api/bans?page=${page}&amountPerPage=${amountPerPage}&active=${active}`);
}

function deleteBan(id) {
    return fetch(`/api/bans/${id}`, {
        method: 'DELETE',
    });
}

export default BannedTable;