import {useState} from "react";

function PlaythroughMatchPairsItem({question, answer, onChange, baseText, isLeft, index}) {
    const [isDragged, setIsDragged] = useState(false);

    const handleDragStart = (event) => {
        event.dataTransfer.clearData('text/plain');
        event.dataTransfer.setData('text/plain', JSON.stringify({
            draggedIndex: parseInt(index),
            draggedText: baseText
        }));
        setIsDragged(true);
    }

    const handleDragOver = (event) => {
        event.preventDefault();
    };

    const handleDragLeave = (event) => {
        event.preventDefault();
    };

    const handleDrop = (event) => {
        event.preventDefault();
        const target = event.currentTarget;
        if (target.classList.contains('pair-item') && event.dataTransfer.getData('text/plain')) {
            const draggedData = JSON.parse(event.dataTransfer.getData('text/plain'));
            const { draggedIndex, draggedText } = draggedData;
            const targetIsLeft = target.getAttribute("data-left") === "true";
            let pairs = answer !== undefined
                ? answer.answer.pairs
                : question.data.leftColumn.map((a, i) => {
                    return {
                        left: question.data.leftColumn[i],
                        right: question.data.rightColumn[i]
                    }});

            if (targetIsLeft === isLeft) {
                const targetIndex = parseInt(target.getAttribute("data-index"));
                const targetText = isLeft ? pairs[targetIndex].left : pairs[targetIndex].right;
                if (isLeft) {
                    pairs = pairs.map((p, i) => {
                        if (i === draggedIndex) {
                            return { ...p, left: targetText };
                        }
                        if (i === targetIndex) {
                            return { ...p, left: draggedText };
                        }
                        return p;
                    });
                }
                else {
                    pairs = pairs.map((p, i) => {
                        if (i === draggedIndex) {
                            return { ...p, right: targetText };
                        }
                        if (i === targetIndex) {
                            return { ...p, right: draggedText };
                        }
                        return p;
                    });
                }

                onChange({...answer, questionId: question.id, answer: {pairs: pairs}});
            }
            setIsDragged(false);
        }
        event.dataTransfer.clearData('text/plain');
    }

    return (<td className={`pair-item${isDragged ? ' dragged' : ''}`} draggable={true} data-index={index} data-left={isLeft}
                onDragStart={handleDragStart} onDragEnd={() => setIsDragged(false)}
                onDragOver={handleDragOver} onDragLeave={handleDragLeave} onDrop={handleDrop}>
        {baseText}
    </td>)
}

export default PlaythroughMatchPairsItem;