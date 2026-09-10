import type { PresenceState } from "@/types/store";
import { create } from "zustand";

export const usePresenceStore = create<PresenceState>((set, get) => ({
  onlineUsers: new Set(),
  setOnlineUsers: (userIds) => {
    set({ onlineUsers: new Set(userIds) });
  },
  isOnline: (userId) => {
    return get().onlineUsers.has(userId);
  },
  setStatusUser: (userId, isOnline) => {
    if (isOnline) {
      set((state) => ({
        onlineUsers: new Set(state.onlineUsers).add(userId),
      }));
    } else {
      set((state) => {
        const users = new Set(state.onlineUsers);
        users.delete(userId);
        return {
          onlineUsers: users,
        };
      });
    }
  },
  clearState: () => {},
}));
