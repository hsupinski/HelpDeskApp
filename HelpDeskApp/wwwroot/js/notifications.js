const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

notificationConnection.on("ReceiveNotification", (message, moreInfo = null) => {
    showNotification(message, moreInfo);
});

function showNotification(message, moreInfo) {
    const notificationArea = document.getElementById("notificationArea");
    const notificationDiv = document.createElement("div");

    notificationDiv.className = "alert alert-info alert-dismissible fade show";
    let notificationContent = `<strong>Notification:</strong> ${message}`;

    if (message.startsWith("New chat created for topic:") || message.startsWith("Redirected chat to topic:")) {
        notificationContent += ` <a href="/HelpDesk/Panel" class="alert-link">Go to HelpDesk Panel</a>`;
    }

    notificationDiv.innerHTML = `
        ${notificationContent}
        ${moreInfo ? `<br><small>${moreInfo}</small>` : ""}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;


    notificationDiv.querySelector('.btn-close').addEventListener('click', () => {
        notificationDiv.remove();
        removeNotificationFromStorage(message, moreInfo);
    });

    notificationDiv.querySelector('.alert-link')?.addEventListener('click', () => {
        notificationDiv.remove();
        removeNotificationFromStorage(message, moreInfo);
    });

    notificationArea.appendChild(notificationDiv);

    // Remove notification after 5 seconds if no more info is provided
    if (moreInfo == null) {
        setTimeout(() => {
            notificationDiv.remove();
            removeNotificationFromStorage(message, moreInfo);
        }, 5000);
    }

    if(!isNotificationOnList(message, moreInfo))
        saveNotificationToStorage(message, moreInfo);
}

function saveNotificationToStorage(message, moreInfo) {
    let notifications = JSON.parse(localStorage.getItem("notifications")) || [];

    notifications.push({ message, moreInfo });

    localStorage.setItem("notifications", JSON.stringify(notifications));
}

function isNotificationOnList(message, moreInfo) {
    let notifications = JSON.parse(localStorage.getItem("notifications")) || [];

    return notifications.some(notification => notification.message === message && notification.moreInfo === moreInfo);
}

function removeNotificationFromStorage(message, moreInfo) {
    let notifications = JSON.parse(localStorage.getItem("notifications")) || [];

    notifications = notifications.filter(notification => notification.message !== message || notification.moreInfo !== moreInfo);

    localStorage.setItem("notifications", JSON.stringify(notifications));
}

function restoreNotifications() {
    let notifications = JSON.parse(localStorage.getItem("notifications")) || [];

    notifications.forEach(notification => {
        showNotification(notification.message, notification.moreInfo);
    });
}

document.addEventListener("DOMContentLoaded", restoreNotifications);

if (!window.location.pathname.includes("Home/Chat")) {
    notificationConnection.start()
        .then(() => {
            console.log("Connection started.");
        })
        .catch((err) => {
            console.error(err.toString());
        });
}