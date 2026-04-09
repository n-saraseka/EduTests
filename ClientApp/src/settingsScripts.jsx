import { createRoot } from 'react-dom/client';
import UserCardHandler from './settings/userCardHandler.jsx';
import ReportsTable from "./settings/reportsTable.jsx";
import BannedTable from "./settings/bannedTable.jsx";

const initialData = window.__INITIAL_DATA__;

const cardRoot = createRoot(document.getElementById('user-card'));
cardRoot.render(<UserCardHandler baseUser={initialData.user}/>);

if (document.getElementById('reports')) {
    const reportRoot = createRoot(document.getElementById('reports'));
    reportRoot.render(<ReportsTable baseReports={initialData.reports} basePages={initialData.reportPages}
                                    rowsPerPage={initialData.rowsPerTablePage}/>)
    
    const banRoot = createRoot(document.getElementById('banned-users'));
    banRoot.render(<BannedTable baseBans={initialData.bans} basePages={initialData.banPages} 
                                rowsPerPage={initialData.rowsPerTablePage}/>);
}
