const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Trace)
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

document.getElementById("sendMessageButton").addEventListener("click", sendMessage);
document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        sendMessage(event);
    }
});

let isConnected = false;

connection.start().then(() => {
    isConnected = true;
    console.log("Connected to SignalR Hub");
    const chatId = document.getElementById("chatId").value;
    connection.invoke("JoinChat", chatId)
        .then(() => console.log("JoinChat invoked successfully"))
        .catch(err => console.error("Error invoking JoinChat:", err));
}).catch(err => console.error(err.toString()));

document.getElementById("leaveChatButton").addEventListener("click", () => {
    const chatId = document.getElementById("chatId").value;
    connection.invoke("LeaveChat", chatId)
        .then(() => {
            console.log("Left chat successfully");
            window.location.href = "/Home/LeaveChat";
        })
        .catch(err => console.error("Error leaving chat:", err.toString()));
});
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

