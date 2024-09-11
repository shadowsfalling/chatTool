<template>
    <v-sheet
      class="message-container"
      :class="{'message-right': isCurrentUser, 'message-left': !isCurrentUser}"
      elevation="2"
    >
      <div class="message-content">
        <v-avatar>
          <img :src="avatar || 'https://via.placeholder.com/50'" alt="Profile Picture" />
        </v-avatar>
        <div class="message-details">
          <div class="message-header">
            <span class="username">{{ username }}</span>
            <span class="timestamp">{{ formattedTimestamp }}</span>
          </div>
          <div class="message-text">
            {{ message }}
          </div>
        </div>
      </div>
    </v-sheet>
  </template>
  
  <script>
  export default {
    props: {
      username: {
        type: String,
        required: true,
      },
      message: {
        type: String,
        required: true,
      },
      timestamp: {
        type: String,
        required: true,
      },
      isCurrentUser: {
        type: Boolean,
        required: true,
      },
      avatar: {
        type: String,
        required: false,
      }
    },
    computed: {
      formattedTimestamp() {
        const date = new Date(this.timestamp);
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
      }
    }
  };
  </script>
  
  <style scoped>
  .message-container {
    display: flex;
    max-width: 60%;
    margin: 10px 0;
    padding: 10px;
    border-radius: 10px;
    background-color: #f0f0f0;
  }
  
  .message-content {
    display: flex;
    align-items: center;
  }
  
  .message-details {
    margin-left: 10px;
  }
  
  .username {
    font-weight: bold;
    margin-right: 10px;
  }
  
  .timestamp {
    font-size: 0.8rem;
    color: #888;
  }
  
  .message-text {
    margin-top: 5px;
  }
  
  .message-left {
    justify-content: flex-start;
  }
  
  .message-right {
    background-color: #e0f7fa;
  }
  </style>