import { getApiErrorMessage } from "@/lib/api-error";
import { friendService } from "@/services/friendService";
import type { FriendState } from "@/types/store";
import { create } from "zustand";

export const useFriendStore = create<FriendState>((set) => ({
  loading: false,
  searchByUsername: async (username, limit = 10, pageNumber = 1) => {
    try {
      set({ loading: true });
      const users = await friendService.searchByUsername(
        username,
        limit,
        pageNumber,
      );
      return users;
    } catch (error) {
      console.error("Lỗi xảy ra khi tìm user bằng username", error);
      return null;
    } finally {
      set({ loading: false });
    }
  },
  addFriend: async (to, message) => {
    try {
      set({ loading: true });
      const resultMessage = await friendService.sendFriendRequest(to, message);
      return resultMessage;
    } catch (error) {
      console.error("Lỗi xảy ra khi gửi kết bạn", error);
      throw new Error(
        getApiErrorMessage(error, "Không thể gửi lời mời kết bạn."),
      );
    } finally {
      set({ loading: false });
    }
  },
}));
