import axios from 'axios';

export const RoomService = {
  async getAccessibleRooms() {
    const response = await axios.get('http://localhost:5177/api/room/all', {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`
      }
    });
    return response.data;
  }
};