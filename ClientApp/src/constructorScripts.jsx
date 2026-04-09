import { createRoot } from 'react-dom/client';
import Constructor from './constructor/constructor.jsx';

const initialData = window.__INITIAL_DATA__;
const appRoot = createRoot(document.querySelector('#app'));

appRoot.render(<Constructor user={initialData.user} baseTest={initialData.test}/>);