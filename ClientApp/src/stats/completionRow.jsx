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
            <td><a href={`/testplaythrough/${baseCompletion.id}/details`}>Просмотр подробностей</a></td>
        </tr>
    )
}

function parseDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString().replace(",", "");
}

export default CompletionRow;