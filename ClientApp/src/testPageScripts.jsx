import { createRoot } from 'react-dom/client';
import Comments from "./comments.jsx";
import StartTestButton from "./test/startTestButton.jsx";
import ReportButton from "./buttons/reportButton.jsx";

const initialData = window.__INITIAL_DATA__;

const commentRoot = createRoot(document.getElementById('comments'));
commentRoot.render(<Comments baseComments={initialData.comments} basePages={initialData.commentPages} 
                             commentsPerPage={initialData.commentsPerPage} isTest={true} dtoId={initialData.test.id} 
                             isBanned={initialData.isCurrBanned} currentUserGroup={initialData.currentUserGroup} 
                             currentUserId={initialData.currentUserId}/>);

const startRoot = createRoot(document.getElementById('test-start'));
startRoot.render(<StartTestButton testId={initialData.test.id} hasPassword={initialData.canStartTest}/>)

const reportRoot = createRoot(document.getElementById('report-button'));
reportRoot.render(<ReportButton entityType={1} entityId={initialData.test.id}/>)