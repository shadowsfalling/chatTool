import axios from 'axios';

const API_URL = 'http://localhost:5176/api/Auth/login';  // Replace with your API endpoint

export const AuthService = {
  login(username, password) {
    return axios
      .post(`${API_URL}`, { username, password })
      .then(response => {
        if (response.data.token) {
          localStorage.setItem('user', JSON.stringify(response.data));
        }
        return response.data;
      });
  },

  logout() {
    localStorage.removeItem('user');
  },

  getCurrentUser() {
    return JSON.parse(localStorage.getItem('user'));
  },

  isAuthenticated() {
    return !!localStorage.getItem('user');
  }
};

// Add a request interceptor
axios.interceptors.request.use(
    config => {
      const user = AuthService.getCurrentUser();
      if (user && user.token) {
        config.headers['Authorization'] = `Bearer ${user.token}`;
      }
      return config;
    },
    error => {
      return Promise.reject(error);
    }
  );