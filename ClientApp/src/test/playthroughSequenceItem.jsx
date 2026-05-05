import {useState} from "react";

function PlaythroughSequenceItem({baseText, sequence, onModification, index}) {
    const [isDragged, setIsDragged] = useState(false);
    const [shadow, setShadow] = useState(null);

    const handleDragStart = (event) => {
        event.dataTransfer.setData('text/plain', JSON.stringify({
            draggedIndex: index,
            draggedText: baseText
        }));
        setIsDragged(true);
    }

    const handleDragOver = (event) => {
        event.preventDefault();
        if (event.target.classList.contains('sequence-item')) {
            const potentialElementCoord = event.target.getBoundingClientRect();
            const center = potentialElementCoord.left + potentialElementCoord.width / 2;

            if (event.clientX < center) {
                setShadow('-2px 0 0 0 blue');
            } else {
                setShadow('2px 0 0 0 blue');
            }
        }
    };

    const handleDragLeave = (event) => {
        event.preventDefault();
        if (event.target.classList.contains('sequence-item')) {
            setShadow(null);
        }
    };

    const handleDrop = (event) => {
        event.preventDefault();
        if (event.target.classList.contains('sequence-item') && event.dataTransfer.getData('text/plain')) {
            const draggedData = JSON.parse(event.dataTransfer.getData('text/plain'));
            const { draggedIndex, draggedText } = draggedData;

            const potentialElementCoord = event.target.getBoundingClientRect();
            const center = potentialElementCoord.left + potentialElementCoord.width / 2;

            // if cursor is to the left - insert to the left
            // insert to the right otherwise
            const cursorPosition = event.clientX;
            const newSequence = [...sequence];

            newSequence.splice(draggedIndex, 1);

            let usedIndex = index;

            if (draggedIndex < index) {
                usedIndex--;
            }

            if (cursorPosition < center) {
                newSequence.splice(usedIndex, 0, draggedText);
            }
            else {
                newSequence.splice(usedIndex + 1, 0, draggedText);
            }

            onModification(newSequence);
            setIsDragged(false);
        }
    }

    return (<div className={`sequence-item${isDragged ? ' dragged' : ''}`} draggable={true} data-index={index}
                 onDragStart={handleDragStart} onDragEnd={() => setIsDragged(false)}
                 onDragOver={handleDragOver} onDragLeave={handleDragLeave} onDrop={handleDrop}
                 style={{boxShadow: shadow}}>{baseText}</div>)
}

export default PlaythroughSequenceItem;