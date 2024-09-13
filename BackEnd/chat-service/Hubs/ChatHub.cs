using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ChatService.Repositories;

namespace ChatService.Hubs
{
    public class ChatHub : Hub
    {

        private readonly MessageRepository _messageRepository;

        public ChatHub(MessageRepository messageRepository) {
            _messageRepository = messageRepository;
        }

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

        // Dictionary zum Speichern, welcher Benutzer in welchem Raum ist
        private static Dictionary<string, string> userRooms = new Dictionary<string, string>();
        // todo: das Dictionary mit Daten aus der DB füllen, welcher User in welchem Raum ist und dann entsprechend informieren



        // Methode zum Beitritt zu einem Raum
        public async Task JoinRoom(string roomName)
        {
            string connectionId = Context.ConnectionId;

            // Benutzer zum Raum hinzufügen
            // todo: ist der User berechtigt?
            if (userRooms.ContainsKey(connectionId))
            {
                userRooms[connectionId] = roomName;
            }
            else
            {
                userRooms.Add(connectionId, roomName);
            }

            await Groups.AddToGroupAsync(connectionId, roomName);
            Console.WriteLine($"{connectionId} ist Raum {roomName} beigetreten.");
        }

        // Methode zum Verlassen eines Raums
        public async Task LeaveRoom(string roomName)
        {
            string connectionId = Context.ConnectionId;

            if (userRooms.ContainsKey(connectionId) && userRooms[connectionId] == roomName)
            {
                userRooms.Remove(connectionId);
            }

            await Groups.RemoveFromGroupAsync(connectionId, roomName);
            Console.WriteLine($"{connectionId} hat Raum {roomName} verlassen.");
        }

        // Methode zum Versenden einer Nachricht
        public async Task SendMessageToRoom(string roomName, string user, string messageContent)
        {
            // Nachricht an den Raum senden
            // todo: ist der User berechtigt?
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, messageContent);


            // save message
            var message = new Message() {
                Content = messageContent,
                RoomId = int.Parse(roomName),
                UserId = user,
                Timestamp = DateTime.Now
            };
            await _messageRepository.AddAsync(message);


            // // Benachrichtige Benutzer, die nicht im Raum sind
            // var usersNotInRoom = userRooms.Where(u => u.Value != roomName).Select(u => u.Key);

            // foreach (var unir in usersNotInRoom)
            // {
            //     await Clients.Client(unir).SendAsync("ReceiveNotification", $"Neue Nachricht im Raum {roomName} von {user}: {message}");
            // }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Entferne den Benutzer aus dem Tracking, wenn er die Verbindung trennt
            string connectionId = Context.ConnectionId;

            if (userRooms.ContainsKey(connectionId))
            {
                userRooms.Remove(connectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
