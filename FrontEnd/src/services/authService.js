import axios from 'axios';
import router from '../router';

const API_URL = 'http://localhost:5176/api/Auth';  // Replace with your API endpoint

export const AuthService = {
  login(username, password) {
    return axios
      .post(`${API_URL}/login`, { username, password })
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
  },

  async getMe() {
    const user = AuthService.getCurrentUser();
    if (!user || !user.token) {
      return;
    }

    const response = await axios.get(`${API_URL}/me`, {
      headers: {
        Authorization: `Bearer ${user.token}`
      }
    });

    return response.data;
  },
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

// Add a response interceptor
axios.interceptors.response.use(
  response => {
    return response;  // Handle the response as needed
  },
  error => {
    if (error.response && error.response.status === 401) {
      console.log('401 Unauthorized - Redirecting to login');
      AuthService.logout();
      router.push('/login');
    }
    return Promise.reject(error);
  }
);
