import { createRoot } from 'react-dom/client';
import IndividualStats from "./stats/individualStats.jsx";

const initialData = window.__INITIAL_DATA__;

const stats = document.getElementById('individual-stats');
if (stats) {
    const statsRoot = createRoot(document.getElementById('individual-stats'));
    statsRoot.render(<IndividualStats testId={initialData.test.id} baseCompletions={initialData.completions} 
                                      basePages={initialData.pages} 
                                      rowsPerPage={initialData.pageSize}/>)
}