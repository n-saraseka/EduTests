import ReportButton from "./reportButton.jsx";

function ProfileUsername({username, userId}) {
    return (<>
        <h2>{username}</h2>
        <ReportButton entityType={0} entityId={userId}/>
    </>)
}

export default ProfileUsername;