const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

function startConnection() {
    connection.start().then(() => {
        console.log("Connected to ConsultantHub");
        return connection.invoke("JoinConsultantPanel");
    }).then(() => {
        console.log("Joined consultant panel");
    }).catch(err => {
        console.error("Error:", err);
        setTimeout(startConnection, 5000); // Attempt to reconnect after 5 seconds
    });
}

startConnection();

connection.onreconnecting((error) => {
    console.log("Connection lost. Attempting to reconnect...", error);
    if (error && error.message.includes("dotnet-watch reload socket error")) {
        console.log("dotnet-watch reload socket error detected. Reloading page...");
        location.reload();
    }
});

connection.onreconnected((connectionId) => {
    console.log("Connection reestablished. ConnectionId:", connectionId);
    connection.invoke("JoinConsultantPanel", "@User.Identity.Name");
});

connection.onclose((error) => {
    console.log("Connection closed. Error:", error);
    if (error && error.message.includes("dotnet-watch reload socket closed")) {
        console.log("dotnet-watch reload socket closed detected. Reloading page...");
        location.reload();
    } else {
        setTimeout(startConnection, 5000);
    }
});

connection.on("ErrorOccurred", (errorMessage) => {
    console.error("Server error:", errorMessage);
});

connection.on("ReceiveRelevantChats", (chats) => {
    console.log("Received relevant chats:", chats);
    updateChatList(chats);
});

connection.on("NewChatCreated", (chat) => {
    console.log("Received new chat created message:", chat);
    addChatToList(chat);
});

connection.on("ChatTopicChanged", (chat) => {
    console.log("Received chat topic changed message:", chat);
    updateChatInList(chat);
});

function updateChatList(chats) {
    console.log("Updating chat list with:", chats);
    const chatList = document.getElementById("chatList");
    chatList.innerHTML = "";
    chats.forEach(chat => {
        addChatToList(chat);
    });
}

function addChatToList(chat) {
    console.log("Adding chat to list:", chat);
    const chatList = document.getElementById("chatList");
    const li = document.createElement("li");
    li.className = "list-group-item d-flex justify-content-between align-items-center";
    li.innerHTML = `
        ${chat.topicName} -
        ${chat.usernamesInChat && chat.usernamesInChat.length > 0
            ? chat.usernamesInChat.join(", ")
            : '<span class="badge bg-secondary rounded-pill">0</span>'}
        <a href="/HelpDesk/JoinChat?chatId=${chat.chatId}"
           class="btn btn-secondary btn-sm">Join Chat</a>
    `;
    chatList.appendChild(li);
}

function updateChatInList(chat) {
    console.log("Updating chat in list:", chat);
    const chatItems = document.querySelectorAll("chatlist");
    if (!chatlist) {
        console.error("Chat list not found");
        return;
    }
    chatList.innerHTML = "";
    chats.forEach(chat => {
        addChatToList(chat);
    });
}