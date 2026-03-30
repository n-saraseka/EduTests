import ReportButton from "../buttons/reportButton.jsx";
import DeleteButton from "../buttons/deleteButton.jsx";

function ProfileUsername({username, userId, currentUserId, currentUserGroup}) {
    const handleUserDelete = async (id) => {
        const result = await deleteUser(id);
        window.location.replace("/home");
    }
    
    return (<>
        <h2>{username}</h2>
        {userId !== currentUserId && <ReportButton entityType={0} entityId={userId}/>}
        {(currentUserGroup === "Administrator" && currentUserId !== userId) && <DeleteButton entityType={0} onDelete={() => handleUserDelete(userId)}/>}
    </>)
}

function deleteUser(id) {
    return fetch(`/api/users/${id}`, {
        method: 'DELETE'
    });
}

export default ProfileUsername;