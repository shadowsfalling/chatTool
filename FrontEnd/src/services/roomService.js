import axios from 'axios';

export const RoomService = {
  async getAccessibleRooms() {
    const response = await axios.get('http://localhost:5177/api/room/all', {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`
      }
    });
    return response.data;
  },
  async getMessagesOfRoom(roomId) {
    const response = await axios.get('http://localhost:5176/' + roomId + '/messages', {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`
      }
    });
    return response.data;
  }
};