import TestTag from "./testTag.jsx";

function TestCard({test}) {
    return <>
        <img src={`/files/tests/${test.id}`} alt="Изображение теста" className="card-image"/>
        <strong className="test-name">{test.name}</strong>
        {(test.description !== null && test.description.length > 0) && 
            <p className="test-description">{test.description}</p>}
        <ul className="test-tags">
            {test.tags.map(tag => (<TestTag key={tag} name={tag}/>))}
        </ul>
        <div className="author-ratings">
            <div className="author-column">
                <img src={`/files/users/${test.user.id}`} alt="Автор"/>
                <a href={`/user/${test.user.id}`}>{test.user.username}</a>
            </div>
            <div className="ratings-completions-column">
                <div className="stat-row">
                    <img src="/files/icons/rating.png" alt="Рейтинг"/>
                    <span>{test.rating}</span>
                </div>
                <div className="stat-row">
                    <img src="/files/icons/completions.png" alt="Прохождения"/>
                    <span>{test.completionCount}</span>
                </div>
            </div>
        </div>
        <button className="btn btn-primary" onClick={() => window.location.href = `/test/${test.id}`}>Пройти тест</button>
    </>
}

export default TestCard;