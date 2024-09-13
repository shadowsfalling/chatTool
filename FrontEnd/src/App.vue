<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <v-btn icon @click="toggleDrawer">
        <v-icon>mdi-menu</v-icon>
      </v-btn>
      <v-toolbar-title>Chat-tool Application</v-toolbar-title>
      <v-spacer></v-spacer>    
    </v-app-bar>

    <!-- Navigation drawer with v-model controlling its visibility -->
    <v-navigation-drawer app v-model="drawer" :permanent="false">
      <v-list dense>
        <v-list-subheader>Sidemenu</v-list-subheader>

        <v-list-item to="/">
          <template v-slot:prepend>
            <v-icon>mdi-home</v-icon>
          </template>
          <v-list-item-title>Home</v-list-item-title>
        </v-list-item>

        <v-list-item to="/rooms">
          <template v-slot:prepend>
            <v-icon>mdi-view-list</v-icon>
          </template>
          <v-list-item-title>Rooms</v-list-item-title>
        </v-list-item>

        <v-list-item v-if="!isAuthenticated" to="/login">
          <template v-slot:prepend>
            <v-icon>mdi-login</v-icon>
          </template>
          <v-list-item-title>Login</v-list-item-title>
        </v-list-item>

        <v-list-item v-else @click="logout">
          <template v-slot:prepend>
            <v-icon>mdi-logout</v-icon>
          </template>
          <v-list-item-title>Logout</v-list-item-title>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <router-view v-if="!loading" />
      <div v-else>Loading...</div>
    </v-main>

    <v-footer app color="primary" dark>
      <span>&copy; {{ new Date().getFullYear() }}</span>
    </v-footer>
  </v-app>
</template>

<script>
import { ref, onMounted } from 'vue';
import router from './router';
import { useAuthStore } from './stores/authStore';
import { AuthService } from './services/authService';

export default {
  setup() {
    const authStore = useAuthStore();
    const isAuthenticated = ref(false);
    const drawer = ref(false); // Controls the drawer visibility
    const user = ref(null);
    const loading = ref(true);

    const toggleDrawer = () => {
      drawer.value = !drawer.value; // Toggle drawer visibility
    };

    onMounted(async () => {
      await authStore.checkAuth();
      isAuthenticated.value = authStore.isAuthenticated;
      user.value = authStore.user;
      loading.value = false;
    });

    return {
      isAuthenticated,
      user,
      loading,
      drawer, // Drawer state
      toggleDrawer // Method to toggle drawer
    };
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