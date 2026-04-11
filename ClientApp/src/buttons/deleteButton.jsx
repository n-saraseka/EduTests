import {useState} from "react";
import ConfirmationModal from "../modals/confirmationModal.jsx";

function DeleteButton({entityType, onDelete}) {
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
    
    const deletePrompts = {
        "comment": "комментарий",
        "user": "пользователя",
        "test": "тест",
        "ban": "блокировку",
        "tag": "тег",
        "question": "вопрос",
        "answer": "ответ",
        "result": "результат"
    }
    
    const objectPrompt = (type) => type in deletePrompts ? deletePrompts[type] : "объект неизвестного типа"; 

    const openDeleteModal = () => {
        setIsDeleteModalOpen(true);
    }

    const onDeleteModalConfirm = async () => {
        const postResult = await onDelete();
        setIsDeleteModalOpen(false);
    }

    const onDeleteModalCancel = () => {
        setIsDeleteModalOpen(false);
    }
    
    return (<>
        <img src="/files/icons/close.png"
             alt="Удалить"
             onClick={openDeleteModal}
             className="delete"/>
        {isDeleteModalOpen && (<ConfirmationModal title={`Удалить ${objectPrompt(entityType)}?`}
                                                 subtitle="Отменить это действие будет невозможно. Все данные будут утеряны"
                                                 onConfirm={onDeleteModalConfirm}
                                                 onCancel={onDeleteModalCancel}/>)}
    </>)
}

export default DeleteButton;