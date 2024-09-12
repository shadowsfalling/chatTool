<template>
    <v-container>
      <h3>Erstelle einen neuen Raum</h3>
  
      <v-form v-model="valid" @submit.prevent="createRoom">
        <v-text-field
          label="Raumname"
          v-model="roomName"
          :rules="[v => !!v || 'Raumname ist erforderlich']"
          required
        ></v-text-field>
  
        <v-checkbox v-model="isPublic" label="Öffentlich"></v-checkbox>
  
        <v-btn :disabled="!valid" type="submit" color="primary">Raum erstellen</v-btn>
      </v-form>
  
      <v-alert v-if="successMessage" type="success">
        {{ successMessage }}
      </v-alert>
  
      <v-alert v-if="errorMessage" type="error">
        {{ errorMessage }}
      </v-alert>
    </v-container>
  </template>
  
  <script>
  import router from '@/router';
import axios from 'axios';
  
  export default {
    data() {
      return {
        roomName: '',
        isPublic: false,
        valid: false,
        successMessage: '',
        errorMessage: ''
      };
    },
    methods: {
      async createRoom() {

        // todo: outsource in service!

        try {
          const response = await axios.post('http://localhost:5177/api/Room', {
            name: this.roomName,
            isPublic: this.isPublic
          });
          this.successMessage = `Raum "${response.data.name}" wurde erfolgreich erstellt!`;
          this.roomName = '';
          this.isPublic = false;
          console.log("response", response, response.data);

          router.push("/room/" + response.data.name);

        } catch (error) {
          this.errorMessage = 'Fehler beim Erstellen des Raumes.';
          console.error(error);
        }
      }
    }
  };
  </script>
  
  <style scoped>
  .v-container {
    max-width: 600px;
    margin: auto;
    padding-top: 20px;
  }
  </style>