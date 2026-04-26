import {useState} from "react";

function ExtraSettingsTab({test, setTest, fieldNames, lastFocusedField, setLastFocusedField}) {
    const [usingTimeLimit, setUsingTimeLimit] = useState(test.timeLimit !== null);
    const [usingAttemptLimit, setUsingAttemptLimit] = useState(test.attemptLimit !== null);
    
    const timeLimitToMinutes = (time) => {
        const [hours, minutes, seconds] = time.split(':').map(Number);
        return hours * 60 + minutes + seconds / 60;
    }
    
    const minutesToTimeLimit = (minutes) => {
        return new Date(parseInt(minutes) * 60 * 1000).toISOString().substring(11, 19);
    }
    
    const timeLimitSwitch = () => {
        const newUsing = !usingTimeLimit;
        if (!newUsing) {
            setTest(prevTest => ({...prevTest, timeLimit: null}));
        }
        else {
            setTest(prevTest => ({...prevTest, timeLimit: minutesToTimeLimit(1)}));
        }
        setUsingTimeLimit(newUsing);
    }

    const changeTimeLimit = (event) => {
        const newLimit = minutesToTimeLimit(parseInt(event.target.value));
        if (test.timeLimit !== newLimit) {
            setTest(prevTest => ({...prevTest, timeLimit: newLimit}));
        }
    }

    const attemptLimitSwitch = () => {
        const newUsing = !usingAttemptLimit;
        if (!newUsing) {
            setTest(prevTest => ({...prevTest, attemptLimit: null}));
        }
        else {
            setTest(prevTest => ({...prevTest, attemptLimit: 1}));
        }
        setUsingAttemptLimit(newUsing);
    }
    
    const changeAttemptLimit = (event) => {
        const newLimit = parseInt(event.target.value);
        if (test.attemptLimit !== newLimit) {
            setTest(prevTest => ({...prevTest, attemptLimit: newLimit}));
        }
    }
    
    const changeTestPassword = (event) => {
        const newPassword = event.target.value;
        if (test.password !== newPassword) {
            setTest(prevTest => ({...prevTest, password: newPassword}));
        }
    }
    
    return (<div className="constructor-tab">
        <h1>Дополнительные настройки</h1>
        <div className="test-card">
            <div className="test-card-row">
                <label htmlFor="using-time-limit">Ограничение времени:</label>
                <input type="checkbox" id="using-time-limit" checked={usingTimeLimit} onChange={timeLimitSwitch}/>
                {usingTimeLimit && <>
                    <input type="number" step={1} min={1} max={300}
                           defaultValue={test.timeLimit === null ? 1 : timeLimitToMinutes(test.timeLimit)}
                           id="time-limit" onChange={changeTimeLimit}
                           onFocus={() => setLastFocusedField(fieldNames[0])} autoFocus={lastFocusedField === fieldNames[0]}/>
                    <label htmlFor="time-limit">мин.</label>
                </>}
            </div>
            <div className="test-card-row">
                <label htmlFor="using-attempt-limit">Ограничение прохождений:</label>
                <input type="checkbox" id="using-attempt-limit" checked={usingAttemptLimit} onChange={attemptLimitSwitch}/>
                {usingAttemptLimit && <>
                    <input type="number" step={1} min={1} max={10} 
                           defaultValue={test.attemptLimit === null ? 1 : test.attemptLimit} 
                           onChange={changeAttemptLimit} 
                           onFocus={() => setLastFocusedField(fieldNames[1])} autoFocus={lastFocusedField === fieldNames[1]}
                           id="attempt-limit"/>
                </>}
            </div>
            <div className="test-card-row">
                <label htmlFor="test-password">Пароль:</label>
                <input type="password" id="test-password" name="test-password"
                       defaultValue={test.password === null ? undefined : test.password} onChange={changeTestPassword} autoFocus={true}/>
            </div>
        </div>
    </div>)
}

export default ExtraSettingsTab;