import { createRoot } from 'react-dom/client';
import TestPlaythrough from "./test/testPlaythrough.jsx";

const initialData = window.__INITIAL_DATA__;

const appRoot = createRoot(document.getElementById('app'));
appRoot.render(<TestPlaythrough baseAnswers={initialData.answers} baseQuestions={initialData.questions} 
                                baseLastUnanswered={initialData.lastUnansweredQuestion} baseTest={initialData.test} 
                                completion={initialData.completion}/>);