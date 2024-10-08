import * as signalR from "@microsoft/signalr";

let connection;

export async function startSignalRConnection() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5176/chatHub") // Ersetze durch die URL deines SignalR-Hubs
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    await connection
        .start()
        .then(() => console.log("SignalR connected."))
        .catch(err => console.error("SignalR connection error: ", err));

    return connection;
}

export function onReceiveMessage(callback) {
    if (!connection) return;

    connection.on("ReceiveMessage", callback);
}

export function sendMessage(message) {
    if (!connection) return;

    connection.invoke("SendMessage", "string", message).catch(err => console.error(err));
}