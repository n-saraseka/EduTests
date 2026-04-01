import ReportButton from "../buttons/reportButton.jsx";
import BanButton from "../buttons/banButton.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";
import BannedLabel from "../bannedLabel.jsx";

function ProfileUsername({username, userId, currentUserId, currentUserGroup, isBanned}) {
    const handleUserDelete = async () => {
        const result = await deleteUser(userId);
        window.location.replace("/home");
    }
    
    const handleUserBan = async () => {
        const banReason = document.getElementById("modal-text").value;
        const dateValue = document.getElementById("modal-date").value;
        
        console.log(dateValue);
        
        let unbanDate = (dateValue === '') ? null : new Date(dateValue);
        
        const banCommand = new BanCommand(banReason, unbanDate);
        
        const result = await banUser(userId, banCommand);
    }
    
    return (<>
        <h2>{username}</h2>
        {isBanned ? <BannedLabel /> : <>
            {userId !== currentUserId && <ReportButton entityType={0} entityId={userId}/>}
            {(["Administrator", "Moderator"].includes(currentUserGroup) && currentUserId !== userId) &&
            <BanButton onBan={handleUserBan}/>}
            {(currentUserGroup === "Administrator" && currentUserId !== userId) &&
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