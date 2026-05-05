import PlaythroughSequenceItem from "./playthroughSequenceItem.jsx";

function PlaythroughSequenceContainer({question, onChange, sequence}) {
    const changeSequence = (seq) => onChange({questionId: question.id, answer: {sequence: seq}});

    return (<>
        <div className="sequence-items">
            {sequence.map((e, i) =>
                <PlaythroughSequenceItem key={i} baseText={e} index={i}
                              sequence={sequence} onModification={changeSequence}/>)}
        </div>
    </>);
}

export default PlaythroughSequenceContainer;