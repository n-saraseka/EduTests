import {useState} from "react";
import TestAside from "./testAside.jsx";
import TestTab from "./testTab.jsx";
import ResultTab from "./resultTab.jsx";
import ExtraSettingsTab from "./extraSettingsTab.jsx";

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
            timeLimit: null,
            attemptLimit: null,
            defaultResult: null
        });
    const [currentTab, setCurrentTab] = useState(0); // Default to test and questions settings
    const fieldNames = ['timeLimit', 'attemptLimit'];
    const [lastFocusedField, setLastFocusedField] = useState(null);
    
    function TabSwitch() {
        switch(currentTab) {
            // General settings and questions
            case 0:
                return <TestTab test={test} setTest={setTest}/>;
            // Result settings
            case 1:
                return <ResultTab test={test} setTest={setTest}/>;
            // Extra settings
            case 2:
                return <ExtraSettingsTab test={test} setTest={setTest} fieldNames={fieldNames}
                                         lastFocusedField={lastFocusedField} setLastFocusedField={setLastFocusedField}/>;
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