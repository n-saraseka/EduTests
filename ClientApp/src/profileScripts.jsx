import { createRoot } from 'react-dom/client';
import Comments from './comments.jsx';

const initialData = window.__INITIAL_DATA__;

const commentRoot = createRoot(document.getElementById('comments'));
commentRoot.render(<Comments commentsPerPage={parseInt(initialData.commentsPerPage)} 
                             dtoId={initialData.user.id} 
                             isTest={false} 
                             baseComments={initialData.comments} 
                             basePages={parseInt(initialData.commentPages)}
                             currentUserId={parseInt(initialData.currentUserId)}/>);