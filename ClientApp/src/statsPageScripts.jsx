import { createRoot } from 'react-dom/client';
import Stats from "./stats/stats.jsx";

const initialData = window.__INITIAL_DATA__;

const stats = document.getElementById('app');
if (stats) {
    const statsRoot = createRoot(document.getElementById('app'));
    statsRoot.render(<>
        <Stats test={initialData.test} baseQuestions={initialData.questions} baseCompletions={initialData.completions} 
               versions={initialData.versions} baseGenStats={initialData.completionStats} 
               rowsPerPage={initialData.pageSize} basePages={initialData.pages}/>
    </>)
}