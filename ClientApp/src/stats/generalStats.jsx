function GeneralStats({data}) {
    return (<div id="general-stats">
        <table>
            <tbody>
            <tr>
                <td>Процент прохождения</td>
                <td>
                    {data.completionCount === 1
                        ? <span>{data.minPercentage}</span>
                        : <div className="general-stat">
                            <div className="stat-bar"></div>
                            <div className="stat-measures">
                                <div className="stat-measure stat-min">
                                    <span title="Минимум">Мин.</span>
                                    <span>{data.minPercentage}</span>
                                </div>
                                <div className="stat-measure stat-mid"
                                     style={{left : `${data.medianPercentage
                                         / data.maxPercentage
                                         * 100.0}%`}}>
                                    <span title="Медиана">Мед.</span>
                                    <span>{data.medianPercentage}</span>
                                </div>
                                <div className="stat-measure stat-max">
                                    <span title="Максимум">Макс.</span>
                                    <span>{data.maxPercentage}</span>
                                </div>
                            </div>
                        </div>}
                </td>
            </tr>
            <tr>
                <td>Время прохождения</td>
                <td>
                    {data.completionCount === 1
                        ? <span>{timeSpanToString(data.minTime)}</span>
                        : <div className="general-stat">
                            <div className="stat-bar"></div>
                            <div className="stat-measures">
                                <div className="stat-measure stat-min">
                                    <span title="Минимум">Мин.</span>
                                    <span>{timeSpanToString(data.minTime)}</span>
                                </div>
                                <div className="stat-measure stat-mid"
                                     style={{left : `${secondsFromTimeSpan(data.interQuartileAverageTime)
                                         / secondsFromTimeSpan(data.maxTime)
                                         * 100.0}%`}}>
                                    { data.completionCount < 4
                                        ? <span title="Среднее">Ср.</span>
                                        : <span title="Усечённое среднее">Усеч. ср.</span> }
                                    <span>{timeSpanToString(data.interQuartileAverageTime)}</span>
                                </div>
                                <div className="stat-measure stat-max">
                                    <span title="Максимум">Макс.</span>
                                    <span>{timeSpanToString(data.maxTime)}</span>
                                </div>
                            </div>
                        </div>}
                </td>
            </tr>
            <tr>
                <td>Количество прохождений:</td>
                <td>{data.completionCount}</td>
            </tr>
            </tbody>
        </table>
    </div>)
}

function secondsFromTimeSpan(time) {
    const regex = /^(-)?(?:(\d+)\.)?(\d{1,2}):(\d{2}):(\d{2})(?:\.(\d+))?$/;
    const match = time.match(regex);
    if (!match) throw new Error('Invalid TimeSpan string: ' + time);

    const negative = match[1] === '-';
    const days = parseInt(match[2] || '0', 10);
    const hours = parseInt(match[3], 10);
    const minutes = parseInt(match[4], 10);
    const seconds = parseInt(match[5], 10);
    
    return (days * 86400) + (hours * 3600) + (minutes * 60) + seconds;
}

function timeSpanToString(time) {
    const allSeconds = secondsFromTimeSpan(time);
    const hours = Math.floor(allSeconds / 3600);
    const minutes = Math.floor((allSeconds - hours * 3600) / 60);
    const seconds = Math.floor(allSeconds - 3600 * hours - 60 * minutes);
    
    const hourString = hours < 10 ? '0' + hours : hours;
    const minuteString = minutes < 10 ? '0' + minutes : minutes;
    const secondString = seconds < 10 ? '0' + seconds : seconds;
    return `${hourString}:${minuteString}:${secondString}`;
}

export default GeneralStats;