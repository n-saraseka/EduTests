import { createRoot } from 'react-dom/client';
import IndividualStats from "./stats/individualStats.jsx";
import DownloadXlsxButton from "./stats/downloadXlsxButton.jsx";

const initialData = window.__INITIAL_DATA__;

const stats = document.getElementById('individual-stats');
if (stats) {
    const statsRoot = createRoot(document.getElementById('individual-stats'));
    statsRoot.render(<>
        <DownloadXlsxButton questions={initialData.questions} test={initialData.test}/>
        <IndividualStats testId={initialData.test.id} baseCompletions={initialData.completions}
                         basePages={initialData.pages}
                         rowsPerPage={initialData.pageSize}/>
    </>)
}