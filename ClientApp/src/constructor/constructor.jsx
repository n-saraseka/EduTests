import {useState} from "react";
import TestAside from "./testAside.jsx";
import TestTab from "./testTab.jsx";
import ResultTab from "./resultTab.jsx";

const tabs = ["test", "result", "extra-settings", "publish"];

function Constructor({baseTest, user}) {
    const [test, setTest] = useState(baseTest !== null ? baseTest :
        {
            userId: user.id,
            name: "Без названия",
            description: null,
            tags: [],
            questions: [],
            results: [],
            defaultResult: null
        });
    const [currentTab, setCurrentTab] = useState(0); // Default to test and questions settings
    
    function TabSwitch() {
        switch(currentTab) {
            case 0:
                return <TestTab test={test} setTest={setTest}/>;
            case 1:
                return <ResultTab test={test} setTest={setTest}/>;
            default:
                return <TestTab test={test} setTest={setTest}/>;
        }
    }
    
    return (<>
        <TestAside tabs={tabs} currentTab={currentTab} setCurrentTab={setCurrentTab}/>
        <TabSwitch/>
    </>)
}

export default Constructor;