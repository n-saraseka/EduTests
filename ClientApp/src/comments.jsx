import {useState} from "react";

function Comment({baseComment}) {
    return (<div className="comment">
        <div className="comment-header">
            <img src={`/files/users/${baseComment.userId}`} 
                 alt={`Аватар пользователя ${baseComment.user.username}`} className="comment-avatar"/>
            <span className="username">{baseComment.user.username}</span>
        </div>
        <p className="comment-content">{baseComment.content}</p>
    </div>)
}

function PostComment({ isPosting, onPost}) {
    return ( <>
            <textarea name="new-comment" id="new-comment" cols="30" rows="10" placeholder="Введите свой комментарий..." disabled={isPosting}></textarea>
            <button className="btn-primary" disabled={isPosting} onClick={onPost}>Опубликовать комментарий</button>
    </>)
}

function Pagination({ page, pageCount, onChangePage }) {
    const windowSize = 2;
    let firstPage = Math.max(1, page - windowSize);
    let lastPage = Math.min(pageCount, page + windowSize);

    const pages = Array.from(
        { length: lastPage - firstPage + 1},
        (_, i) => firstPage + i
    );
    
    return (
        <ul className="pagination">
            <li className="page-item" onClick={() => onChangePage(page-1)}>&lt;</li>
            {pages.map(p => (
                <li key={p} className={`page-item${p === page ? ' active' : ''}`} onClick={() => onChangePage(p)}>{p}</li>
            ))}
            <li className="page-item" onClick={() => onChangePage(page+1)}>&gt;</li>
        </ul>
    )
}

function Comments({ dtoId, isTest, baseComments, basePages, commentsPerPage, isAuthorized}) {
    const [comments, setComments] = useState(baseComments);
    const [page, setPage] = useState(1);
    const [pageCount, setPageCount] = useState(basePages);
    const [isPosting, setIsPosting] = useState(false);
    const [isLoading, setIsLoading] = useState(false);

    const handlePageChange = async (newPage) => {
        if (isPosting || isLoading) return;
        
        const targetPage = Math.max(1, Math.min(newPage, pageCount));
        if (targetPage == page) return;
        
        setPage(targetPage);
        
        setIsLoading(true);
        const result = await getComments(dtoId, targetPage, commentsPerPage, isTest);
        const newComments = await result.json();
        const pages = parseInt(newComments.pages);
        if (pages !== pageCount) {
            setPageCount(pages)
        }
        setComments(newComments.comments);
        setIsLoading(false);
    }
    
    const handleCommentSubmit = async () => {
        if (isPosting || isLoading) return;
        
        const textarea = document.getElementById("new-comment");
        const command = new CommentCommand(textarea.value);

        setIsPosting(true);
        const postResult = await postComment(command, dtoId, isTest);
        setIsPosting(false);

        setIsLoading(true);
        const result = await getComments(dtoId, 1, commentsPerPage, isTest);
        const newComments = await result.json();
        const pages = parseInt(newComments.pages);
        if (pages !== pageCount) {
            setPageCount(pages)
        }
        setComments(newComments.comments);
        setIsLoading(false);
    }
    
    return (
        <>
            { isAuthorized && <PostComment onPost={handleCommentSubmit}/>}
            {isLoading ? <div className="loading">
                    <img src="/files/icons/loading.png" alt="Загрузка контента" className="loading-icon"/>
                </div> 
                : comments.map(comment => <Comment key={comment.id} baseComment={comment}/>)}
            {pageCount > 1 && <Pagination page={page} pageCount={pageCount} onChangePage={handlePageChange}/>}
        </>
    )
}

function getComments(id, page, amountPerPage, isTest) {
    const endpointBase = isTest ? 'tests' : 'users';
    const endpointEnd = isTest ? 'comments' : 'profilecomments';
    return fetch(`/api/${endpointBase}/${id}/${endpointEnd}?page=${page}&amountPerPage=${amountPerPage}`);
}

class CommentCommand {
    constructor (content) {
        this.content = content;
    }
}

function postComment(data, id, isTest) {
    const endpointBase = isTest ? 'tests' : 'users';
    const endpointEnd = isTest ? 'comments' : 'profilecomments';
    return fetch(`/api/${endpointBase}/${id}/${endpointEnd}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json;charset=utf-8',
        },
        body: JSON.stringify(data)
    });
}

export default Comments;