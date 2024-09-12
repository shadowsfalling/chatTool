<template>
  <v-container>
    <h3>Liste der Räume</h3>

    <v-list>

      <v-list-item v-for="room in rooms" :key="room.id">{{ room.name }}
        <v-list-item-action>
          <v-btn color="primary" @click="joinRoom(room.id)">Join Room</v-btn>
          <v-btn :to="'/room/' + room.id">Enter Room</v-btn>
        </v-list-item-action>
      </v-list-item>
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
import { useAuthStore } from '@/stores/authStore'; // Deinen Pinia-Store importieren

export default {
  setup() {
    const authStore = useAuthStore();  // Zugriff auf den AuthStore

    return { authStore }; // Rückgabe des Stores für die Nutzung in der Komponente
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
      const response = await axios.get('http://localhost:5177/api/Room/all');
      this.rooms = response.data;
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