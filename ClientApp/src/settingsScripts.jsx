import { createRoot } from 'react-dom/client';
import UserCardHandler from './userCardHandler.jsx';

const cardRoot = createRoot(document.getElementById('user-card'));
cardRoot.render(<UserCardHandler/>);