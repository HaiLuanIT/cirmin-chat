import api from "@/lib/axios";

const DEFAULT_LIMIT = 10;
const DEFAULT_PAGE_NUMBER = 1;
export const friendService = {
  async searchByUsername(
    username: string,
    limit = DEFAULT_LIMIT,
    pageNumber = DEFAULT_PAGE_NUMBER,
  ) {
    const res = await api.get(
      `/users?username=${username}&limit=${limit}&pagenumber=${pageNumber}`,
    );
    return res.data;
  },

  async sendFriendRequest(receiverId: string, message?: string) {
    const res = await api.post("/friends/requests", { receiverId, message });
    return res.data;
  },

  async getAllFriendRequests() {
    try {
      const res = await api.get("/friends/requests");
      const { inbound, outbound } = res.data;
      return { inbound, outbound };
    } catch (error) {
      console.error("Lỗi khi gửi get all friend request", error);
    }
  },

  async acceptFriendRequest(requestId: string) {
    try {
      const res = await api.post(`/friends/requests/${requestId}/accept`);
      return res.data;
    } catch (error) {
      console.error("Lỗi khi gửi accept request", error);
    }
  },
  async rejectFriendRequest(requestId: string) {
    try {
      await api.post(`/friends/requests/${requestId}/reject`);
    } catch (error) {
      console.error("Lỗi khi gửi reject request", error);
    }
  },

  async getAllFriend() {
    try {
      const res = await api.get("/friends");
      return res.data;
    } catch (error) {
      console.error("Lỗi khi getAllFriend", error);
    }
  },
};
