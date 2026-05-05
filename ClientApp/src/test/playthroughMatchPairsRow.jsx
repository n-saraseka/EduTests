import {useState} from "react";
import DeleteButton from "../buttons/deleteButton.jsx";
import PlaythroughMatchPairsItem from "./playthroughMatchPairsItem.jsx";

function PlaythroughMatchPairsRow({question, answer, onChange, leftColumn, rightColumn, index}) {
    return (<tr>
        <PlaythroughMatchPairsItem question={question} answer={answer} onChange={onChange} baseText={leftColumn} isLeft={true} index={index}/>
        <PlaythroughMatchPairsItem question={question} answer={answer} onChange={onChange} baseText={rightColumn} isLeft={false} index={index}/>
    </tr>)
}

export default PlaythroughMatchPairsRow;