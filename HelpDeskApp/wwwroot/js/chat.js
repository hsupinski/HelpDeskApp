const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .configureLogging(signalR.LogLevel.Information)  // Zwiększenie poziomu logowania
    .build();

connection.on("ReceiveMessage", (userId, message) => {
    console.log("ReceiveMessage event received");
    const messagesList = document.getElementById("messagesList");
    if (messagesList) {
        const msg = document.createElement("div");
        msg.textContent = `${userId}: ${message}`;
        messagesList.appendChild(msg);
    } else {
        console.error("Element 'messagesList' nie został znaleziony.");
    }
});

connection.start().then(() => {
    console.log("SignalR connection established successfully.");
}).catch(err => console.error("Error establishing SignalR connection: " + err.toString()));

document.getElementById("sendMessageButton").addEventListener("click", async event => {
    const userId = document.getElementById("userId").value;
    const message = document.getElementById("messageInput").value;
    const chatId = document.getElementById("chatId").value;

    console.log(`Sending message: ${message} from userId: ${userId} to chatId: ${chatId}`);

    try {
        await connection.invoke("SendMessage", chatId, message, userId);
        console.log("Message sent successfully.");
    } catch (err) {
        console.error("Failed to send message: " + err.toString());
    }

    event.preventDefault();
});
