import ConstructorSequenceItem from "./constructorSequenceItem.jsx";

function ConstructorSequenceContainer({question, onChange}) {
    const addItem = () => {
        const newSequence = question.correctData.sequence.concat(["Введите текст"]);
        const newOptions = shuffleSequence(newSequence);
        onChange({...question,
            data: {...question.data, options: newOptions},
            correctData: {...question.correctData, sequence: newSequence}});
    }

    const changeSequence = (seq) => {
        const newOptions = shuffleSequence(seq);
        onChange({...question,
            data: {...question.data, options: newOptions},
            correctData: {...question.correctData, sequence: seq}});
    }

    const shuffleSequence = (seq) => {
        const copy = [...seq];
        return copy.sort(() => Math.random() - 0.5);
    };

    const deleteItem = (index) => {
        const newOptions = question.data.options.filter(o => o !== question.correctData.sequence[index]);
        onChange({...question,
            data: {...question.data, options: newOptions},
            correctData: {...question.correctData, sequence: question.correctData.sequence.filter((e, i) => i !== index)}});
    }

    return (<>
        <div className="sequence-items">
            {question.correctData.sequence.map((e, i) =>
                <ConstructorSequenceItem question={question} key={i} baseText={e} index={i}
                              sequence={question.correctData.sequence} onModification={changeSequence}
                              onDeletion={deleteItem}/>)}
            <button className="btn btn-primary" onClick={addItem}>Добавить</button>
        </div>
    </>);
}

export default ConstructorSequenceContainer;