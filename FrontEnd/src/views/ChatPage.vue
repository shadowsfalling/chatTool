<template>
  <div class="chat-container">
    <h3>Chat (Room: {{ currentRoom }})</h3>
    <h3 v-if="!authStore.loading">Welcome, {{ user?.username }}</h3>

    <v-sheet class="chat-sheet" border>
      <!-- Nachrichtenliste -->
      <ChatMessage v-for="(msg, index) in messages" :key="index" :username="msg.username" :message="msg.text"
        :timestamp="msg.timestamp" :isCurrentUser="msg.username === user.username" :avatar="msg.avatar" />
    </v-sheet>

    <!-- Eingabefeld und Button fixiert über dem Footer -->
    <v-footer fixed class="footer-input">
      <v-container class="d-flex justify-space-between align-center" fluid>
        <v-text-field v-model="newMessage" placeholder="Schreibe eine Nachricht..." @keyup.enter="sendChatMessage" flat
          hide-details full-width />
        <v-btn icon @click="sendChatMessage">
          <v-icon>mdi-send</v-icon>
        </v-btn>
      </v-container>
    </v-footer>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { startSignalRConnection } from '@/services/signalrService';
import { RoomService } from '@/services/roomService';
import ChatMessage from '../components/ChatMessage.vue';
import { useAuthStore } from '@/stores/authStore';
import { useRoute } from 'vue-router';

export default {
  components: { ChatMessage },
  setup() {
    const messages = ref([]);
    const newMessage = ref("");
    const currentRoom = ref("");
    const authStore = useAuthStore();
    const route = useRoute();

    let connection;

    onMounted(async () => {
      await authStore.checkAuth();
      connection = await startSignalRConnection();

      connection.on("ReceiveMessage", (user, message) => {
        messages.value.push({
          username: user, text: message, timestamp: new Date().toISOString(), avatar: ''
        });
      });

      RoomService.getMessagesOfRoom(route.params.roomId).then((m) => {
        m.forEach(message => {
          messages.value.push({
            username: message.userId, text: message.content, timestamp: message.timestamp, avatar: ''
          })
        });
      });

      currentRoom.value = route.params.roomId;
      await joinRoom();
    });

    const sendChatMessage = () => {
      if (newMessage.value.trim()) {
        connection.invoke("SendMessageToRoom", currentRoom.value, authStore.user.username, newMessage.value)
          .catch(err => console.error(err));
        newMessage.value = "";
      }
    };

    const joinRoom = async () => {
      if (currentRoom.value) {
        connection.invoke("LeaveRoom", currentRoom.value)
          .catch(err => console.error(err));
      }

      connection.invoke("JoinRoom", currentRoom.value)
        .catch(err => console.error(err));
    };

    return {
      messages,
      newMessage,
      currentRoom,
      sendChatMessage,
      joinRoom,
      user: authStore.user,
      authStore,
    };
  }
}
</script>

<style scoped>
.chat-sheet {
  width: calc(100% - 40px);
  height: 85%;
  overflow-y: scroll;
  margin: 20px;
}

.chat-container {
  height: 100%;
  padding-bottom: 80px;
}

.messages-list {
  list-style: none;
  padding: 0;
  margin-bottom: 80px;
}

.footer-input {
  background-color: #fff;
  box-shadow: 0 -1px 5px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  display: inline-block;
}

.footer-input .v-input {
  margin-right: 20px;
}

.v-footer {
  position: fixed;
  bottom: 38px;
  left: 256px;
  right: 0;
  padding: 0 16px;
  width: calc(100% - 256px);
}
</style>