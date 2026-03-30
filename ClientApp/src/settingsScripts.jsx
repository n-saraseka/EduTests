import { createRoot } from 'react-dom/client';
import UserCardHandler from './settings/userCardHandler.jsx';
import ReportsTable from "./settings/reportsTable.jsx";

const initialData = window.__INITIAL_DATA__;

const cardRoot = createRoot(document.getElementById('user-card'));
cardRoot.render(<UserCardHandler userId={initialData.user.id} baseUsername={initialData.user.username} 
                                 baseDescription={initialData.user.description}/>);

if (document.getElementById('reports')) {
    const reportRoot = createRoot(document.getElementById('reports'));
    reportRoot.render(<ReportsTable baseReports={initialData.reports} basePages={initialData.reportPages}
                                    rowsPerPage={initialData.rowsPerTablePage}/>)
}
