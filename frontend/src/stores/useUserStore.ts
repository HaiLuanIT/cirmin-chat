import { create } from "zustand";
import { useAuthStore } from "./useAuthStore";
import { userService } from "@/services/userService";
import { toast } from "sonner";
import type { UserState } from "@/types/store";
import { useChatStore } from "./useChatStore";

export const useUserStore = create<UserState>(() => ({
  uploadAvatarUrl: async (formData) => {
    try {
      const { user, setUser } = useAuthStore.getState();
      const data = await userService.upLoadAvatar(formData);

      if (user) {
        setUser({
          ...user,
          avatarUrl: data.avatarUrl,
          updatedAt: data.updatedAt,
        });
      }

      useChatStore.getState().fetchConversations();
    } catch (error) {
      console.error("Lỗi khi upload avatar", error);
      throw error;
    }
  },
  updateUserInfo: async (displayName, email, bio) => {
    try {
      const { user, setUser } = useAuthStore.getState();
      const data = await userService.updateUserInfor(displayName, email, bio);
      if (user) {
        setUser({
          ...user,
          displayName: data.displayName,
          email: data.email,
          bio: data.bio,
          updatedAt: data.updatedAt ?? user.updatedAt,
        });
      }
      toast.success("Cập nhật thông tin cá nhân thành công.");
    } catch (error) {
      console.error("Lỗi khi update user info", error);
      throw error;
    }
  },
  changePassword: async (oldPassword, newPassword) => {
    await userService.changePassword(oldPassword, newPassword);
    toast.success("Thay đổi mật khẩu thành công. Vui lòng đăng nhập lại");
  },
}));
