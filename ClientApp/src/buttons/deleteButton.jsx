import {useState} from "react";
import ConfirmationModal from "../modals/confirmationModal.jsx";

function DeleteButton({entityType, onDelete}) {
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);

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
    
    const objectPrompt = (type) => {
        switch (type) {
            case "comment":
                return "комментарий";
            case "user":
                return "пользователя";
            case "test":
                return "тест";
            case "ban":
                return "блокировку";
            case "tag":
                return "тег";
            case "question":
                return "вопрос";
            case "answer":
                return "ответ";
            default:
                return "объект неизвестного типа";
        }
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