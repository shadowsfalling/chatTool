<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-toolbar-title>My Application</v-toolbar-title>
    </v-app-bar>

    <v-navigation-drawer app v-model="isAuthenticated" permanent>
      <v-list dense>
        <v-list-subheader>Sidemenu</v-list-subheader>

        <v-list-item to="/">
          <template v-slot:prepend>
            <v-icon icon="mdi-home"></v-icon>
          </template>
          <v-list-item-title v-text="'Home'"></v-list-item-title>
        </v-list-item>

        <v-list-item to="/chat">
          <template v-slot:prepend>
            <v-icon icon="mdi-chat"></v-icon>
          </template>
          <v-list-item-title v-text="'Chat'"></v-list-item-title>
        </v-list-item>

        <v-list-item to="/rooms">
          <template v-slot:prepend>
            <v-icon icon="mdi-view-list"></v-icon>
          </template>
          <v-list-item-title v-text="'Rooms'"></v-list-item-title>
        </v-list-item>

        <v-list-item v-if="!isAuthenticated" to="/login">
          <template v-slot:prepend>
            <v-icon icon="mdi-login"></v-icon>
          </template>
          <v-list-item-title v-text="'Login'"></v-list-item-title>
        </v-list-item>

        <v-list-item v-else @click="logout">
          <template v-slot:prepend>
            <v-icon icon="mdi-logout"></v-icon>
          </template>
          <v-list-item-title v-text="'Logout'"></v-list-item-title>
        </v-list-item>
      </v-list>

    </v-navigation-drawer>

    <v-main>
      <router-view />
    </v-main>

    <v-footer app color="primary" dark>
      <span>&copy; {{ new Date().getFullYear() }}</span>
    </v-footer>
  </v-app>
</template>

<script>

import router from './router';
import { AuthService } from './services/authService';

export default {
  computed: {
    isAuthenticated() {
      return AuthService.isAuthenticated();
    }
  },
  methods: {
    logout() {
      AuthService.logout();
      this.isAuthenticated = false;
      router.push('/login');
    }
  }
};
</script>

<style>
body {
  font-family: 'Roboto', sans-serif;
}
</style>