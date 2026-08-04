import { create } from "zustand";
import { toast } from "sonner";
import { authService } from "../services/authService";
import type { AuthState } from "../types/store";
import { persist } from "zustand/middleware";
import { useChatStore } from "./useChatStore";
import { userService } from "@/services/userService";

let refreshPromise: Promise<void> | null = null;
export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      accessToken: null,
      user: null,
      loading: false,

      setAccessToken: (accessToken) => {
        set({ accessToken });
      },
      clearState: () => {
        set({ accessToken: null, user: null, loading: false });
        useChatStore.getState().reset();
        localStorage.clear();
        sessionStorage.clear();
      },

      signUp: async (username, password, email, firstName, lastName) => {
        try {
          set({ loading: true });
          //gọi api

          await authService.signUp(
            username,
            password,
            email,
            firstName,
            lastName,
          );
          toast.success(
            "Đăng ký thành công! Bạn sẽ được chuyển sang trang đăng nhập.",
          );
        } catch (error) {
          console.log(error);
          toast.error("Đăng ký không thành công");
        } finally {
          set({ loading: false });
        }
      },

      signIn: async (username, password) => {
        try {
          //set loading
          set({ loading: true });
          get().clearState();
          //fetch api
          const { accessToken, user } = await authService.signIn(
            username,
            password,
          );
          //set access token
          set({ user: user });
          get().setAccessToken(accessToken);

          useChatStore.getState().fetchConversations();

          toast.success("Chào mừng bạn quay lại với Moji!");
        } catch (error) {
          console.log(error);
          toast.error("Đăng nhập không thành công");
          throw error;
        } finally {
          set({ loading: false });
        }
      },

      signOut: async () => {
        try {
          get().clearState();
          toast.success("Đăng xuất thành công!");
        } catch (error) {
          console.log(error);
          toast.error("Đăng xuất không thành công");
        }
      },
      fetchMe: async () => {
        try {
          set({ loading: true });
          const user = await authService.fetchMe();
          set({ user });
        } catch (error) {
          console.error(error);
          set({ user: null, accessToken: null });
          toast.error("Đã xảy ra lỗi trong quá trình lấy thông tin người dùng");
        } finally {
          set({ loading: false });
        }
      },

      refresh: () => {
        if (refreshPromise) return refreshPromise;
        set({ loading: true });
        refreshPromise = (async () => {
          try {
            const accessToken = await authService.refresh();

            set({ accessToken: accessToken });
            if (!get().user) {
              await get().fetchMe();
            }
          } catch (error) {
            console.error(error);
            get().clearState();
            toast.error("Phiên đăng nhập đã hết hạn! Vui lòng đăng nhập lại!");
            throw error;
          } finally {
            set({ loading: false });
            refreshPromise = null;
          }
        })();
        return refreshPromise;
      },
      setUser: (user) => {
        set({ user });
      },
    }),
    {
      name: "auth-storage",
      partialize: (state) => ({ user: state.user }), //chỉ persist lại user
    },
  ),
);
