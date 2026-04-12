function TestAside({tabs, currentTab, setCurrentTab}) {
    const tabNames = ["Тест", "Результат", "Доп. настройки", "Опубликовать тест"]
    
    return (<aside>
        <ul id="tabs" className="border-right">
            {tabs.map((tab, index) => (
                <li key={index}>
                    <img src={`/files/icons/${tab}.png`} alt="Иконка вкладки" onClick={() => setCurrentTab(index)}
                         className={currentTab === index ? "active" : ""}/>
                    <span className={currentTab === index ? "aside-title active" : "aside-title"}
                          onClick={() => setCurrentTab(index)}>{tabNames[index]}</span>
                </li>
            ))}
        </ul>
    </aside>)
}

export default TestAside;