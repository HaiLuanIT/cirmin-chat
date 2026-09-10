import { friendService } from "@/services/friendService";
import type { FriendState } from "@/types/store";
import { create } from "zustand";

export const useFriendStore = create<FriendState>((set) => ({
  loading: false,
  receivedList: [],
  sentList: [],
  friends: [],
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
      throw error;
    } finally {
      set({ loading: false });
    }
  },
  getAllFriendRequests: async () => {
    try {
      set({ loading: true });

      const result = await friendService.getAllFriendRequests();
      if (!result) return;

      const { inbound, outbound } = result;
      set({ receivedList: inbound, sentList: outbound });
    } catch (error) {
      console.error("Lỗi xảy ra khi getAllFriendRequests", error);
    } finally {
      set({ loading: false });
    }
  },
  acceptFriendRequest: async (requestId) => {
    try {
      set({ loading: true });

      await friendService.acceptFriendRequest(requestId);

      set((state) => ({
        receivedList: state.receivedList.filter((r) => r.id !== requestId),
      }));
    } catch (error) {
      console.error("Lỗi xảy ra khi acceptFriendRequest", error);
      throw error;
    } finally {
      set({ loading: false });
    }
  },

  rejectFriendRequest: async (requestId) => {
    try {
      set({ loading: true });

      await friendService.rejectFriendRequest(requestId);

      set((state) => ({
        receivedList: state.receivedList.filter((r) => r.id !== requestId),
      }));
    } catch (error) {
      console.error("Lỗi xảy ra khi rejectFriendRequest", error);
    } finally {
      set({ loading: false });
    }
  },
  getFriends: async () => {
    try {
      set({ loading: true });
      const friendList = await friendService.getAllFriend();
      set({ friends: friendList });
    } catch (error) {
      console.error("Lỗi xảy ra khi getAllFriends", error);
      set({ friends: [] });
    } finally {
      set({ loading: false });
    }
  },
}));
