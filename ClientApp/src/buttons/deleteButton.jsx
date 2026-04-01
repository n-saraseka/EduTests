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
    
    return (<>
        <img src="/files/icons/close.png"
             alt="Удалить"
             onClick={openDeleteModal}
             className="delete"/>
        {isDeleteModalOpen && (<ConfirmationModal title={`Удалить ${entityType === "comment" ? 'комментарий' 
            : entityType === "user" ? 'пользователя' : 
                entityType === "test" ? "тест" : "блокировку"}?`}
                                                 subtitle="Отменить это действие будет невозможно. Все данные будут утеряны"
                                                 onConfirm={onDeleteModalConfirm}
                                                 onCancel={onDeleteModalCancel}/>)}
    </>)
}

export default DeleteButton;