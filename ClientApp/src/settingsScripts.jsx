import { createRoot } from 'react-dom/client';
import UserCardHandler from './settings/userCardHandler.jsx';
import ReportsTable from "./settings/reportsTable.jsx";
import BannedTable from "./settings/bannedTable.jsx";
import TestsTable from "./settings/testsTable.jsx";

const initialData = window.__INITIAL_DATA__;

const cardRoot = createRoot(document.getElementById('user-card'));
cardRoot.render(<UserCardHandler baseUser={initialData.user}/>);

const testRoot = createRoot(document.getElementById('tests'));
testRoot.render(<TestsTable userId={initialData.user.id} baseTests={initialData.tests} basePages={initialData.testPages} 
                            rowsPerPage={initialData.rowsPerTablePage}/>);

if (document.getElementById('reports')) {
    const reportRoot = createRoot(document.getElementById('reports'));
    reportRoot.render(<ReportsTable baseReports={initialData.reports} basePages={initialData.reportPages}
                                    rowsPerPage={initialData.rowsPerTablePage}/>)
    
    const banRoot = createRoot(document.getElementById('banned-users'));
    banRoot.render(<BannedTable baseBans={initialData.bans} basePages={initialData.banPages} 
                                rowsPerPage={initialData.rowsPerTablePage}/>);
}
