import {useState} from "react";
import ConfirmationModal from "../modals/confirmationModal.jsx";

function PromoteButton({user}) {
    const [isMod, setIsMod] = useState(user.group !== 0);
    const [confirmationModalOpen, setConfirmationModalOpen] = useState(false);
    
    const onPromoteDemote = async () => {
        let command;
        if (isMod) {
            command = new ChangeUserGroupCommand(0);
        }
        else {
            command = new ChangeUserGroupCommand(1);
        }
        const response = await changeUserGroup(user.id, command);
        if (response.ok) {
            if (isMod) {
                setIsMod(false);
            }
            else {
                setIsMod(true);
            }
        }
        setConfirmationModalOpen(false);
    }
    
    return (<div className="promote">
            <img className="edit-icon" src={isMod ? "/files/icons/demote.png" : "/files/icons/promote.png"}
                 alt={isMod ? "Снять с должности модератора" : "Повысить до модератора"} 
                 onClick={() => setConfirmationModalOpen(true)}/>
            {confirmationModalOpen && <ConfirmationModal 
                title={isMod? "Снять пользователя с должности модератора?" : "Повысить пользователя до модератора?"} 
                onConfirm={onPromoteDemote}
                onCancel={() => setConfirmationModalOpen(false)}/>}
    </div>
    );
}

class ChangeUserGroupCommand {
    constructor(userGroup) {
        this.userGroup = userGroup;
    }
}

function changeUserGroup(id, command) {
    return fetch(`/api/users/${id}/group`, {
        method: "PATCH",
        body: JSON.stringify(command),
        headers: {
            "Content-Type": "application/json",
        }
    });
}

export default PromoteButton;
