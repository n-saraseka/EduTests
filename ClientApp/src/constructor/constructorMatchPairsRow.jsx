import {useState} from "react";
import DeleteButton from "../buttons/deleteButton.jsx";
import ConstructorMatchPairsItem from "./constructorMatchPairsItem.jsx";

function ConstructorMatchPairsRow({question, onChange, leftColumn, rightColumn, onDeletion, index}) {
    const [showDeleteButton, setShowDeleteButton] = useState(false);

    return (<tr onMouseOver={() => setShowDeleteButton(true)} onMouseLeave={() => setShowDeleteButton(false)}>
        <ConstructorMatchPairsItem question={question} onChange={onChange} baseText={leftColumn} isLeft={true} index={index}/>
        <ConstructorMatchPairsItem question={question} onChange={onChange} baseText={rightColumn} isLeft={false} index={index}/>
        {showDeleteButton ? <td><DeleteButton entityType="answer" onDelete={() => onDeletion(index)}/></td> : <td></td>}
    </tr>)
}

export default ConstructorMatchPairsRow;