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

connection.on("ErrorOccurred", (errorMessage) => {
    console.error("Server error:", errorMessage);
});

connection.on("NewChatCreated", (chat) => {
    console.log("Received new chat created message:", chat);
    console.log("Is chat relevant:", isChatRelevant(chat));
    console.log("Is chat on list:", inChatOnList(chat));

    if (isChatRelevant(chat)) {
        if (inChatOnList(chat)) {
            removeChatFromList(chat);
        }
        addChatToList(chat);
    }

    if (!isChatRelevant(chat) && inChatOnList(chat)) {
        setTimeout(() => {
            removeChatFromList(chat);
        }, 100);
    }   
});

connection.on("ChatRemoved", (chat) => {
    console.log("Received chat removed message:", chat);

    // Check if the chat is on the list
    if(inChatOnList(chat))
        removeChatFromList(chat);
});

function isChatRelevant(chat) {
    // Check if the chat's topic is in the user's topics
    console.log(userTopics, chat.topicName, userTopics.includes(chat.topicName))

    return userTopics.includes(chat.topicName);
}

function inChatOnList(chat) {
    // Check if chat is already in the list
    console.log(chatIds, chat.chatId, chatIds.includes(chat.chatId))

    return chatIds.includes(chat.chatId);
}

function removeChatFromList(chat) {
/*    console.log("Removing chat from list:", chat);
    const chatElement = document.getElementById(`chat-${chat.chatId}`);
    if (chatElement) {
        console.log("Removing chat element:", chatElement);
        chatElement.remove();
    }*/
    location.reload();
}

function addChatToList(chat) {
/*    console.log("Adding chat to list:", chat);
    const chatList = document.getElementById("chatList");
    const li = document.createElement("li");
    li.id = `chat-${chat.chatId}`;
    li.className = "list-group-item d-flex justify-content-between align-items-center";
    li.innerHTML = `
        <span class="chat-topic">${chat.topicName}</span> -
        <span class="chat-users">
            ${chat.usernamesInChat && chat.usernamesInChat.length > 0
            ? chat.usernamesInChat.join(", ")
            : '<span class="badge bg-secondary rounded-pill">0</span>'}
        </span>
        <a href="/HelpDesk/JoinChat?chatId=${chat.chatId}"
           class="btn btn-secondary btn-sm">Join Chat</a>
    `;
    chatList.appendChild(li);*/
    location.reload();
}
