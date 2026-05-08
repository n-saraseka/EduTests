import {useState} from "react";
import Pagination from "../pagination.jsx";
import ResultDetail from "./resultDetail.jsx";

function ResultDetails({baseQuestions, baseAnswers, basePages, answersPerPage}) {
    const [currentPage, setCurrentPage] = useState(1);
    const [questions, setQuestions] = useState(baseQuestions.slice(0, answersPerPage));
    
    const changePage = (page) => {
        if (page !== currentPage) {
            setCurrentPage(page);
            setQuestions(baseQuestions.slice((page - 1) * answersPerPage, page * answersPerPage));
        }
    }
    
    return (<>
        {questions.map((q, index) => (<ResultDetail key={index} question={q} answer={baseAnswers.find(a => a.questionId === q.id)}/>))}
        {basePages > 1 && <Pagination page={currentPage} pageCount={basePages} onChangePage={changePage}/>}
    </>)
}

export default ResultDetails;