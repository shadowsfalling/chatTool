<template>
  <v-container>
    <h3>Liste der Räume</h3>

    <v-list>

      <v-expansion-panels variant="accordion">
        <v-expansion-panel v-for="room in rooms" :key="room.id">
        
          <v-expansion-panel-title>
            <v-btn icon="mdi-login" :to="'/room/' + room.id"></v-btn>
            &nbsp;
            {{ room.name }} 
          </v-expansion-panel-title>

          <v-expansion-panel-text>

            <v-btn :to="'/room/' + room.id">Enter Room</v-btn>
            <br />

            <v-btn color="primary" @click="joinRoom(room.id)">Join Room-Notificationlist</v-btn>
          </v-expansion-panel-text>

        </v-expansion-panel>
      </v-expansion-panels>

    </v-list>

    <v-alert v-if="successMessage" type="success">
      {{ successMessage }}
    </v-alert>

    <v-alert v-if="errorMessage" type="error">
      {{ errorMessage }}
    </v-alert>
  </v-container>
</template>

<script>
import axios from 'axios';
import { useAuthStore } from '@/stores/authStore';
import { RoomService } from '@/services/roomService';

export default {
  setup() {
    const authStore = useAuthStore();

    return { authStore };
  },
  data() {
    return {
      rooms: [],
      successMessage: '',
      errorMessage: '',
    };
  },
  async created() {
    // Räume vom Backend abrufen
    try {
      this.rooms = await RoomService.getRooms();
    } catch (error) {
      console.error('Fehler beim Abrufen der Räume:', error);
    }
  },
  methods: {
    async joinRoom(roomId) {
      try {
        const userId = this.authStore.user.id;  // Benutzer-ID aus dem Store abrufen
        await axios.post(`http://localhost:5177/api/Room/${roomId}/add-user`, {
          userId: userId
        });
        this.successMessage = `Du bist dem Raum beigetreten!`;

        setTimeout(() => {
          this.successMessage = "";
        }, 2000);

      } catch (error) {
        this.errorMessage = 'Fehler beim Beitreten des Raumes.';
        console.error(error);
      }
    },
  },
};
</script>

<style scoped>
.v-container {
  max-width: 600px;
  margin: auto;
  padding-top: 20px;
}
</style>