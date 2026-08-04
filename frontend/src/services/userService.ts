import api from "@/lib/axios";

export const userService = {
  upLoadAvatar: async (formData: FormData) => {
    const res = await api.put("/users/me/avatar", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    if (res.status === 400 || res.status === 409) {
      throw new Error(res.data.message);
    }
    return res.data;
  },
  updateUserInfor: async (
    displayName?: string,
    email?: string,
    bio?: string,
  ) => {
    const res = await api.patch("/users/me/update-information", {
      displayName,
      email,
      bio,
    });
    return res.data;
  },
  changePassword: async (oldPassword: string, newPassword: string) => {
    const res = await api.patch("/users/me/change-password", {
      oldPassword,
      newPassword,
    });
    return res.data;
  },
};
