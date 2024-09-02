const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMessage", (userId, message, username) => {
    const messagesList = document.getElementById("messagesList");
    if (messagesList) {
        const msg = document.createElement("div");
        msg.className = "message";
        msg.innerHTML = `<strong>${username}:</strong> ${message}`;
        messagesList.appendChild(msg);
        messagesList.scrollTop = messagesList.scrollHeight;
    } else {
        console.error("Element 'messagesList' not found.");
    }
});

connection.on("UserJoined", (userId, username) => {
    const messagesList = document.getElementById("messagesList");
    const msg = document.createElement("div");
    msg.className = "system-message";
    msg.innerHTML = `<em>${username} has joined the chat.</em>`;
    messagesList.appendChild(msg);
    messagesList.scrollTop = messagesList.scrollHeight;
});

connection.on("UserLeft", (userId, username) => {
    const messagesList = document.getElementById("messagesList");
    const msg = document.createElement("div");
    msg.className = "system-message";
    msg.innerHTML = `<em>${username} has left the chat.</em>`;
    messagesList.appendChild(msg);
    messagesList.scrollTop = messagesList.scrollHeight;

    console.log("Userleft:", username);

    const userList = document.getElementById("userList");
    const userItems = userList.getElementsByTagName("li");
    for (let i = 0; i < userItems.length; i++) {
        if (userItems[i].textContent.trim() === username) {
            console.log(userItems[i].textContent.trim());
            // Without delay item is not removed from the list correctly
            setTimeout(() => {
                userList.removeChild(userItems[i]);
            }, 100);
        }
    }
});

connection.on("UpdateUserList", (users) => {
    const userList = document.getElementById("userList");
    userList.innerHTML = "";
    users.forEach(user => {
        const li = document.createElement("li");
        li.className = "list-group-item";
        li.textContent = user.username;
        li.setAttribute("data-user-id", user.id);
        userList.appendChild(li);
    });
});

connection.on("TopicChanged", (newTopicId) => {
    const topicSelect = document.getElementById("topicSelect");
    topicSelect.value = newTopicId;
    const messagesList = document.getElementById("messagesList");
    const msg = document.createElement("div");
    msg.className = "system-message";
    msg.innerHTML = `<em>The chat topic has been changed to ${topicSelect.options[topicSelect.selectedIndex].text}.</em>`;
    messagesList.appendChild(msg);
});

document.getElementById("sendMessageButton").addEventListener("click", sendMessage);
document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        sendMessage(event);
    }
});

if(document.getElementById("leaveChatButton"))
    document.getElementById("leaveChatButton").addEventListener("click", leaveChat);

const changeTopicButton = document.getElementById("changeTopicButton");
if (changeTopicButton) {
    changeTopicButton.addEventListener("click", changeTopic);
}

let isConnected = false;

connection.start().then(() => {
    isConnected = true;
    console.log("Connected to SignalR Hub");
    const chatId = document.getElementById("chatId").value;
    connection.invoke("JoinChat", chatId)
        .then(() => console.log("JoinChat invoked successfully"))
        .catch(err => console.error("Error invoking JoinChat:", err));
}).catch(err => console.error(err.toString()));

function sendMessage(event) {
    if (!isConnected) {
        console.error("Not connected to SignalR Hub");
        return;
    }

    const userId = document.getElementById("userId").value;
    const message = document.getElementById("messageInput").value;
    const chatId = document.getElementById("chatId").value;

    if (message.trim() === "") return;

    connection.invoke("SendMessage", chatId, message, userId)
        .then(() => {
            console.log("SendMessage invoked successfully");
            document.getElementById("messageInput").value = "";
        })
        .catch(err => {
            console.error("Error invoking SendMessage:", err);
            if (err.message) console.error("Error message:", err.message);
            if (err.stack) console.error("Error stack:", err.stack);
        });
    event.preventDefault();
}

function leaveChat() {
    const chatId = document.getElementById("chatId").value;
    let checkboxStatus = true;

    if (document.getElementById("isSavedCheckbox")) {
        const saveChatCheckbox = document.getElementById("isSavedCheckbox");
        console.log("Checkbox checked:", saveChatCheckbox.checked);

        if (!saveChatCheckbox.checked)
            checkboxStatus = false;
    }
    
    connection.invoke("LeaveChat", chatId, checkboxStatus)
        .then(() => {
            console.log("Left chat successfully");
            window.location.href = "/Home/LeaveChat";
        })
        .catch(err => console.error("Error leaving chat:", err.toString()));
}

function changeTopic() {
    const chatId = document.getElementById("chatId").value;
    const newTopicId = document.getElementById("topicSelect").value;
    connection.invoke("ChangeChatTopic", chatId, newTopicId)
        .then(() => console.log("Topic changed successfully"))
        .catch(err => console.error("Error changing topic:", err.toString()));
}

function updateUserList(userId, username, joined) {
    const userList = document.getElementById("userList");
    userList.innerHTML = "";
    users.forEach(user => {
        const li = document.createElement("li");
        li.className = "list-group-item";
        li.textContent = user.username;
        li.setAttribute("data-user-id", user.id);
        userList.appendChild(li);
    });
}