import { usePresenceStore } from "@/stores/usePresenceStore";
import React, { useEffect } from "react";
import { signalRService } from "@/services/signalRService";
import * as signalR from "@microsoft/signalr";
import { useAuthStore } from "@/stores/useAuthStore";

export const useSignalR = () => {
  const setOnlineUsers = usePresenceStore((s) => s.setOnlineUsers);
  const setStatusUser = usePresenceStore((s) => s.setStatusUser);
  const token = useAuthStore((s) => s.accessToken);

  useEffect(() => {
    if (!token) {
      console.warn("Không có access token");
      return;
    }
    const connection = signalRService.initConnection(
      useAuthStore.getState().accessToken,
    );

    const handleSetOnlineUsers = (userIds: string[]) => {
      setOnlineUsers(userIds);
    };

    const handleSetUserStatus = (userId: string, isOnline: boolean) => {
      setStatusUser(userId, isOnline);
    };
    connection.on("GetOnlineUsers", handleSetOnlineUsers);

    connection.on("UserStatusChanged", handleSetUserStatus);

    if (connection.state === signalR.HubConnectionState.Disconnected) {
      connection
        .start()
        .then(() => console.log("Kết nối với signalR thành công."))
        .catch((err) => console.error("Kết nối với signalR thất bại", err));
    }
    return () => {
      connection.off("GetOnlineUsers", handleSetOnlineUsers);
      connection.off("UserStatusChanged", handleSetUserStatus);
      console.log("Tắt lắng nghe sự kiện SignalR");
    };
  }, [token, setOnlineUsers, setStatusUser]);
};
