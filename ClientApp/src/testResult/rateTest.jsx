import { useState } from "react";

function RateTest({testId, baseRating}) {
    const [isPositive, setIsPositive] = useState(baseRating === null ? null : baseRating.isPositive);
    
    const rateTest = async (newRating) => {
        const command = new RateTestCommand(newRating);
        const response = await rate(testId, command);
        if (response.ok) {
            setIsPositive(newRating);
        }
    }
    
    return (<span>Вам понравился тест? <img src="/files/icons/like.png" alt="Да" 
                                            className={`edit-icon ${isPositive !== true && "inactive"}`}
                                            onClick={() => rateTest(true)}/> / 
        <img src="/files/icons/dislike.png" alt="Нет" className={`edit-icon ${isPositive !== false && "inactive"}`} 
             onClick={() => rateTest(false)}/></span>)
}

class RateTestCommand {
    constructor(isPositive) {
        this.isPositive = isPositive;
    }
}

function rate(testId, command) {
    return fetch(`/api/tests/${testId}/rating`, {
        method: "PUT",
        body: JSON.stringify(command),
        headers: {
            "Content-Type": "application/json"
        }
    });
}

export default RateTest;