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
};
