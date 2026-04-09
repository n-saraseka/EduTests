function TestAside({tabs, currentTab, setCurrentTab}) {
    return (<aside>
        <ul id="tabs" className="border-right">
            {tabs.map((tab, index) => (
                <li key={index}>
                    <img src={`/files/icons/${tab}.png`} alt="Иконка вкладки" onClick={() => setCurrentTab(index)}
                         style={{opacity: currentTab === index ? 1.0 : 0.5}}/>
                </li>
            ))}
        </ul>
    </aside>)
}

export default TestAside;