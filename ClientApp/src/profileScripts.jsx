import { createRoot } from 'react-dom/client';
import Comments from './comments.jsx';
import ProfileUsername from "./profile/profileUsername.jsx";

const initialData = window.__INITIAL_DATA__;

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