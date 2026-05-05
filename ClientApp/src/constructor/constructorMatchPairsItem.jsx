import {useState} from "react";
import EditButton from "../buttons/editButton.jsx";

function ConstructorMatchPairsItem({question, onChange, baseText, isLeft, index}) {
    const [isDragged, setIsDragged] = useState(false);
    const [showEditButtons, setShowEditButtons] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [text, setText] = useState(baseText);
    const [tempText, setTempText] = useState(baseText);

    const handleDragStart = (event) => {
        event.dataTransfer.clearData('text/plain');
        event.dataTransfer.setData('text/plain', JSON.stringify({
            draggedIndex: parseInt(index),
            draggedText: text
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

            if (targetIsLeft === isLeft) {
                const targetIndex = parseInt(target.getAttribute("data-index"));
                const targetText = target.querySelector("span").innerText;
                let pairs = [...question.correctData.pairs];
                if (isLeft) {
                    pairs = pairs.map((p, i) => {
                        if (i === draggedIndex) {
                            p.left = targetText;
                        }
                        if (i === targetIndex) {
                            p.left = draggedText;
                        }
                        return p;
                    })
                }
                else {
                    pairs = pairs.map((p, i) => {
                        if (i === draggedIndex) {
                            p.right = targetText;
                        }
                        if (i === targetIndex) {
                            p.right = draggedText;
                        }
                        return p;
                    })
                }
                const leftColumn = pairs.map((p) => p.left), rightColumn = pairs.map((p) => p.right);

                onChange({...question,
                    data: {...question.data, leftColumn: shuffleColumn(leftColumn), rightColumn: shuffleColumn(rightColumn)},
                    correctData: {...question.correctData, pairs: pairs}});
            }
            setIsDragged(false);
        }
        event.dataTransfer.clearData('text/plain');
    }
    const confirmEdit = () => {
        setText(tempText);
        setTempText(text);

        let pairs = [...question.correctData.pairs];
        if (isLeft) {
            pairs = pairs.map((p, i) => {
                if (i === index) {
                    p.left = tempText;
                }
                return p;
            });
        }
        else {
            pairs = pairs.map((p, i) => {
                if (i === index) {
                    p.right = tempText;
                }
                return p;
            });
        }
        const leftColumn = pairs.map((p) => p.left), rightColumn = pairs.map((p) => p.right);
        
        onChange({...question,
            data: {...question.data, leftColumn: shuffleColumn(leftColumn), rightColumn: shuffleColumn(rightColumn)},
            correctData: {...question.correctData, pairs: pairs}});

        setIsEditing(false);
    }

    return (<td className={`pair-item${isDragged ? ' dragged' : ''}`} draggable={true} data-index={index} data-left={isLeft}
                onDragStart={handleDragStart} onDragEnd={() => setIsDragged(false)}
                onDragOver={handleDragOver} onDragLeave={handleDragLeave} onDrop={handleDrop}
                onMouseOver={() => setShowEditButtons(true)} onMouseLeave={() => setShowEditButtons(false)}>
        <>
            {isEditing ? <input type="text" defaultValue={tempText} onChange={(event) => setTempText(event.target.value)}/>
                : <span>{text}</span>}
            {showEditButtons && <>
                <EditButton isEditing={isEditing} onEditToggle={() => setIsEditing(true)}
                            onCancel={() => setIsEditing(false)} onConfirm={confirmEdit}
                            isDisabled={false}/>
            </>}      
        </>
    </td>)
}

function shuffleColumn(column) {
    return column.sort(() => Math.random() - 0.5);
}

export default ConstructorMatchPairsItem;