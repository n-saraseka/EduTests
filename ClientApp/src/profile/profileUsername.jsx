import ReportButton from "../buttons/reportButton.jsx";
import BanButton from "../buttons/banButton.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";
import BannedLabel from "../bannedLabel.jsx";
import PromoteButton from "../buttons/promoteButton.jsx";

function ProfileUsername({user, currentUserId, currentUserGroup, isBanned}) {
    const handleUserDelete = async () => {
        const result = await deleteUser(user.id);
        window.location.replace("/home");
    }
    
    const handleUserBan = async () => {
        const banReason = document.getElementById("modal-text").value;
        const dateValue = document.getElementById("modal-date").value;
        
        let unbanDate = (dateValue === '') ? null : new Date(dateValue);
        
        const banCommand = new BanCommand(banReason, unbanDate);
        
        const result = await banUser(user.id, banCommand);
    }
    
    return (<>
        <h2>{user.username}</h2>
        {isBanned ? <BannedLabel /> : <>
            {user.id !== currentUserId && <ReportButton entityType={0} entityId={user.id}/>}
            {(["Administrator", "Moderator"].includes(currentUserGroup) && currentUserId !== user.id) &&
            <BanButton onBan={handleUserBan}/>}
            {(user.id !== currentUserId && currentUserGroup === "Administrator") && <PromoteButton user={user}/>}
            {(currentUserGroup === "Administrator" && currentUserId !== user.id) &&
            <DeleteButton entityType={"user"} onDelete={() => handleUserDelete}/>}
        </>}
    </>)
}

class BanCommand {
    constructor(reason, unbanDate) {
        this.reason = reason;
        this.unbanDate = unbanDate;
    }
}

function banUser(id, data) {
    return fetch(`/api/users/${id}/bans`, {
        method: "POST",
        headers: {'Content-Type': 'application/json;charset=utf-8'},
        body: JSON.stringify(data)
    });
}

function deleteUser(id) {
    return fetch(`/api/users/${id}`, {
        method: 'DELETE'
    });
}

export default ProfileUsername;