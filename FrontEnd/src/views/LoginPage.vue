<template>
  <v-container>
    <v-row justify="center">
      <v-col cols="12" md="4">
        <v-card>
          <v-card-title>Login</v-card-title>
          <v-card-text>
            <!-- Updated form to listen for keydown.enter -->
            <v-form>
              <v-text-field
                label="Username"
                v-model="username"
                @keydown.enter="login" 
              />
              <v-text-field
                label="Password"
                type="password"
                v-model="password"
                @keydown.enter="login" 
              />
              <v-alert v-if="errorMessage" type="error">{{ errorMessage }}</v-alert>
              <!-- Button to trigger login -->
              <v-btn color="primary" @click="login">Login</v-btn>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import { AuthService } from '@/services/authService';
import router from '@/router';

export default {
  data() {
    return {
      username: '',
      password: '',
      errorMessage: ''
    };
  },
  methods: {
    login() {
      AuthService.login(this.username, this.password)
        .then(() => {
          router.push('/'); // Redirect to home page after successful login
        })
        .catch(() => {
          this.errorMessage = 'Invalid credentials';
        });
    }
  }
};
</script>