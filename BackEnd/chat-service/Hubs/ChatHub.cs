using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {
        // Methode für das Senden von Nachrichten an alle Clients
        public async Task SendMessage(string user, string message)
        {

            Console.WriteLine("user " + user + "  -  " + "message " + message);

            try
            {

                // todo: hier mit rabbitMQ dem roomService Bescheid geben, dass es eine neue Nachricht in einem Chat gibt!
                // dann aus dem RoomService an den NotificationService?! eine Nachricht an die User in dem Raum, dass es eine neue Nachricht in dem Raum gibt

                // Sende die Nachricht an alle verbundenen Clients
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            catch (Exception ex)
            {
                // Logge den Fehler für Debugging
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw; // Lasse die Exception weiterlaufen, um die tatsächliche Ursache zu sehen
            }
        }

        // Hinzufügen des Benutzers zur Gruppe beim Beitritt zu einem Raum
        public async Task JoinRoom(string roomName)
        {

            Console.WriteLine("join room " + roomName);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined the room {roomName}");
        }

        // Entfernen des Benutzers aus der Gruppe beim Verlassen
        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has left the room {roomName}");
        }

        // Nachricht nur an die Gruppe senden
        public async Task SendMessageToRoom(string roomName, string user, string message)
        {
            Console.WriteLine("roomname " + roomName + "  -  user " + user + "  -  " + "message " + message);

            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
            // await Clients.All.SendAsync("ReceiveMessage", user, message); // sends really to all clients
        }
    }
}