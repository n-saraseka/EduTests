import { createRoot } from 'react-dom/client';
import Comments from './comments.jsx';
import ProfileUsername from "./profile/profileUsername.jsx";
import ProfileTests from "./profile/profileTests.jsx";

const initialData = window.__INITIAL_DATA__;

window.onLoad = () => {
    
}
const commentRoot = createRoot(document.getElementById('comments'));
const usernameRoot = createRoot(document.getElementById('username'));

usernameRoot.render(<ProfileUsername username={initialData.user.username} userId={initialData.user.id} 
                                     currentUserId={parseInt(initialData.currentUserId)}
                                     currentUserGroup={initialData.currentUserGroup} 
                                     isBanned={initialData.isBanned}/>);
commentRoot.render(<Comments commentsPerPage={parseInt(initialData.commentsPerPage)} 
                             dtoId={initialData.user.id} 
                             isTest={false} 
                             baseComments={initialData.comments} 
                             basePages={parseInt(initialData.commentPages)}
                             currentUserId={parseInt(initialData.currentUserId)} 
                             currentUserGroup={initialData.currentUserGroup} 
                             isBanned={initialData.isCurrentBanned}/>);

if (document.getElementById('tests')) {
    const testRoot = createRoot(document.getElementById('tests'));
    testRoot.render(<ProfileTests basePages={initialData.testPages} baseTests={initialData.tests} pageSize={initialData.testPageSize} userId={initialData.user.id}/>);
}