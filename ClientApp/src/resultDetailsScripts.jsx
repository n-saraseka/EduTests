import { createRoot } from 'react-dom/client';
import ResultDetails from "./testResult/resultDetails.jsx";

const initialData = window.__INITIAL_DATA__;

console.log(initialData);

const appRoot = createRoot(document.getElementById('details'));

appRoot.render(<ResultDetails basePages={initialData.pages} answersPerPage={initialData.pageSize} 
                              baseQuestions={initialData.questions} baseAnswers={initialData.answers}/>)