function Pagination({ page, pageCount, onChangePage }) {
    const windowSize = 2;
    let firstPage = Math.max(1, page - windowSize);
    let lastPage = Math.min(pageCount, page + windowSize);

    const pages = Array.from(
        { length: lastPage - firstPage + 1},
        (_, i) => firstPage + i
    );

    return (
        <ul className="pagination">
            <li className="page-item" onClick={() => onChangePage(page-1)}>&lt;</li>
            {pages.map(p => (
                <li key={p} className={`page-item${p === page ? ' active' : ''}`} onClick={() => onChangePage(p)}>{p}</li>
            ))}
            <li className="page-item" onClick={() => onChangePage(page+1)}>&gt;</li>
        </ul>
    )
}

export default Pagination;