import DeleteButton from "../buttons/deleteButton.jsx";
import EditButton from "../buttons/editButton.jsx";
import {useState} from "react";

function ConstructorSequenceItem({question, baseText, sequence, onModification, onDeletion, index}) {
    const [isDragged, setIsDragged] = useState(false);
    const [showEditButtons, setShowEditButtons] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [text, setText] = useState(baseText);
    const [tempText, setTempText] = useState(baseText);
    const [shadow, setShadow] = useState(null);

    const handleDragStart = (event) => {
        event.dataTransfer.clearData('text/plain');
        event.dataTransfer.setData('text/plain', JSON.stringify({
            draggedIndex: index,
            draggedText: text
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
        event.dataTransfer.clearData('text/plain');
    }

    const confirmEdit = () => {
        setText(tempText);
        setTempText(text);
        const newSequence = sequence.map((e, i) => i === index ? tempText : e);
        onModification(newSequence);
        setIsEditing(false);
    }

    return (<div className={`sequence-item${isDragged ? ' dragged' : ''}`} draggable={true} data-index={index}
                 onDragStart={handleDragStart} onDragEnd={() => setIsDragged(false)}
                 onDragOver={handleDragOver} onDragLeave={handleDragLeave} onDrop={handleDrop}
                 onMouseOver={() => setShowEditButtons(true)} onMouseLeave={() => setShowEditButtons(false)}
                 style={{boxShadow: shadow}}>
        {isEditing ? <input type="text" id={`sequence-item-${question.orderIndex}-${index}`}
                            defaultValue={tempText} onChange={(event) => setTempText(event.target.value)} autoFocus={true}/>
            : <div>{text}</div>}
        {showEditButtons && <>
            <EditButton isEditing={isEditing} onEditToggle={() => setIsEditing(true)}
                        onCancel={() => setIsEditing(false)} onConfirm={confirmEdit}
                        isDisabled={false}/>
            <DeleteButton entityType="answer" onDelete={() => onDeletion(index)}/>
        </>}
    </div>)
}

function shuffleColumn(column) {
    return column.sort(() => Math.random() - 0.5);
}

export default ConstructorSequenceItem;