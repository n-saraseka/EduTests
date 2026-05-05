import PlaythroughMatchPairsRow from "./playthroughMatchPairsRow.jsx";

function PlaythroughMatchPairsContainer({question, answer, onChange}) {
    return (<>
        <table>
            <thead>
            <tr>
                <th>Левый столбец</th>
                <th>Правый столбец</th>
            </tr>
            </thead>
            <tbody>
            {answer !== undefined 
                ? answer.answer.pairs.map((pair, i) => 
                    (<PlaythroughMatchPairsRow question={question} onChange={onChange} key={i} leftColumn={pair.left} 
                                           rightColumn={pair.right} index={i}/>)) : 
                question.data.leftColumn.map((answer, i) =>
                    (<PlaythroughMatchPairsRow question={question} onChange={onChange} key={i} 
                                               leftColumn={question.data.leftColumn[i]} 
                                               rightColumn={question.data.rightColumn[i]} index={i}/>))}
            </tbody>
        </table>
    </>)
}

export default PlaythroughMatchPairsContainer;