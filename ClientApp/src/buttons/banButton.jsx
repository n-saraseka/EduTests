import {useState} from "react";
import ConfirmationModalWithDate from "../modals/confirmationModalWithDate.jsx";

function BanButton({onBan}) {
    const [isBanModalOpen, setIsBanModalOpen] = useState(false);
    let currDate = new Date();
    currDate.setDate(currDate.getDate() + 1);
    
    const openBanModal = () => {
        setIsBanModalOpen(true);
    }

    const onBanModalConfirm = async () => {
        const postResult = await onBan();
        setIsBanModalOpen(false);
    }

    const onBanModalCancel = () => {
        setIsBanModalOpen(false);
    }

    return (<>
        <img src="/files/icons/ban.png"
             alt="Удалить"
             onClick={openBanModal}
             className="delete"/>
        {isBanModalOpen && (<ConfirmationModalWithDate title="Заблокировать пользователя?"
                                                  dateTitle="Дата разблокировки: " 
                                                  dateMin={currDate.toISOString().split("T")[0]}
                                                  onConfirm={onBanModalConfirm}
                                                  onCancel={onBanModalCancel}/>)}
    </>)
}

export default BanButton;