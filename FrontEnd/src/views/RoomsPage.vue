<template>
  <div>
    <h3>Available Rooms</h3>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <v-list dense>
      <v-list-subheader>Rooms</v-list-subheader>

      <v-list-item v-for="room in rooms" :to="'/room/' + room.id" :key="room.id">{{ room.name }}

      </v-list-item>
    </v-list>

  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { RoomService } from '@/services/roomService';

export default {
  setup() {
    const rooms = ref([]);
    const error = ref(null);

    onMounted(() => {
      RoomService.getAccessibleRooms()
        .then(response => {
          rooms.value = response;
        })
        .catch(err => {
          error.value = err.response?.data?.message || 'Fehler beim Abrufen der Räume';
        });
    });

    return {
      rooms,
      error
    };
  }
};
</script>

<style scoped>
.error-message {
  color: red;
  font-weight: bold;
  margin-bottom: 20px;
}
</style>