import { createRoot } from 'react-dom/client';
import Comments from "./comments.jsx";
import RateTest from "./testResult/rateTest.jsx";
import ReportButton from "./buttons/reportButton.jsx";

const initialData = window.__INITIAL_DATA__;

const commentRoot = createRoot(document.getElementById('comments'));
commentRoot.render(<Comments baseComments={initialData.comments} basePages={initialData.commentPages}
                             commentsPerPage={initialData.commentsPerPage} isTest={true} dtoId={initialData.completion.testId}
                             isBanned={initialData.isCurrBanned} currentUserGroup={initialData.currentUserGroup}
                             currentUserId={initialData.currentUserId}/>);

if (document.getElementById('test-interact')) {
    const interactRoot = createRoot(document.getElementById('test-interact'));
    interactRoot.render(<RateTest testId={initialData.completion.testId} baseRating={initialData.currentRating}/>)
}

const reportRoot = createRoot(document.getElementById('report-button'));
reportRoot.render(<ReportButton entityType={1} entityId={initialData.completion.testId}/>)