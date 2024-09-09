<template>
  <div>
    <h3>Chat (Room: {{ currentRoom }})</h3>
    <select v-model="selectedRoom" @change="joinRoom">
      <option value="room1">Room 1</option>
      <option value="room2">Room 2</option>
    </select>

    <ul>
      <li v-for="(message, index) in messages" :key="index">{{ message }}</li>
    </ul>

    <input v-model="newMessage" placeholder="Schreibe eine Nachricht..." @keyup.enter="sendChatMessage" />
    <button @click="sendChatMessage">Senden</button>
  </div>

  <v-btn @click="joinRoom()">join room</v-btn>

</template>

<script>
import { ref, onMounted } from 'vue';
import { startSignalRConnection } from '@/services/signalrService';

export default {
  setup() {
    const messages = ref([]);
    const newMessage = ref("");
    const selectedRoom = ref("room1"); // Standardraum
    const currentRoom = ref("");

    let connection;

    // Verbindung herstellen und dem Raum beitreten
    onMounted(() => {
      connection = startSignalRConnection();

      connection.start().then(() => {
        console.log("SignalR-Verbindung hergestellt");
        joinRoom(); // Beitreten zum Standardraum
      }).catch(err => console.error("Fehler bei der SignalR-Verbindung:", err));

      connection.on("ReceiveMessage", (user, message) => {
        console.log(user + "Nachricht empfangen:", message);
        messages.value.push(message);
      });
    });

    // Nachricht an den Raum senden
    const sendChatMessage = () => {
      if (newMessage.value.trim()) {

        console.log(currentRoom, currentRoom.value);

        connection.invoke("SendMessageToRoom", currentRoom.value, "string", newMessage.value).catch(err => console.error(err));
        newMessage.value = "";
      }
    };

    // Raum wechseln und beitreten
    const joinRoom = () => {
      if (currentRoom.value) {
        connection.invoke("LeaveRoom", currentRoom.value).catch(err => console.error(err));
      }

      currentRoom.value = selectedRoom.value;
      connection.invoke("JoinRoom", currentRoom.value).catch(err => console.error(err));
    };

    return {
      messages,
      newMessage,
      selectedRoom,
      currentRoom,
      sendChatMessage,
      joinRoom,
    };
  }
}
</script>

<style scoped>
/* Optional: Stil für die Chat-Komponente */
</style>