import {useState} from "react";
import ConfirmationModalWithText from "./modals/confirmationModalWithText.jsx";

function ReportButton({ reportFunction, entityType, entityId }) {
    const [isReportModalOpen, setIsReportModalOpen] = useState(false);

    const openReportModal = () => {
        setIsReportModalOpen(true);
    }

    const onReportModalConfirm = async () => {
        const reportText = document.getElementById('modal-text').value;
        const command = new ReportCommand(entityType, entityId, reportText);

        const postResult = await reportFunction(command);
        setIsReportModalOpen(false);
    }

    const onReportModalCancel = () => {
        setIsReportModalOpen(false);
    }
    
    return ( <>
            <img src="/files/icons/report.png" alt="Пожаловаться" onClick={openReportModal} className="comment-report"/>
            {isReportModalOpen && (<ConfirmationModalWithText title={"Пожаловаться?"}
                                                          onCancel={onReportModalCancel}
                                                          onConfirm={onReportModalConfirm}/>)}
    </>);
}

class ReportCommand {
    constructor(entityType, entityId, text) {
        this.entityType = entityType;
        this.entityId = entityId;
        this.text = text;
    }
}

export default ReportButton;