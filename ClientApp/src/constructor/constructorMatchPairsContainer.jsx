import ConstructorMatchPairsRow from "./constructorMatchPairsRow.jsx";

function ConstructorMatchPairsContainer({question, onChange}) {
    const removePair = (index) => {
        const pair = [question.correctData.pairs[index]];
        // This is bad in case of repeating text. Don't have the time and motivation to figure out a correct way of doing this
        const leftColumn = question.data.leftColumn.filter(a => a !== pair[0]);
        const rightColumn = question.data.rightColumn.filter(a => a !== pair[1]);
        const newPairs = question.correctData.pairs.filter((p, i) => i !== index);

        onChange({...question,
            data: {...question.data, leftColumn: shuffleColumn(leftColumn), rightColumn: shuffleColumn(rightColumn)},
            correctData: {...question.correctData, pairs: newPairs}});
    }

    const addPair = () => {
        const sampleText = "Введите текст";
        onChange({...question,
            data: {...question.data,
                leftColumn: [...question.data.leftColumn, sampleText],
                rightColumn: [...question.data.rightColumn, sampleText]},
            correctData: {...question.correctData, pairs: [...question.correctData.pairs, {left: sampleText, right: sampleText}]}});
    }

    return (<>
        <table>
            <thead>
            <tr>
                <th>Левый столбец</th>
                <th>Правый столбец</th>
            </tr>
            </thead>
            <tbody>
            {question.correctData.pairs.map((pair, i) => (<ConstructorMatchPairsRow question={question} 
                                                                         onChange={onChange} key={i} leftColumn={pair.left} 
                                                                         rightColumn={pair.right} onDeletion={removePair} index={i}/>))}
            </tbody>
        </table>
        <button className="btn btn-primary" onClick={addPair}>Добавить пару</button>
    </>)
}

function shuffleColumn(column) {
    return column.sort(() => Math.random() - 0.5);
}

export default ConstructorMatchPairsContainer;